using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class SwitchMove : MonoBehaviour
{
    [SerializeField]
    Controller RightController;
    [SerializeField]
    Toggle switchMoveToggle;

    public IObservable<bool> IsSwitchingAsObservable()
    {
        return switchMoveToggle.OnValueChangedAsObservable().CombineLatest(RightController.MainButtonPressed, (s, m) => s && m);
    }
}
