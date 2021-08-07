using System.Threading;
using UniRx;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] InputEmulator InputEmulator;
    [SerializeField] Transform Target;
    [SerializeField] int targetFrameRate = 90;

    void LateUpdate()
    {
        var transform = Target.transform;
        var position = transform.position;
        var rotation = transform.rotation;
        InputEmulator.SetAllDeviceTransform(position, rotation);
    }

    void Update()
    {
        Thread.Sleep(1000 / targetFrameRate);
    }

    void Start()
    {
        Observable.IntervalFrame(100)
            .Select(_ => Time.realtimeSinceStartup)
            .Pairwise()
            .Subscribe(p => Logger.Log($"FPS: {100 / (p.Current - p.Previous)}"))
            .AddTo(this);
    }
}
