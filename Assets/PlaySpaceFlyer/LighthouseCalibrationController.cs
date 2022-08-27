using UnityEngine;
using UnityEngine.UI;

public class QuestCalibrationController : MonoBehaviour
{
    [SerializeField] Toggle questCalibrationToggle;

    [SerializeField] InputEmulator InputEmulator;
    [SerializeField] Transform Target;

    void LateUpdate()
    {
        if (!questCalibrationToggle.isOn) return;
        var transform = Target.transform;
        var position = transform.position;
        var rotation = transform.rotation;
        InputEmulator.SetLighthouseOffset(position, rotation);
    }
}
