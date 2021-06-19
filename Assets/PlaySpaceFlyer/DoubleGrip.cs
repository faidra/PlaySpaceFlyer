using UnityEngine;
using UniRx;
using System;

public class DoubleGrip : MonoBehaviour
{
    [SerializeField]
    Controller Controller;
    [SerializeField]
    float GraceSeconds;

    public IObservable<bool> IsDoubleGrabbingAsObservable()
    {
        return Controller.MainButtonPressed
            .Timestamp()
            .Buffer(3, 1)
            .Select(buffered => buffered[2].Value && (buffered[2].Timestamp - buffered[0].Timestamp < TimeSpan.FromSeconds(GraceSeconds)))
            .StartWith(false)
            .DistinctUntilChanged();
    }
}
