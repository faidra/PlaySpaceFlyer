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
        com = hmd.Position * 0.2f +
              lHand.Position * 0.2f +
              rHand.Position * 0.2f +
              chest.Position * 0.2f +
              lFoot.Position * 0.2f +
              rFoot.Position * 0.2f;
        return true;
    }
}
