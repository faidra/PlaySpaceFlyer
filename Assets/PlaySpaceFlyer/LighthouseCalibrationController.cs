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

    void Start()
    {
        shortcutController.InputSources.GetDoublePressAsObservable(firstAction, graceSeconds)
            .Select(d => d ? shortcutController.InputSources.GetDoublePressAsObservable(secondAction, graceSeconds).Where(d2 => d2).AsUnitObservable() : Observable.Empty<Unit>())
            .Switch()
            .Subscribe(_ => lighthouseCalibrationToggle.isOn = !lighthouseCalibrationToggle.isOn)
            .AddTo(this);
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
