using UnityEngine;
using UniRx;

public sealed class SpaceMoveModule : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] SpaceKickModule[] kickModules;
    [SerializeField] Vector3 gravity;
    [SerializeField] Vector3 antiGravity;
    [SerializeField] float dampingValue;
    [SerializeField] float dampingRate;
    
    Vector3 velocity;

    void Start()
    {
        foreach (var kick in kickModules)
        {
            kick.KickAsObservable().Subscribe(k => velocity += k).AddTo(this);
        }
    }
    
    void Update()
    {
        var deltaTime = Time.deltaTime;
        var isUnderWorld = target.localPosition.y < 0;
        velocity += deltaTime * (isUnderWorld ? antiGravity : gravity);
        if (isUnderWorld)
        {
            velocity *= 1f - Mathf.Min(dampingRate * deltaTime, 1f);
        }
        velocity *= 1f - Mathf.Min(dampingValue / velocity.magnitude * deltaTime, 1f);

        target.localPosition += velocity * deltaTime;
    }
}
