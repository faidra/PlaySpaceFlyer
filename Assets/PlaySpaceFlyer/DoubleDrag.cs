﻿using UnityEngine;
using Valve.VR;
using System.Runtime.InteropServices;
using UniRx;
using UniRx.Triggers;
using System;

public class DoubleDrag : MonoBehaviour
{
    [SerializeField]
    Controller Controller;
    [SerializeField]
    float GraceSeconds;

    Vector3 DragStartPosition;

    public IObservable<Vector3> MoveAsObservable()
    {
        return IsDoubleDraggingAsObservable()
            .Select(dragging => dragging ? DragAsObservable() : Observable.Empty<Vector3>())
            .Switch();
    }

    IObservable<bool> IsDoubleDraggingAsObservable()
    {
        return Observable.Create<bool>(observer =>
        {
            var isSeriesUntil = 0f;
            return StableCompositeDisposable.Create(
            Controller.MenuPressed.Select(pressed => pressed && Time.time < isSeriesUntil).DistinctUntilChanged().Subscribe(observer),
            Controller.MenuPressed.Where(pressed => pressed).Subscribe(_ => isSeriesUntil = Time.time + GraceSeconds));
        });
    }

    IObservable<Vector3> DragAsObservable()
    {
        return Observable.Defer(() =>
        {
            var grabbedAt = Controller.Position;
            return this.UpdateAsObservable().Select(_ => Controller.Position - grabbedAt);
        });
    }
}
