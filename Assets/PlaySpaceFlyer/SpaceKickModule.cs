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
    Vector3 diff;

    readonly Subject<Vector3> kick = new Subject<Vector3>();
    public IObservable<Vector3> KickAsObservable() => kick;

    void Update()
    {
        if (!centerOfMass.TryGetCOM(out var center))
        {
            hasPrevDiff = false;
            return;
        }

        var prevDiff = diff;
        diff = center - footTracker.Position;
        if (!hasPrevDiff)
        {
            hasPrevDiff = true;
            return;
        }

        var deltaTime = Time.deltaTime;
        var velocity = (diff - prevDiff) / deltaTime;
        var magnitude = velocity.magnitude;
        if (magnitude > error)
        {
            hasPrevDiff = false;
            return;
        }
        if (magnitude < threshold || Vector3.Dot(diff, velocity) < 0)
        {
            return;
        }

        var magnifiedSpeed = threshold + magnify * (magnitude - threshold);
        kick.OnNext(velocity * (magnifiedSpeed / magnitude));
    }
}
