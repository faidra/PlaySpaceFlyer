using UnityEngine;
using UniRx;
using System.Linq;

public class UpDownModule : MonoBehaviour
{
    [SerializeField]
    DoubleDrag[] Draggs;
    [SerializeField]
    ResetEvent ResetEvent;

    [SerializeField]
    float SpeedMultiplier;

    void Start()
    {
        Draggs.Select(d => d.MoveAsObservable()).Merge().Subscribe(AddOffset).AddTo(this);

        ResetEvent.OnResetAsObservable().Subscribe(_ => transform.localPosition = Vector3.zero).AddTo(this);
    }

    void AddOffset(Vector3 grab)
    {
        transform.Translate(0f, grab.y * SpeedMultiplier * Time.deltaTime, 0f);
    }
}
