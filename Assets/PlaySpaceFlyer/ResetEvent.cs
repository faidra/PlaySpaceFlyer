using UnityEngine;
using UniRx;
using System;
using System.Linq;

public class ResetEvent : MonoBehaviour
{
    [SerializeField]
    Controller[] Controllers;

    readonly Subject<Unit> _onReset = new Subject<Unit>();

    public IObservable<Unit> OnResetAsObservable()
    {
        return _onReset;
    }

    void Start()
    {
        Controllers.Select(c => c.GripPressed)
            .CombineLatestValuesAreAllTrue()
            .Where(on => on)
            .Subscribe(_ => _onReset.OnNext(Unit.Default))
            .AddTo(this);
    }
}
