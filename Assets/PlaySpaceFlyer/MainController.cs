using UnityEngine;
using UniRx;

public class MainController : MonoBehaviour
{
    [SerializeField]
    VRCMoving Moving;

    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform Target;

    [SerializeField]
    DoubleGrip P;
    [SerializeField]
    HMD HMD;

    bool isMoving;

    void Start()
    {
        Moving.IsMovingAsObservable().Subscribe(m => isMoving = m).AddTo(this);
        P.IsDoubleGrabbingAsObservable()
            .Where(on => on)
            .Subscribe(on => InputEmulator.SetAllDeviceWorldRotOffset(Quaternion.Euler(0, Random.Range(0, 360), 0), HMD.Position))
            .AddTo(this);
    }

    void Update()
    {
        var offset = isMoving ? Vector3.zero : Target.transform.position;
        //InputEmulator.SetAllDeviceWorldPosOffset(offset);
        Debug.LogError("HMD:"+HMD.Position);
    }
}
