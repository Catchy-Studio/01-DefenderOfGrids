using __Project.Systems.DevDebugUI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DevDebugUIInstaller
{
    [MenuItem("DoGTools/DefenderOfGrids/Dev Debug UI/Add To Current Scene")]
    public static void AddToCurrentScene()
    {
        var existing = Object.FindFirstObjectByType<DevDebugPanel>(FindObjectsInactive.Include);
        if (existing != null)
        {
            Selection.activeObject = existing.gameObject;
            EditorUtility.DisplayDialog("Dev Debug UI", "DevDebugPanel already exists in this scene.", "OK");
            return;
        }

        var go = new GameObject("DevDebugPanel");
        go.AddComponent<DevDebugPanel>();
        Selection.activeObject = go;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Dev Debug UI", "Added DevDebugPanel to the current scene.", "OK");
    }
}

