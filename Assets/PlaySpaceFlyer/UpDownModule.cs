using UnityEngine;
using UniRx;
using System.Linq;

public class UpDownModule : MonoBehaviour
{
    [SerializeField]
    DoubleDrag[] Draggs;
    [SerializeField]
    Controller[] Controllers;

    [SerializeField]
    float SpeedMultiplier;

    void Start()
    {
        Draggs.Select(d => d.MoveAsObservable()).Merge().Subscribe(AddOffset).AddTo(this);

        Controllers.Select(c => c.GripPressed)
            .CombineLatestValuesAreAllTrue()
            .Where(on => on)
            .Subscribe(_ => transform.localPosition = Vector3.zero)
            .AddTo(this);
    }

    void AddOffset(Vector3 grab)
    {
        transform.Translate(0f, grab.y * SpeedMultiplier * Time.deltaTime, 0f);
    }
}
