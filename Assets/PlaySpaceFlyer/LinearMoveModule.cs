using UnityEngine;
using UniRx;

public class LinearMoveModule : MonoBehaviour
{
    [SerializeField]
    DoubleDrag Drag;
    [SerializeField]
    Controller Controller;
    [SerializeField]
    ResetEvent ResetEvent;
    [SerializeField]
    InputEmulator InputEmulator;

    [SerializeField]
    float SpeedMultiplier;

    void Start()
    {
        Drag.MoveAsObservable().Subscribe(AddOffset).AddTo(this);

        ResetEvent.OnResetAsObservable().Subscribe(_ => transform.localPosition = Vector3.zero).AddTo(this);
    }

    void AddOffset(Vector3 grab)
    {
        if (Controller.GripPressed.Value)
        {
            grab = InputEmulator.CurrentRotation * grab;
        }
        else
        {
            grab.x = 0f;
            grab.z = 0f;
        }
        transform.Translate(grab * SpeedMultiplier * Time.deltaTime);
    }
}
