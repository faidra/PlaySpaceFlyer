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

    bool isMoving;

    void Start()
    {
        Moving.IsMovingAsObservable().Subscribe(m => isMoving = m).AddTo(this);
    }

    void Update()
    {
        var offset = isMoving ? Vector3.zero : Target.transform.position;
        InputEmulator.SetAllDeviceWorldPosOffset(offset);
    }
}
