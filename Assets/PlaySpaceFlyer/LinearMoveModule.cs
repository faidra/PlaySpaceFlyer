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
    VRCMoving Moving;

    [SerializeField] SwitchMove SwitchMove;

    [SerializeField]
    float SpeedMultiplier;

    void Start()
    {
        var moveOrSwitch = Moving.IsMovingAsObservable().CombineLatest(SwitchMove.IsSwitchingAsObservable(), (m, s) => m || s);
        Drag.MoveAsObservable()
            .WithLatestFrom(moveOrSwitch, (v, moving) => (v, moving))
            .Subscribe(t => AddOffset(t.v, t.moving)).AddTo(this);

        ResetEvent.OnResetAsObservable()
            .Subscribe(_ => transform.localPosition = Vector3.zero)
            .AddTo(this);
    }

    void AddOffset(Vector3 grab, bool isMoving)
    {
        if (Controller.MenuPressed.Value)
        {
            if (isMoving) grab.y = 0f;
            grab = InputEmulator.CurrentRotation * grab;
        }
        else
        {
            if (isMoving) return;
            grab.x = 0f;
            grab.z = 0f;
        }

        transform.Translate(grab * SpeedMultiplier * Time.deltaTime);
    }
}
