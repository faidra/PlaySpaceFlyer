using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Valve.VR;

public class OpenVRInitializer : MonoBehaviour
{
    IEnumerator Start()
    {
        var openVRError = EVRInitError.None;

        Application.targetFrameRate = 120;

        //OpenVRの初期化
        OpenVR.Init(ref openVRError, EVRApplicationType.VRApplication_Overlay);
        if (openVRError != EVRInitError.None)
        {
            throw new Exception("OpenVRの初期化に失敗." + openVRError.ToString());
        }

        if (XRSettings.loadedDeviceName != "OpenVR")
        {
            XRSettings.LoadDeviceByName("OpenVR");
            yield return null;
            if (XRSettings.loadedDeviceName != "OpenVR") throw new Exception("failed to load OpenVR");
        }

        XRSettings.enabled = true;
        
        SteamVR.Initialize();

        DontDestroyOnLoad(gameObject);

        SceneManager.LoadScene("Main");
    }

    void OnDestroy()
    {
        if (XRSettings.loadedDeviceName != "None")
        {
            XRSettings.LoadDeviceByName("None");
        }
        SteamVR.SafeDispose();
        XRSettings.enabled = false;
        OpenVR.Shutdown();
    }
}