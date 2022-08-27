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

    void Start()
    {
        drag.MoveAsObservable()
            .Where(_ => lighthouseCalibrationToggle.isOn)
            .Subscribe(AddOffset).AddTo(this);

        resetEvent.OnResetAsObservable()
            .Where(_ => lighthouseCalibrationToggle.isOn)
            .Subscribe(_ =>
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            })
            .AddTo(this);
    }

    bool isFirst;
    void Update()
    {
        if (isFirst)
        {
            LoadOffset();
            isFirst = true;
        }
    }

    void LoadOffset()
    {
        transform.localPosition = lighthouseOffset.CurrentOffset;
        transform.localRotation = lighthouseOffset.CurrentRotation;
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
