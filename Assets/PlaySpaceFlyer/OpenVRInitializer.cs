using UnityEngine;
using Valve.VR;

public class OpenVRInitializer : MonoBehaviour
{
    void Awake()
    {
        var openVRError = EVRInitError.None;

        Application.targetFrameRate = 120;

        //OpenVRの初期化
        OpenVR.Init(ref openVRError, EVRApplicationType.VRApplication_Overlay);
        if (openVRError != EVRInitError.None)
        {
            Debug.LogError("OpenVRの初期化に失敗." + openVRError.ToString());
            return;
        }

        OpenVR.Compositor.SetTrackingSpace(ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated);
    }
}