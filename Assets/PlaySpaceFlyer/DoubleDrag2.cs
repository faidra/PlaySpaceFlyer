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
    [SerializeField] InputEmulator inputEmulator;

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
        return Observable.Defer(() =>
        {
            var grabbedAt = inputEmulator.GetRealPosition(controller.Position);
            return this.UpdateAsObservable().Select(_ => inputEmulator.GetRealPosition(controller.Position) - grabbedAt);
        });
    }
}
