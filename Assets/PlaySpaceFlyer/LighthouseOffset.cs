using UnityEngine;

public sealed class LightOffset : MonoBehaviour
{
    public Vector3 CurrentOffset { get; private set; }
    public Quaternion CurrentRotation { get; private set; } = Quaternion.identity;

    const string QuestOffsetPositionKey = "QuestOffsetPosition.{0}";
    const string QuestOffsetRotationKey = "QuestOffsetRotation.{0}";

    void Start()
    {
        Import();
    }

    void Import()
    {
        if (!PlayerPrefs.HasKey(GetPositionKey(0))) return;
        Vector3 offset;
        offset.x = PlayerPrefs.GetFloat(GetPositionKey(0));
        offset.y = PlayerPrefs.GetFloat(GetPositionKey(1));
        offset.z = PlayerPrefs.GetFloat(GetPositionKey(2));
        Quaternion rotation;
        rotation.x = PlayerPrefs.GetFloat(GetRotationKey(0));
        rotation.y = PlayerPrefs.GetFloat(GetRotationKey(1));
        rotation.z = PlayerPrefs.GetFloat(GetRotationKey(2));
        rotation.w = PlayerPrefs.GetFloat(GetRotationKey(3));
        CurrentOffset = offset;
        CurrentRotation = rotation;
        
        // todo: これだとModuleの方が初期化されたままだから、更新した瞬間死ぬ
        // todo: 初期値をキャリブレーションに反映する
    }

    static string GetPositionKey(int index)
    {
        return string.Format(QuestOffsetPositionKey, index);
    }

    static string GetRotationKey(int index)
    {
        return string.Format(QuestOffsetRotationKey, index);
    }

    public void SetValues(Vector3 offset, Quaternion rotation)
    {
        CurrentOffset = offset;
        CurrentRotation = rotation;
        PlayerPrefs.SetFloat(GetPositionKey(0), offset.x);
        PlayerPrefs.SetFloat(GetPositionKey(1), offset.y);
        PlayerPrefs.SetFloat(GetPositionKey(2), offset.z);
        PlayerPrefs.SetFloat(GetRotationKey(0), rotation.x);
        PlayerPrefs.SetFloat(GetRotationKey(1), rotation.y);
        PlayerPrefs.SetFloat(GetRotationKey(2), rotation.z);
        PlayerPrefs.SetFloat(GetRotationKey(3), rotation.w);
        PlayerPrefs.Save();
    }
}
