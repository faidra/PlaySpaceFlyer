using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class LighthouseCalibrationModule : MonoBehaviour
{
    [SerializeField] Toggle lighthouseCalibrationToggle;
    
    [SerializeField]
    DoubleDrag drag;
    [SerializeField]
    Controller controller;
    [SerializeField]
    ResetEvent resetEvent;

    [SerializeField]
    float positionSpeedMultiplier;
    [SerializeField]
    float rotationSpeedMultiplier;

    [SerializeField] LighthouseOffset lighthouseOffset;

    bool prev;
    Vector3 prevPos;
    Quaternion prevRot;

    void Start()
    {
        resetEvent.OnResetAsObservable()
            .Where(_ => lighthouseCalibrationToggle.isOn)
            .Subscribe(_ => Set(Vector3.zero, Quaternion.identity))
            .AddTo(this);
    }

    void Update()
    {
        var v = lighthouseCalibrationToggle.isOn && controller.MainButtonPressed.Value;
        if (v)
        {
            var pos = controller.Position;
            var rot = controller.Rotation;
            if (prev)
            {
                var offset = pos - prevPos;
                var offsetRot = Quaternion.Inverse(prevRot) * rot;
                offsetRot = Quaternion.Euler(0f, offsetRot.eulerAngles.y, 0f);
                transform.localPosition += offset;
                transform.localRotation *= offsetRot;
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

    public void Set(Vector3 pos, Quaternion rot)
    {
        transform.localPosition = pos;
        transform.localRotation = rot;
    }

    void AddOffset(Vector3 grab)
    {
        if (controller.ModifierPressed.Value)
        {
            var position = controller.Position;
            transform.RotateAround(position, Vector3.up, grab.y * rotationSpeedMultiplier * Time.deltaTime);
        }
        else
        {
            transform.Translate(grab * positionSpeedMultiplier * Time.deltaTime);
        }
    }
}
