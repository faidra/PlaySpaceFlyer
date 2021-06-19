using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class VRCMoving : MonoBehaviour
{
    [SerializeField]
    Controller LeftController;

    public IObservable<bool> IsMovingAsObservable()
    {
        return LeftController.Stick.Select(v => v.sqrMagnitude > 0f).DistinctUntilChanged();
    }
}
