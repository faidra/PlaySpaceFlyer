using System;
using Valve.VR;
using UniRx;
using UnityEngine;

public static class SteamVRInputSourcesExtensions
{
    public static IObservable<bool> GetStateAsObservable(this SteamVR_Input_Sources inputSource, SteamVR_Action_Boolean action)
    {
        return action.ObserveEveryValueChanged(a => a.GetState(inputSource));
    }

    public static IObservable<bool> GetDoublePressAsObservable(this SteamVR_Input_Sources inputSource, SteamVR_Action_Boolean action, float graceSeconds)
    {
        return Observable.Create<bool>(observer =>
        {
            var state = inputSource.GetStateAsObservable(action).ToReadOnlyReactiveProperty();
            var isSeriesUntil = 0f;
            return StableCompositeDisposable.Create(
                state,
                state.Select(pressed => pressed && Time.time < isSeriesUntil).DistinctUntilChanged().Subscribe(observer),
                state.Where(pressed => pressed).Subscribe(_ => isSeriesUntil = Time.time + graceSeconds));
        });
    }
}
