using UnityEngine;
using System.Linq;
using UniRx;

public class FloatAnimationModule : MonoBehaviour
{
    [SerializeField]
    DoubleGrip[] Grips;
    [SerializeField]
    VRCMoving Moving;

    [SerializeField]
    Animator Animator;
    [SerializeField]
    string ParameterName;

    void Start()
    {
        var toggle = Grips.Select(g => g.IsDoubleGrabbingAsObservable())
            .CombineLatestValuesAreAllTrue()
            .Where(on => on)
            .Scan((a, b) => !a);

        Observable.CombineLatest(toggle, Moving.IsMovingAsObservable(), (on, off) => on && !off)
            .Subscribe(on => Animator.SetBool(ParameterName, on))
            .AddTo(this);
    }
}
