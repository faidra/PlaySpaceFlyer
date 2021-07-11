using UnityEngine;
using UniRx;

public class MainController : MonoBehaviour
{
    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform Target;

    [SerializeField]
    TrackingReferenceSelector TrackingReferenceSeloctor;
    
    void Start()
    {
        TrackingReferenceSeloctor.SelectedTrackingReferenceIdAsObservable()
            .Subscribe(InputEmulator.SetReferenceBaseStation)
            .AddTo(this);
    }

    void LateUpdate()
    {
        var transform = Target.transform;
        var position = transform.position;
        var rotation = transform.rotation;
        InputEmulator.SetAllDeviceTransform(position, rotation);
    }
}
