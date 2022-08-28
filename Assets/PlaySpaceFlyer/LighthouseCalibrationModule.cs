using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class LighthouseCalibrationModule : MonoBehaviour
{
    [SerializeField] Toggle lighthouseCalibrationToggle;
    
    [SerializeField]
    Controller controller;
    [SerializeField]
    ResetEvent resetEvent;

    [SerializeField] Vector3 defaultCalibrationOffset;

    [SerializeField] Controller otherController;
    [SerializeField] HMD hmd;

    [SerializeField] Vector3 controllerForward;
    [SerializeField] float rotationMultiplier;

    bool prev;
    Vector3 prevPos;
    Quaternion prevRot;

    void Start()
    {
        resetEvent.OnResetAsObservable()
            .Where(_ => lighthouseCalibrationToggle.isOn)
            .Subscribe(_ => InitializePosition())
            .AddTo(this);
    }

    void Update()
    {
        var v = lighthouseCalibrationToggle.isOn && controller.MainButtonPressed.Value;
        if (v)
        {
            var pos = controller.Position;
            var rot = controller.Rotation;
            var transform = this.transform;
            if (prev)
            {
                var offset = pos - prevPos;
                var offsetRot = rot * Quaternion.Inverse(prevRot);
                offsetRot = Quaternion.Euler(0f, offsetRot.eulerAngles.y, 0f);
                var rotate = Quaternion.Slerp(Quaternion.identity, offsetRot, rotationMultiplier);
                var crot = transform.localRotation;
                var cpos = transform.localPosition;
                var center = cpos + pos;
                transform.localRotation = crot * rotate;
                transform.localPosition = cpos -(rotate * center - center) + offset;
            }

            prevPos = pos;
            prevRot = rot;
            prev = true;
        }
        else
        {
            prev = false;
        }
    }

    void Set(Vector3 pos, Quaternion rot)
    {
        transform.localPosition = pos;
        transform.localRotation = rot;
    }
    
    void InitializePosition()
    {
        var forward = AsY(hmd.Rotation);
        var targetPosition = hmd.Position + forward * defaultCalibrationOffset;
        var rightForward = controller.Rotation * controllerForward;
        var leftForward = otherController.Rotation * controllerForward;
        var currentForward = Vector3.Lerp(rightForward, leftForward, 0.5f);
        var currentRotation = AsY(Quaternion.LookRotation(currentForward));
        var rotationOffset = forward * Quaternion.Inverse(currentRotation);
        var currentPosition = rotationOffset * Vector3.Lerp(controller.Position, otherController.Position, 0.5f);
        var positionOffset = Quaternion.Inverse(rotationOffset) * (targetPosition - currentPosition);
        Set(positionOffset, rotationOffset);
    }

    static Quaternion AsY(Quaternion q)
    {
        return Quaternion.Euler(0f, q.eulerAngles.y, 0f);
    }
}
