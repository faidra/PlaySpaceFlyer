using UnityEngine;
using UniRx;
using System;

public class VRCMoving : MonoBehaviour
{
    [SerializeField]
    Controller LeftController;
    [SerializeField]
    float SafetySeconds;

    public IObservable<bool> IsMovingAsObservable()
    {
        return LeftController.PadPressed
            .Where(pressed => pressed)
            .Select(_ =>
                LeftController.PadTouched
                    .FirstOrDefault(touched => !touched)
                    .Delay(TimeSpan.FromSeconds(SafetySeconds))
                    .StartWith(true))
            .Switch()
            .StartWith(false)
            .DistinctUntilChanged();
    }
}
