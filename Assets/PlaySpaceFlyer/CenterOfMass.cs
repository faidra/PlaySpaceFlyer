using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    [SerializeField] HMD hmd;
    [SerializeField] Controller lHand;
    [SerializeField] Controller rHand;
    [SerializeField] Tracker chest;
    [SerializeField] Tracker lFoot;
    [SerializeField] Tracker rFoot;

    public bool TryGetCOM(out Vector3 com)
    {
        if (!lFoot.IsActive || !rFoot.IsActive || !chest.IsActive)
        {
            com = default;
            return false;
        }
        com = (hmd.Position * 1f +
              lHand.Position * 1f +
              rHand.Position * 1f +
              chest.Position * 1f +
              lFoot.Position * 1f +
              rFoot.Position * 1f) / 6f;
        return true;
    }

    void Start()
    {
        PoseVisualizer.Create(this, () =>
            new PoseVisualizer.Param(TryGetCOM(out var com), com, Quaternion.identity, new Vector3(0.3f, 0.3f, 0.3f)));
    }
}
