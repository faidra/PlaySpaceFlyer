using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class LighthouseCalibrationController : MonoBehaviour
{
    [SerializeField] Toggle lighthouseCalibrationToggle;

    [SerializeField] InputEmulator InputEmulator;
    [SerializeField] Transform Target;

    [SerializeField] Controller shortcutController;
    [SerializeField] SteamVR_Action_Boolean firstAction;
    [SerializeField] SteamVR_Action_Boolean secondAction;
    [SerializeField] float graceSeconds;

    [SerializeField] Controller otherController;
    [SerializeField] HMD hmd;
    [SerializeField] LighthouseOffset lighthouseOffset;

    void Start()
    {
        shortcutController.InputSources.GetDoublePressAsObservable(firstAction, graceSeconds)
            .Select(d => d ? shortcutController.InputSources.GetDoublePressAsObservable(secondAction, graceSeconds).Where(d2 => d2).AsUnitObservable() : Observable.Empty<Unit>())
            .Subscribe(_ => lighthouseCalibrationToggle.isOn = !lighthouseCalibrationToggle.isOn)
            .AddTo(this);

        lighthouseCalibrationToggle.OnValueChangedAsObservable()
            .Where(on => on)
            .Subscribe(_ => InitializePosition())
            .AddTo(this);
    }

    void InitializePosition()
    {
        var targetPosition = hmd.Position + hmd.Rotation * (0.5f * Vector3.forward);
        var currentRotation = Quaternion.Slerp(shortcutController.Rotation, otherController.Rotation, 0.5f);
        var rotationOffset = Quaternion.Euler(0f, (Quaternion.Inverse(currentRotation) * hmd.Rotation).eulerAngles.y, 0f);
        var currentPosition = rotationOffset * Vector3.Lerp(shortcutController.Position, otherController.Position, 0.5f);
        var positionOffset = targetPosition - currentPosition;
        lighthouseOffset.SetValues(positionOffset, rotationOffset);
    }

    void LateUpdate()
    {
        if (!lighthouseCalibrationToggle.isOn) return;
        var transform = Target.transform;
        var position = transform.position;
        var rotation = transform.rotation;
        InputEmulator.SetLighthouseOffset(position, rotation);
    }
}
