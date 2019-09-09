using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    [SerializeField]
    VRCMoving Moving;

    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform Target;

    [SerializeField]
    InputField ReferenceBaseStationId;

    bool isMoving;

    void Start()
    {
        ReferenceBaseStationId.OnValueChangedAsObservable()
            .Select(uint.Parse)
            .Subscribe(InputEmulator.SetReferenceBaseStation)
            .AddTo(this);

        Moving.IsMovingAsObservable().Subscribe(m => isMoving = m).AddTo(this);
    }

    void Update()
    {
        var position = Target.transform.position;
        var rotation = Target.transform.rotation;
        if (isMoving)
        {
            position.y = 0;
            rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
        }
        InputEmulator.SetAllDeviceWorldRotOffset(rotation);
        InputEmulator.SetAllDeviceWorldPosOffset(position);
    }
}
