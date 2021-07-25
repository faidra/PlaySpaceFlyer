using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public sealed class PoseVisualizer : MonoBehaviour
{
    public readonly struct Param
    {
        public readonly bool isActive;
        public readonly Vector3 position;
        public readonly Quaternion rotation;
        public readonly Vector3 scale;

        public Param(bool isActive, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.isActive = isActive;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }

    public static PoseVisualizer Create(MonoBehaviour source, Func<Param> func)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = source.name;
        var i = go.AddComponent<PoseVisualizer>();
        i.Initialize(source, func);
        return i;
    }

    MonoBehaviour source;
    Func<Param> func;

    void Initialize(MonoBehaviour source, Func<PoseVisualizer.Param> func)
    {
        this.source = source;
        this.func = func;
    }

    void Start()
    {
        source.OnDestroyAsObservable().Subscribe(_ => Destroy(gameObject)).AddTo(this);
        Observable.EveryUpdate().Subscribe(_ =>
        {
            var param = func();
            gameObject.SetActive(param.isActive);
            if (!param.isActive)
            {
                return;
            }

            var transform = this.transform;
            transform.position = param.position;
            transform.rotation = param.rotation;
            transform.localScale = param.scale;
        }).AddTo(this);
    }
}