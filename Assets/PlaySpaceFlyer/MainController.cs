using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField]
    float PhaseMultiplier;
    [SerializeField]
    float SpeedMultiplier;
    [SerializeField]
    InputSimulator InputSimulator;

    void Update()
    {
        //InputSimulator.SetAllDeviceWorldPosOffset(Vector3.up * Mathf.Sin(Time.time * PhaseMultiplier) * SpeedMultiplier);
    }
}
