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
    float SpeedMultiplier;
    [SerializeField]
    InputSimulator InputSimulator;

    Vector3 currentOffset;

    void Start()
    {
        Draggs.Select(d => d.MoveAsObservable()).Merge().Subscribe(AddOffset).AddTo(this);

        Observable.CombineLatest(Left.GripPressed, Right.GripPressed, (l, r) => l && r)
            .Where(on => on)
            .Subscribe(_ => currentOffset = Vector3.zero)
            .AddTo(this);
    }

    void Update()
    {
        var offset = Left.PadPressed.Value ? Vector3.zero : currentOffset;
        InputSimulator.SetAllDeviceWorldPosOffset(offset);
    }

    void AddOffset(Vector3 grab)
    {
        currentOffset += Vector3.up * grab.y * SpeedMultiplier * Time.deltaTime;
    }
}
