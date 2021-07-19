using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class DoubleDrag : MonoBehaviour
{
    [SerializeField]
    Controller Controller;
    [SerializeField]
    float GraceSeconds;
    [SerializeField] PoseReceiver poseReceiver;

    Vector3 DragStartPosition;

    public IObservable<Vector3> MoveAsObservable()
    {
        return IsDoubleDraggingAsObservable()
            .Select(dragging => dragging ? DragAsObservable() : Observable.Empty<Vector3>())
            .Switch();
    }

    public IObservable<IObservable<Vector3>> MovesAsObservable()
    {
        return IsDoubleDraggingAsObservable()
            .Where(on => on)
            .Select(_ => DragAsObservable().TakeUntil(Controller.MainButtonPressed.Where(on => !on)));
    }

    IObservable<bool> IsDoubleDraggingAsObservable()
    {
        return Observable.Create<bool>(observer =>
        {
            var isSeriesUntil = 0f;
            return StableCompositeDisposable.Create(
            Controller.MainButtonPressed.Select(pressed => pressed && Time.time < isSeriesUntil).DistinctUntilChanged().Subscribe(observer),
            Controller.MainButtonPressed.Where(pressed => pressed).Subscribe(_ => isSeriesUntil = Time.time + GraceSeconds));
        });
    }

    IObservable<Vector3> DragAsObservable()
    {
        // 最初の位置から、毎フレーム合計の移動量を返す
        return poseReceiver.OnPoseUpdatedAsObservable(Controller.InputSources).FirstOrDefault()
            .ContinueWith(p => this.UpdateAsObservable()
                .WithLatestFrom(poseReceiver.OnPoseUpdatedAsObservable(Controller.InputSources),
                    (_, cp) => cp.position - p.position));
    }
}
