using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AntiRootedModule : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] CenterOfMass centerOfMass;
    [SerializeField] ResetEvent resetEvent;

    Vector3 lastCOM;
    bool lastActive;

    void Update()
    {
        if (!toggle.isOn)
        {
            lastActive = false;
            return;
        }

        if (!centerOfMass.TryGetCOM(out var com))
        {
            lastActive = false;
            return;
        }

        if (lastActive)
        {
            transform.Translate(lastCOM - com);
        }

        lastCOM = com;
        lastActive = true;
    }

    void Start()
    {
        resetEvent.OnResetAsObservable()
            .Subscribe(_ =>
            {
                lastActive = false;
                transform.localPosition = Vector3.zero;
            })
            .AddTo(this);
    }
}
