using System;
using System.Collections;
using System.Collections.Generic;
using _NueCore.Common.KeyValueDict;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.Common.Utility;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace _NueExtras.StockSystem._StockSpawnerSubSystem
{
    public class StockSpawnerController : MonoBehaviour
    {
        [SerializeField, TabGroup("References")]
        private Canvas movingCanvas;

        [SerializeField, TabGroup("References")]
        private KeyValueDict<StockTypes, StockSpawnObject> stockSpawnObjectUIDict;

        [SerializeField, TabGroup("References")]
        private KeyValueDict<StockTypes, StockSpawnObject> stockSpawnObject3DDict;

        private const int MaxSpawnCount = 40;
        private const int DefaultPoolCapacity = 25;
        private const int MaxPoolSize = 50;

        private readonly Dictionary<StockTypes, ObjectPool<StockSpawnObject>> _uiPools = new();
        private readonly Dictionary<StockTypes, ObjectPool<StockSpawnObject>> _3dPools = new();
        private readonly Dictionary<StockSpawnObject, (StockTypes type, bool is3D)> _activeObjects = new();

        public static Dictionary<StockTypes, Transform> StockMoveTargetDict { get; private set; } = new();

        private Camera Camera => CameraStatic.MainCamera;
        private Transform CameraTransform => Camera.transform;
        private int _movingObjectCount;
        private bool _isSfxPlaying;
        private Tween _delayTween;
        private readonly object _poolLock = new object();

        private void Awake()
        {
            InitializePools();
            RegisterREvents();
        }

        private void OnDestroy()
        {
            _delayTween?.Kill();
            DisposePools();
        }

        #region Pool Management

        private void InitializePools()
        {
            lock (_poolLock)
            {
                foreach (var kvp in stockSpawnObjectUIDict)
                {
                    if (!_uiPools.ContainsKey(kvp.Key))
                    {
                        _uiPools[kvp.Key] = CreatePool(kvp.Value, movingCanvas.transform, kvp.Key, false);
                    }
                }

                foreach (var kvp in stockSpawnObject3DDict)
                {
                    if (!_3dPools.ContainsKey(kvp.Key))
                    {
                        _3dPools[kvp.Key] = CreatePool(kvp.Value, null, kvp.Key, true);
                    }
                }
            }
        }

        private ObjectPool<StockSpawnObject> CreatePool(StockSpawnObject prefab, Transform parent, StockTypes type,
            bool is3D)
        {
            return new ObjectPool<StockSpawnObject>(
                createFunc: () =>
                {
                    $"New StockSpawnObject created. {prefab.gameObject.name}".NLog();
                    var obj = Instantiate(prefab, parent);
                    obj.gameObject.SetActive(false);
                    return obj;
                },
                actionOnGet: obj =>
                {
                    if (obj != null && obj.gameObject != null)
                    {
                        obj.ResetState();
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                        obj.gameObject.SetActive(true);

                        lock (_poolLock)
                        {
                            _activeObjects[obj] = (type, is3D);
                        }
                    }
                },
                actionOnRelease: obj =>
                {
                    if (obj != null && obj.gameObject != null)
                    {
                        obj.ResetState();
                        obj.gameObject.SetActive(false);

                        lock (_poolLock)
                        {
                            _activeObjects.Remove(obj);
                        }
                    }
                },
                actionOnDestroy: obj =>
                {
                    if (obj != null && obj.gameObject != null)
                    {
                        lock (_poolLock)
                        {
                            _activeObjects.Remove(obj);
                        }

                        Destroy(obj.gameObject);
                    }
                },
                collectionCheck: true,
                defaultCapacity: DefaultPoolCapacity,
                maxSize: MaxPoolSize
            );
        }


        private void DisposePools()
        {
            lock (_poolLock)
            {
                _activeObjects.Clear();

                foreach (var pool in _uiPools.Values)
                {
                    pool?.Dispose();
                }

                foreach (var pool in _3dPools.Values)
                {
                    pool?.Dispose();
                }

                _uiPools.Clear();
                _3dPools.Clear();
            }
        }

        private StockSpawnObject GetFromPool(StockTypes type, bool is3D)
        {
            lock (_poolLock)
            {
                var pools = is3D ? _3dPools : _uiPools;
                if (pools.TryGetValue(type, out var pool))
                {
                    return pool.Get();
                }

                return null;
            }
        }

        private void ReturnToPool(StockSpawnObject obj)
        {
            if (obj == null || obj.gameObject == null) return;

            lock (_poolLock)
            {
                if (_activeObjects.TryGetValue(obj, out var poolInfo))
                {
                    var pools = poolInfo.is3D ? _3dPools : _uiPools;
                    if (pools.TryGetValue(poolInfo.type, out var pool))
                    {
                        pool.Release(obj);
                    }
                }
            }
        }

        #endregion

        #region Reactive Events

        private void RegisterREvents()
        {
            RBuss.OnEvent<Move3DToUIWithPhysicGlobalREvent>()
                .TakeUntilDisable(gameObject)
                .Subscribe(HandleMove3DToUIWithPhysic);

            RBuss.OnEvent<StockSpawnerREvents.SetDefaultSpawnTargetREvent>()
                .Subscribe(ev => SetDefaultStockMoveTarget(ev.StockType, ev.TargetRoot));

            RBuss.OnEvent<StockSpawnerREvents.SpawnStockREvent>()
                .Subscribe(HandleSpawnStock);

            RBuss.OnEvent<StockSpawnerREvents.DeSpawnStockREvent>()
                .Subscribe(HandleDeSpawnStock);
        }

        private void HandleMove3DToUIWithPhysic(Move3DToUIWithPhysicGlobalREvent ev)
        {
            StartCoroutine(Move3DToUIRoutineWithPhysicCustom(
                ev.CustomStock,
                ev.TargetRoot,
                ev.FinishedAction,
                ev.DownScaleRate));
        }

        private void HandleSpawnStock(StockSpawnerREvents.SpawnStockREvent ev)
        {
            var targetRoot = StockMoveTargetDict.TryGetValue(ev.StockType, out var value) ? value : null;
            var clone = SpawnStock(ev.StockType, targetRoot, ev.FinishedAction, ev.Spawn3D);

            if (clone == null) return;

            clone.ChangeSprite(ev.CustomSprite);

            if (ev.HasStartPos)
            {
                clone.transform.position = ev.Spawn3D
                    ? ev.StartPos
                    : Camera.WorldToScreenPoint(ev.StartPos);
            }
        }

        private void HandleDeSpawnStock(StockSpawnerREvents.DeSpawnStockREvent ev)
        {
            var spawnParent = StockMoveTargetDict.TryGetValue(ev.StockType, out var value)
                ? value
                : transform;

            var clone = DeSpawnStock(
                ev.StockType,
                ev.TargetTransform,
                ev.FinishedAction,
                ev.Spawn3D,
                ev.IsLastParticle);

            if (clone != null)
            {
                clone.transform.position = spawnParent.position;
            }
        }

        public static void Move3DToUIWithPhysicGlobal(
            ICustomStock stockSpawnObject,
            Transform targetRoot,
            Action onFinishedAction = null,
            float downScaleRate = 0.5f)
        {
            RBuss.Publish(new Move3DToUIWithPhysicGlobalREvent(
                stockSpawnObject,
                targetRoot,
                onFinishedAction,
                downScaleRate));
        }

        public class Move3DToUIWithPhysicGlobalREvent : REvent
        {
            public ICustomStock CustomStock { get; }
            public Transform TargetRoot { get; }
            public Action FinishedAction { get; }
            public float DownScaleRate { get; }

            public Move3DToUIWithPhysicGlobalREvent(
                ICustomStock customStock,
                Transform targetRoot,
                Action onFinishedAction = null,
                float downScaleRate = 0.5f)
            {
                CustomStock = customStock;
                TargetRoot = targetRoot;
                FinishedAction = onFinishedAction;
                DownScaleRate = downScaleRate;
            }
        }

        #endregion

        #region Spawn/DeSpawn

        private StockSpawnObject SpawnStock(
            StockTypes targetType,
            Transform spawnParent,
            Action<StockSpawnObject> onFinishedAction = null,
            bool is3D = false)
        {
            var clone = GetFromPool(targetType, is3D);
            if (clone == null) return null;

            clone.Build(obj =>
            {
                onFinishedAction?.Invoke(obj);
                ReturnToPool(obj);
            });

            if (is3D)
            {
                HandleSpawn3D(clone, spawnParent);
            }
            else
            {
                clone.transform.SetParent(movingCanvas.transform);
                MoveUIToUI(clone.transform, spawnParent, () => HandleSpawnComplete(clone));
            }

            return clone;
        }

        private void HandleSpawn3D(StockSpawnObject clone, Transform spawnParent)
        {
            if (clone.UsePhysic)
            {
                Move3DToUIWithPhysic(clone, spawnParent, () => HandleSpawnComplete(clone));
            }
            else
            {
                Move3DToUI(clone.transform, spawnParent, () => HandleSpawnComplete(clone));
            }
        }

        private void HandleSpawnComplete(StockSpawnObject clone)
        {
            PlaySfx();
            clone.DestroyYourself();
        }

        private StockSpawnObject DeSpawnStock(
            StockTypes targetType,
            Transform spawnParent,
            Action<StockSpawnObject> onFinishedAction = null,
            bool is3D = false,
            bool isLastParticle = false)
        {
            var clone = GetFromPool(targetType, is3D);
            if (clone == null) return null;

            clone.Build(obj =>
            {
                onFinishedAction?.Invoke(obj);
                ReturnToPool(obj);
            });

            if (is3D)
            {
                Move3DToUI(clone.transform, spawnParent, () => HandleDeSpawnComplete(clone, onFinishedAction));
            }
            else
            {
                clone.transform.SetParent(movingCanvas.transform);
                MoveUIToUIDeSpawn(
                    clone.transform,
                    spawnParent,
                    () => HandleDeSpawnComplete(clone, onFinishedAction),
                    isLastParticle: isLastParticle);
            }

            return clone;
        }

        private void HandleDeSpawnComplete(StockSpawnObject clone, Action<StockSpawnObject> onFinishedAction)
        {
            PlaySfx();
            clone.DestroyYourself();
        }

        #endregion

        #region Common Methods

        private void PlaySfx()
        {
            if (_isSfxPlaying) return;

            _isSfxPlaying = true;
            _delayTween?.Kill();
            _delayTween = DOVirtual.DelayedCall(0.5f, () => _isSfxPlaying = false, false);
        }

        private void SetDefaultStockMoveTarget(StockTypes targetType, Transform targetRoot)
        {
            StockMoveTargetDict[targetType] = targetRoot;
        }

        #endregion

        #region Movement Coroutines

        private void MoveUIToUI(
            Transform movingRoot,
            Transform targetRoot,
            Action onFinishedAction = null,
            float downScaleRate = 0.5f)
        {
            if (ShouldSkipMovement())
            {
                onFinishedAction?.Invoke();
                return;
            }

            StartCoroutine(MoveUIToUIRoutine(movingRoot, targetRoot, onFinishedAction, downScaleRate));
        }

        private void MoveUIToUIDeSpawn(
            Transform movingRoot,
            Transform targetRoot,
            Action onFinishedAction = null,
            float downScaleRate = 0.5f,
            bool isLastParticle = false)
        {
            if (ShouldSkipMovement(isLastParticle))
            {
                onFinishedAction?.Invoke();
                return;
            }

            StartCoroutine(MoveUIToUIRoutineDeSpawn(
                movingRoot,
                targetRoot,
                onFinishedAction,
                downScaleRate,
                isLastParticle));
        }

        private void Move3DToUI(
            Transform movingRoot,
            Transform targetRoot,
            Action onFinishedAction = null,
            float downScaleRate = 0.5f)
        {
            if (ShouldSkipMovement())
            {
                onFinishedAction?.Invoke();
                return;
            }

            StartCoroutine(Move3DToUIRoutine(movingRoot, targetRoot, onFinishedAction, downScaleRate));
        }

        private void Move3DToUIWithPhysic(
            StockSpawnObject stockSpawnObject,
            Transform targetRoot,
            Action onFinishedAction = null,
            float downScaleRate = 0.5f)
        {
            if (ShouldSkipMovement())
            {
                onFinishedAction?.Invoke();
                return;
            }

            StartCoroutine(Move3DToUIRoutineWithPhysic(
                stockSpawnObject,
                targetRoot,
                onFinishedAction,
                downScaleRate));
        }

        private bool ShouldSkipMovement(bool isLastParticle = false)
        {
            return _movingObjectCount >= MaxSpawnCount && !isLastParticle;
        }

        private IEnumerator MoveUIToUIRoutine(
            Transform movingRoot,
            Transform targetRoot,
            Action onFinishedAction,
            float downScaleRate)
        {
            if (!movingRoot) yield break;

            movingRoot.SetParent(targetRoot);
            _movingObjectCount++;

            var velocity = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f);

            yield return DelayPhase(movingRoot, velocity, Random.Range(0.1f, 0.2f));
            yield return MovePhase(movingRoot, 0.3f, downScaleRate);

            _movingObjectCount = Mathf.Max(0, _movingObjectCount - 1);
            onFinishedAction?.Invoke();
        }

        private IEnumerator MoveUIToUIRoutineDeSpawn(
            Transform movingRoot,
            Transform targetRoot,
            Action onFinishedAction,
            float downScaleRate,
            bool isLastParticle)
        {
            if (!movingRoot) yield break;

            movingRoot.SetParent(targetRoot);
            _movingObjectCount++;

            var velocity = new Vector3(
                Random.Range(-1f, 1f) * 2f,
                Random.Range(-2f, 2f) * 2f,
                0f);

            var delay = isLastParticle ? 0.65f : Random.Range(0.2f, 0.5f);
            yield return DelayPhaseDeSpawn(movingRoot, velocity, delay, isLastParticle);
            yield return MovePhase(movingRoot, 0.3f, downScaleRate);

            _movingObjectCount = Mathf.Max(0, _movingObjectCount - 1);
            onFinishedAction?.Invoke();
        }

        private IEnumerator DelayPhase(Transform movingRoot, Vector3 velocity, float delay)
        {
            var timer = 0f;
            var initScale = movingRoot.localScale;

            while (timer < delay && movingRoot)
            {
                timer += Time.deltaTime;
                velocity.y += Time.deltaTime * 5.5f;
                movingRoot.localPosition += new Vector3(velocity.x, velocity.y, 0);
                movingRoot.localScale = Vector3.Lerp(initScale, Vector3.one * 1.5f, timer / delay);

                if (ShouldSkipMovement()) yield break;
                yield return null;
            }
        }

        private IEnumerator DelayPhaseDeSpawn(
            Transform movingRoot,
            Vector3 velocity,
            float delay,
            bool isLastParticle)
        {
            var timer = 0f;
            var initScale = movingRoot.localScale;

            while (timer < delay && movingRoot)
            {
                timer += Time.deltaTime;
                velocity.y -= Time.deltaTime * 2.5f;
                movingRoot.localPosition += new Vector3(velocity.x, velocity.y, 0);
                movingRoot.localScale = Vector3.Lerp(initScale, Vector3.one, timer / delay);

                if (ShouldSkipMovement(isLastParticle)) yield break;
                yield return null;
            }
        }

        private IEnumerator MovePhase(Transform movingRoot, float duration, float downScaleRate)
        {
            if (!movingRoot) yield break;

            var timer = 0f;
            var initPos = movingRoot.localPosition;
            var initScale = movingRoot.localScale;

            while (timer < duration && movingRoot)
            {
                timer += Time.deltaTime;
                var t = Mathf.InverseLerp(0, duration, timer);

                movingRoot.localPosition = Vector3.Lerp(initPos, Vector3.zero, t);
                movingRoot.localScale = Vector3.Lerp(initScale, downScaleRate * Vector3.one, t);

                if (ShouldSkipMovement()) yield break;
                yield return null;
            }
        }

        private IEnumerator Move3DToUIRoutine(
            Transform movingRoot,
            Transform targetRoot,
            Action onFinishedAction,
            float downScaleRate)
        {
            yield return null;
            if (!movingRoot) yield break;

            var (targetPos, parent) = Get3DMovePos(targetRoot);
            movingRoot.SetParent(parent);

            var initWorldPos = movingRoot.position;
            var initScale = movingRoot.localScale;
            _movingObjectCount++;

            var velocity = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.2f, 0.7f),
                Random.Range(-1f, 1f));

            yield return Delay3DPhase(movingRoot, initWorldPos, velocity, Random.Range(1f, 2f), initScale);
            yield return Move3DPhase(movingRoot, targetPos, Random.Range(0.5f, 1f), downScaleRate);

            _movingObjectCount = Mathf.Max(0, _movingObjectCount - 1);
            onFinishedAction?.Invoke();
        }

        private IEnumerator Move3DToUIRoutineWithPhysic(
            StockSpawnObject stockSpawnObject,
            Transform targetRoot,
            Action onFinishedAction,
            float downScaleRate)
        {
            yield return null;
            if (!stockSpawnObject) yield break;

            var (targetPos, parent) = Get3DMovePos(targetRoot);
            _movingObjectCount++;

            yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));

            DisablePhysics(stockSpawnObject);
            stockSpawnObject.transform.SetParent(parent);

            yield return Move3DPhase(
                stockSpawnObject.transform,
                targetPos,
                Random.Range(0.25f, 0.5f),
                downScaleRate);

            _movingObjectCount = Mathf.Max(0, _movingObjectCount - 1);
            onFinishedAction?.Invoke();
        }

        private IEnumerator Move3DToUIRoutineWithPhysicCustom(
            ICustomStock customStock,
            Transform targetRoot,
            Action onFinishedAction,
            float downScaleRate)
        {
            yield return null;

            var customStockTransform = customStock.GetTransform();
            if (!customStockTransform) yield break;

            customStockTransform.SetParent(CameraTransform);
            DisablePhysics(customStock);

            yield return new WaitForSeconds(Random.Range(0f, 0.02f));

            var initPos = customStockTransform.position;
            var initScale = customStockTransform.localScale;
            var duration = Random.Range(0.5f, 1f);
            var timer = 0f;

            while (timer < duration && customStockTransform)
            {
                timer += Time.deltaTime;
                var targetPos = Get3DMovePos(targetRoot).Item1;
                var t = EaseHelper.EaseInSine(timer, duration);

                customStockTransform.localPosition = NMath.SmoothCurveY(initPos, targetPos, 1f, t);
                customStockTransform.localScale = Vector3.Lerp(initScale, downScaleRate * initScale, t);

                yield return null;
            }

            if (customStockTransform)
                customStock.ActivateDisable();

            onFinishedAction?.Invoke();
        }

        private IEnumerator Delay3DPhase(
            Transform movingRoot,
            Vector3 initWorldPos,
            Vector3 velocity,
            float duration,
            Vector3 initScale)
        {
            var timer = 0f;

            while (timer < duration && movingRoot)
            {
                timer += Time.deltaTime;
                velocity += new Vector3(
                    Time.deltaTime,
                    -Time.deltaTime,
                    Time.deltaTime);

                movingRoot.position = NMath.SmoothCurveY(
                    initWorldPos,
                    initWorldPos + velocity,
                    1f,
                    EaseHelper.EaseInSine(timer, duration));

                movingRoot.localScale = Vector3.Lerp(
                    initScale,
                    Vector3.one * 1.5f,
                    timer / duration);

                if (ShouldSkipMovement()) yield break;
                yield return null;
            }
        }

        private IEnumerator Move3DPhase(
            Transform movingRoot,
            Vector3 targetPos,
            float duration,
            float downScaleRate)
        {
            if (!movingRoot) yield break;

            var timer = 0f;
            var initPos = movingRoot.localPosition;
            var initScale = movingRoot.localScale;

            while (timer < duration && movingRoot)
            {
                timer += Time.deltaTime;
                var t = EaseHelper.EaseInSine(timer, duration);

                movingRoot.localPosition = NMath.SmoothCurveY(initPos, targetPos, 1f, t);
                movingRoot.localScale = Vector3.Lerp(initScale, downScaleRate * Vector3.one, t);

                if (ShouldSkipMovement()) yield break;
                yield return null;
            }
        }

        private void DisablePhysics(StockSpawnObject stockSpawnObject)
        {
            if (stockSpawnObject.Rb) stockSpawnObject.Rb.isKinematic = true;
            if (stockSpawnObject.Col) stockSpawnObject.Col.enabled = false;
        }

        private void DisablePhysics(ICustomStock customStock)
        {
            var rb = customStock.GetRigidBody();
            var col = customStock.GetCollider();

            if (rb) rb.isKinematic = true;
            if (col) col.enabled = false;
        }

        private (Vector3, Transform) Get3DMovePos(Transform target)
        {
            if (!Camera) return (Vector3.zero, null);

            var ray = Camera.ScreenPointToRay(target.position);
            var point = ray.origin + ray.direction * 5f;

            if (CameraTransform)
                point = CameraTransform.InverseTransformPoint(point);

            return (point, CameraTransform);
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        [Button, TabGroup("Tabs", "Examples")]
        private void SpawnStockEditor(StockTypes stockType, int count = 3, bool useRandomTypes = false)
        {
            for (int i = 0; i < count; i++)
            {
                var targetType = GetStockType(stockType, useRandomTypes);
                StockStatic.SpawnStock(targetType, 1);
            }
        }

        [Button, TabGroup("Tabs", "Examples")]
        private void SpawnStockEditorVector(
            StockTypes stockType,
            Vector3 startVector,
            int count = 3,
            bool useRandomTypes = false)
        {
            for (int i = 0; i < count; i++)
            {
                var targetType = GetStockType(stockType, useRandomTypes);
                StockStatic.SpawnStock(targetType, startVector, 1);
            }
        }

        [Button, TabGroup("Tabs", "Examples")]
        private void SpawnStockEditor3D(StockTypes stockType, int count = 3, bool useRandomTypes = false)
        {
            for (int i = 0; i < count; i++)
            {
                var targetType = GetStockType(stockType, useRandomTypes);
                StockStatic.SpawnStock3D(targetType, 1);
            }
        }

        [Button, TabGroup("Tabs", "Examples")]
        private void SpawnStockEditorVector3D(
            StockTypes stockType,
            Vector3 startVector,
            int count = 3,
            bool useRandomTypes = false)
        {
            for (int i = 0; i < count; i++)
            {
                var targetType = GetStockType(stockType, useRandomTypes);
                StockStatic.SpawnStock3D(targetType, startVector, 1);
            }
        }

        private StockTypes GetStockType(StockTypes stockType, bool useRandomTypes)
        {
            if (!useRandomTypes) return stockType;

            var enumNames = Enum.GetNames(typeof(StockTypes));
            return (StockTypes)Random.Range(0, enumNames.Length);
        }
#endif

        #endregion
    }
}