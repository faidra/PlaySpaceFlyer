using UnityEngine;
using UniRx;
using System;
using System.Linq;
using UnityEngine.UI;

public class ResetEvent : MonoBehaviour
{
    [SerializeField]
    Controller[] Controllers;
    [SerializeField]
    Toggle resetEnabledToggle;

    readonly Subject<Unit> _onReset = new Subject<Unit>();

    public IObservable<Unit> OnResetAsObservable() => _onReset;

    void Start()
    {
        Controllers.Select(c => c.CancellerPressed)
            .CombineLatestValuesAreAllTrue()
            .Where(on => on && resetEnabledToggle.isOn)
            .Subscribe(_ => _onReset.OnNext(Unit.Default))
            .AddTo(this);
    }
}
