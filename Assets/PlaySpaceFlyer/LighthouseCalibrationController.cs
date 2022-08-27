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
    [SerializeField] Vector3 defaultCalibrationOffset;

    [SerializeField] Controller otherController;
    [SerializeField] HMD hmd;
    [SerializeField] LighthouseOffset lighthouseOffset;

    [SerializeField] LighthouseCalibrationModule calibrationModule;

    void Start()
    {
        shortcutController.InputSources.GetDoublePressAsObservable(firstAction, graceSeconds)
            .Select(d => d ? shortcutController.InputSources.GetDoublePressAsObservable(secondAction, graceSeconds).Where(d2 => d2).AsUnitObservable() : Observable.Empty<Unit>())
            .Switch()
            .Subscribe(_ => lighthouseCalibrationToggle.isOn = !lighthouseCalibrationToggle.isOn)
            .AddTo(this);

        lighthouseCalibrationToggle.OnValueChangedAsObservable()
            .Where(on => on)
            .Subscribe(_ => InitializePosition())
            .AddTo(this);
    }

    void InitializePosition()
    {
        var forward = AsY(hmd.Rotation);
        var targetPosition = hmd.Position + forward * defaultCalibrationOffset;
        var currentRotation = AsY(Quaternion.Slerp(shortcutController.Rotation, otherController.Rotation, 0.5f));
        var rotationOffset = forward * Quaternion.Inverse(currentRotation);
        var currentPosition = rotationOffset * Vector3.Lerp(shortcutController.Position, otherController.Position, 0.5f);
        var positionOffset = targetPosition - currentPosition;
        calibrationModule.Set(positionOffset, rotationOffset);
        InputEmulator.SetLighthouseOffset(positionOffset, rotationOffset);
    }

    static Quaternion AsY(Quaternion q)
    {
        return Quaternion.Euler(0f, q.eulerAngles.y, 0f);
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
