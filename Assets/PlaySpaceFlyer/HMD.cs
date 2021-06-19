using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class HMD : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources inputSource;

    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    void Update()
    {
        var device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
        {
            Position = position;
        }
        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
        {
            Rotation = rotation;
        }
    }
}
