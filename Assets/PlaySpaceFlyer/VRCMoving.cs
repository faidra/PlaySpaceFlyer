using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class VRCMoving : MonoBehaviour
{
    [SerializeField]
    Controller LeftController;
    [SerializeField]
    float SafetySeconds;
    [SerializeField]
    Toggle vrcModeToggle;

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
            .CombineLatest(vrcModeToggle.OnValueChangedAsObservable(), (move, vrc) => move && vrc)
            .DistinctUntilChanged();
    }
}
