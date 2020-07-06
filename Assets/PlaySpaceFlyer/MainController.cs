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

    void Update()
    {
        var position = Target.transform.position;
        var rotation = Target.transform.rotation;
        InputEmulator.SetAllDeviceWorldRotOffset(rotation);
        InputEmulator.SetAllDeviceWorldPosOffset(position);
    }
}
