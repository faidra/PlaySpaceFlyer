using UnityEngine;

public sealed class LighthouseOffset : MonoBehaviour
{
    public Vector3 CurrentOffset { get; private set; }
    public Quaternion CurrentRotation { get; private set; } = Quaternion.identity;

    public void SetValues(Vector3 offset, Quaternion rotation)
    {
        CurrentOffset = offset;
        CurrentRotation = rotation;
    }
}
