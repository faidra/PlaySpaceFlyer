using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using Valve.VR;

public sealed class DoubleDrag2 : MonoBehaviour
{
    [SerializeField] Controller controller;
    [SerializeField] SteamVR_Action_Boolean actionBoolean;
    [SerializeField] float graceSeconds;
    [SerializeField] PoseReceiver poseReceiver;

    public IObservable<Vector3> MoveAsObservable()
    {
        return controller.InputSources.GetDoublePressAsObservable(actionBoolean, graceSeconds)
            .Select(dragging => dragging ? DragAsObservable() : Observable.Empty<Vector3>())
            .Switch();
    }

    public IObservable<IObservable<Vector3>> MovesAsObservable()
    {
        var onRelease = controller.InputSources.GetStateAsObservable(actionBoolean).Where(on => !on);
        return controller.InputSources.GetDoublePressAsObservable(actionBoolean, graceSeconds)
            .Where(on => on)
            .Select(_ => DragAsObservable().TakeUntil(onRelease));
    }


    IObservable<Vector3> DragAsObservable()
    {
        // 最初の位置から、毎フレーム合計の移動量を返す
        return poseReceiver.OnPoseUpdatedAsObservable(controller.InputSources).FirstOrDefault()
            .ContinueWith(p => this.UpdateAsObservable()
                .WithLatestFrom(poseReceiver.OnPoseUpdatedAsObservable(controller.InputSources),
                    (_, cp) => cp.pos - p.pos));
    }
}
