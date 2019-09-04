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
        Grips.Select(g => g.IsDoubleGrabbingAsObservable())
            .CombineLatestValuesAreAllTrue()
            .Where(on => on)
            .Subscribe(_ => Toggle())
            .AddTo(this);
    }

    void Toggle()
    {
        Animator.SetBool(ParameterName, !Animator.GetBool(ParameterName));
    }
}
