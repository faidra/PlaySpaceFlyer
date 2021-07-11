using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform Target;

    void LateUpdate()
    {
        var transform = Target.transform;
        var position = transform.position;
        var rotation = transform.rotation;
        InputEmulator.SetAllDeviceTransform(position, rotation);
    }
}
