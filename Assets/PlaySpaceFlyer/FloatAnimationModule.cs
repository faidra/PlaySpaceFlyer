using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.UI;

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
    [SerializeField]
    string WeightParameterName;
    [SerializeField]
    string speedParameterName;

    [SerializeField]
    Slider weightSlider;
    [SerializeField]
    Slider speedSlider;

    void Start()
    {
        var toggle = Grips.Select(g => g.IsDoubleGrabbingAsObservable())
            .CombineLatestValuesAreAllTrue()
            .Where(on => on)
            .Scan((a, b) => !a);

        toggle.Subscribe(on => Animator.SetBool(ParameterName, on))
            .AddTo(this);

        weightSlider.OnValueChangedAsObservable().Subscribe(SetWeight).AddTo(this);
        speedSlider.OnValueChangedAsObservable()
            .CombineLatest(Moving.IsMovingAsObservable(), (speed, isMoving) => isMoving ? 0f : speed)
            .Subscribe(SetSpeed).AddTo(this);
    }

    void SetWeight(float weight)
    {
        Animator.SetFloat(WeightParameterName, weight);
    }

    void SetSpeed(float speed)
    {
        Animator.SetFloat(speedParameterName, speed);
    }
}
