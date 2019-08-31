using UnityEngine;
using System.Linq;
using UniRx;

public class MainController : MonoBehaviour
{
    [SerializeField]
    DoubleDrag[] Draggs;
    [SerializeField]
    Controller Left;
    [SerializeField]
    Controller Right;
    [SerializeField]
    DoubleGrip[] Grips;

    [SerializeField]
    float SpeedMultiplier;
    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform AnimatedObject;

    Vector3 currentDragOffset;
    bool useAnimation;

    void Start()
    {
        Draggs.Select(d => d.MoveAsObservable()).Merge().Subscribe(AddOffset).AddTo(this);

        Observable.CombineLatest(Left.GripPressed, Right.GripPressed, (l, r) => l && r)
            .Where(on => on)
            .Subscribe(_ => currentDragOffset = Vector3.zero)
            .AddTo(this);

        Observable.CombineLatest(Grips.Select(g => g.IsDoubleGrabbingAsObservable()))
            .Where(grabings => grabings.All(on => on))
            .Subscribe(_ => useAnimation = !useAnimation)
            .AddTo(this);
    }

    void Update()
    {
        var offset = Left.PadPressed.Value ? Vector3.zero : useAnimation ? currentDragOffset + AnimatedObject.transform.localPosition : currentDragOffset;
        InputEmulator.SetAllDeviceWorldPosOffset(offset);
    }

    void AddOffset(Vector3 grab)
    {
        currentDragOffset += Vector3.up * grab.y * SpeedMultiplier * Time.deltaTime;
    }
}
