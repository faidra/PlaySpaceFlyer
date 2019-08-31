using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class VRInputEmulator : IDisposable
{
    IntPtr vrInputSimulator;
    readonly bool autoConnect;
    bool disposed = false;
    public VRInputEmulator(bool autoConnect = true)
    {
        vrInputSimulator = CreateVRInputEmulator();
        this.autoConnect = autoConnect;
        if (autoConnect) Connect();
    }

    ~VRInputEmulator()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (disposed) return;
        if (autoConnect) Disconnect();
        DeleteVRInputEmulator(vrInputSimulator);
        disposed = true;
    }

    public void Connect() => Connect(vrInputSimulator);
    public void Disconnect() => Disconnect(vrInputSimulator);
    public void EnableDeviceOffsets(uint deviceId, bool enable, bool modal) => EnableDeviceOffsets(vrInputSimulator, deviceId, enable, modal);
    public void SetWorldFromDriverTranslationOffset(uint deviceId, Vector3 pos, bool modal) => SetWorldFromDriverTranslationOffset(vrInputSimulator, deviceId, pos.x, pos.y, pos.z, modal);

    #region DLL
    const string dllName = "libvrinputemulator";
    [DllImport(dllName)] static extern IntPtr CreateVRInputEmulator();
    [DllImport(dllName)] static extern void Connect(IntPtr instance);
    [DllImport(dllName)] static extern void Disconnect(IntPtr instance);
    [DllImport(dllName)] static extern void DeleteVRInputEmulator(IntPtr instance);
    [DllImport(dllName)] static extern void EnableDeviceOffsets(IntPtr instance, uint deviceId, bool enable, bool modal);
    [DllImport(dllName)] static extern void SetWorldFromDriverTranslationOffset(IntPtr instance, uint deviceId, float x, float y, float z, bool modal);
    #endregion
}
