using System;
using UniRx;
using UnityEngine;

public sealed class SpaceKickModule : MonoBehaviour
{
    [SerializeField] CenterOfMass centerOfMass;
    [SerializeField] Tracker footTracker;
    [SerializeField] float threshold;
    [SerializeField] float magnify;
    [SerializeField] float error;

    bool hasPrevDiff;
    Vector3 legToCenter;
    Vector3 velocity;

    readonly Subject<Vector3> kick = new Subject<Vector3>();
    public IObservable<Vector3> KickAsObservable() => kick;

    void Start()
    {
        const float speedToSize = 1f;
        PoseVisualizer.Create(this, () =>
            new PoseVisualizer.Param(
                hasPrevDiff,
                footTracker.Position + velocity * speedToSize * 0.5f,
                velocity.sqrMagnitude > 0 ? Quaternion.LookRotation(velocity) : Quaternion.identity,
                new Vector3(0.1f, 0.1f, speedToSize * velocity.magnitude)));
    }

    void Update()
    {
        if (!centerOfMass.TryGetCOM(out var center))
        {
            hasPrevDiff = false;
            return;
        }

        var prevLegToCenter = legToCenter;
        legToCenter = center - footTracker.Position;
        if (!hasPrevDiff)
        {
            hasPrevDiff = true;
            return;
        }

        var deltaTime = Time.deltaTime;
        velocity = (legToCenter - prevLegToCenter) / deltaTime;
        var magnitude = velocity.magnitude;
        if (magnitude > error)
        {
            hasPrevDiff = false;
            return;
        }
        if (magnitude < threshold || Vector3.Dot(legToCenter, velocity) < 0)
        {
            return;
        }

        var magnifiedSpeed = threshold + magnify * (magnitude - threshold);
        kick.OnNext(velocity * (magnifiedSpeed / magnitude));
    }
}
