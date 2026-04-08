using System.IO;
using _Game.Scripts.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SniperTowerSetup
{
    private const string TowersSoFolder = "Assets/_Game/ScriptableObjects/Towers";
    private const string PrefabsFolder = "Assets/_Game/Prefabs";

    private const string BaseTowerPrefabPath = PrefabsFolder + "/Tower_Base.prefab";
    private const string SniperPrefabPath = PrefabsFolder + "/Tower_Sniper.prefab";
    private const string SniperMetricsPath = TowersSoFolder + "/SniperTowerMetrics_Default.asset";
    private const string SniperTowerDataPath = TowersSoFolder + "/Tower_Sniper.asset";

    [MenuItem("Tools/DefenderOfGrids/Setup Sniper Tower (Create Assets + Prefab)")]
    public static void SetupSniperTowerAssets()
    {
        EnsureFolderExists("Assets/Editor");
        EnsureFolderExists("Assets/_Game");
        EnsureFolderExists("Assets/_Game/ScriptableObjects");
        EnsureFolderExists(TowersSoFolder);
        EnsureFolderExists(PrefabsFolder);

        var metrics = EnsureSniperMetrics();
        var sniperPrefab = EnsureSniperPrefab(metrics);
        var towerData = EnsureSniperTowerData(sniperPrefab);

        EditorUtility.DisplayDialog(
            "Sniper Tower Setup",
            $"Created/updated:\n- {SniperMetricsPath}\n- {SniperPrefabPath}\n- {SniperTowerDataPath}\n\nNext: run 'Tools/DefenderOfGrids/Add Sniper To Shop (Current Scene)' to add a shop button in the open scene.",
            "OK"
        );
    }

    [MenuItem("Tools/DefenderOfGrids/Add Sniper To Shop (Current Scene)")]
    public static void AddSniperToShopInCurrentScene()
    {
        var towerData = AssetDatabase.LoadAssetAtPath<TowerData>(SniperTowerDataPath);
        if (towerData == null)
        {
            EditorUtility.DisplayDialog("Sniper Tower", "Sniper TowerData not found. Run setup first.", "OK");
            return;
        }

        var buttons = Object.FindObjectsByType<TowerShopButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (buttons == null || buttons.Length == 0)
        {
            EditorUtility.DisplayDialog("Sniper Tower", "No TowerShopButton found in the open scene.", "OK");
            return;
        }

        var template = buttons[0];
        var clone = Object.Instantiate(template.gameObject, template.transform.parent);
        clone.name = "ShopButton_Sniper";

        var shopButton = clone.GetComponent<TowerShopButton>();
        var so = new SerializedObject(shopButton);
        so.FindProperty("_towerData").objectReferenceValue = towerData;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Selection.activeGameObject = clone;

        EditorUtility.DisplayDialog("Sniper Tower", "Sniper shop button added to the current scene (duplicated from an existing shop button).", "OK");
    }

    private static SniperTowerMetrics EnsureSniperMetrics()
    {
        var metrics = AssetDatabase.LoadAssetAtPath<SniperTowerMetrics>(SniperMetricsPath);
        if (metrics != null) return metrics;

        metrics = ScriptableObject.CreateInstance<SniperTowerMetrics>();
        metrics.range = 10f;
        metrics.fireRate = 0.7f;
        metrics.damage = 8f;
        metrics.lineThickness = 0.05f;
        metrics.pierceCount = 1;
        metrics.retargetInterval = 0.2f;

        AssetDatabase.CreateAsset(metrics, SniperMetricsPath);
        AssetDatabase.SaveAssets();
        return metrics;
    }

    private static GameObject EnsureSniperPrefab(SniperTowerMetrics metrics)
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(SniperPrefabPath);
        if (existing != null)
        {
            // Make sure it has the attack component wired.
            EnsureAttackComponent(existing, metrics);
            return existing;
        }

        var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BaseTowerPrefabPath);
        if (basePrefab == null)
        {
            throw new FileNotFoundException($"Base tower prefab not found at '{BaseTowerPrefabPath}'.");
        }

        var clone = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
        clone.name = "Tower_Sniper";
        EnsureAttackComponent(clone, metrics);

        var saved = PrefabUtility.SaveAsPrefabAsset(clone, SniperPrefabPath);
        Object.DestroyImmediate(clone);
        AssetDatabase.SaveAssets();
        return saved;
    }

    private static void EnsureAttackComponent(GameObject prefabOrInstance, SniperTowerMetrics metrics)
    {
        // In case we were handed a prefab asset, open it for editing.
        var path = AssetDatabase.GetAssetPath(prefabOrInstance);
        if (!string.IsNullOrEmpty(path) && PrefabUtility.IsPartOfPrefabAsset(prefabOrInstance))
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                EnsureAttackComponent(root, metrics);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
            return;
        }

        var attack = prefabOrInstance.GetComponent<SniperTowerAttack>();
        if (attack == null) attack = prefabOrInstance.AddComponent<SniperTowerAttack>();

        // Best-effort auto-wire using TowerController fields.
        var controller = prefabOrInstance.GetComponent<TowerController>();
        if (controller != null)
        {
            var so = new SerializedObject(controller);
            var firePoint = so.FindProperty("_firePoint")?.objectReferenceValue as Transform;
            var weaponPart = so.FindProperty("_weaponPart")?.objectReferenceValue as Transform;
            var enemyLayer = so.FindProperty("_enemyLayer");

            var attackSo = new SerializedObject(attack);
            attackSo.FindProperty("_metrics").objectReferenceValue = metrics;
            if (firePoint != null) attackSo.FindProperty("_firePoint").objectReferenceValue = firePoint;
            if (weaponPart != null) attackSo.FindProperty("_weaponPart").objectReferenceValue = weaponPart;
            if (enemyLayer != null) attackSo.FindProperty("_enemyLayer").intValue = enemyLayer.intValue;
            attackSo.ApplyModifiedPropertiesWithoutUndo();
        }
        else
        {
            var attackSo = new SerializedObject(attack);
            attackSo.FindProperty("_metrics").objectReferenceValue = metrics;
            attackSo.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static TowerData EnsureSniperTowerData(GameObject sniperPrefab)
    {
        var existing = AssetDatabase.LoadAssetAtPath<TowerData>(SniperTowerDataPath);
        if (existing != null)
        {
            var so = new SerializedObject(existing);
            so.FindProperty("towerName").stringValue = "Sniper Tower";
            so.FindProperty("cost").intValue = 200;
            so.FindProperty("prefab").objectReferenceValue = sniperPrefab;
            so.FindProperty("towerType").enumValueIndex = (int)TowerTypes.Sniper;
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            return existing;
        }

        var data = ScriptableObject.CreateInstance<TowerData>();
        data.towerName = "Sniper Tower";
        data.cost = 200;
        data.prefab = sniperPrefab;

        var soNew = new SerializedObject(data);
        soNew.FindProperty("towerType").enumValueIndex = (int)TowerTypes.Sniper;
        soNew.ApplyModifiedPropertiesWithoutUndo();

        AssetDatabase.CreateAsset(data, SniperTowerDataPath);
        AssetDatabase.SaveAssets();
        return data;
    }

    private static void EnsureFolderExists(string assetFolder)
    {
        if (AssetDatabase.IsValidFolder(assetFolder)) return;

        var parent = Path.GetDirectoryName(assetFolder)?.Replace("\\", "/");
        var name = Path.GetFileName(assetFolder);
        if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name)) return;

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolderExists(parent);
        }

        AssetDatabase.CreateFolder(parent, name);
    }
}

