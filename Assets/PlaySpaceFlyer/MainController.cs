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
    VRCMoving Moving;

    [SerializeField]
    float SpeedMultiplier;
    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform AnimatedObject;

    Vector3 currentDragOffset;
    bool useAnimation;
    bool isMoving;

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

        Moving.IsMovingAsObservable().Subscribe(m => isMoving = m).AddTo(this);
    }

    void Update()
    {
        var offset = isMoving ? Vector3.zero : useAnimation ? currentDragOffset + AnimatedObject.transform.localPosition : currentDragOffset;
        InputEmulator.SetAllDeviceWorldPosOffset(offset);
    }

    void AddOffset(Vector3 grab)
    {
        currentDragOffset += Vector3.up * grab.y * SpeedMultiplier * Time.deltaTime;
    }
}
