using System.Text;
using UnityEngine;
using UnityEngine.UI;

public sealed class Logger : MonoBehaviour
{
    static Logger instance;

    public static void Log(string str)
    {
        if (instance == null) instance = FindObjectOfType<Logger>();
        if (instance == null)
        {
            Debug.LogError("found no logger");
            return;
        }
        instance.LogString(str);
    }

    [SerializeField] Text text;

    const int Capacity = 1000;
    readonly StringBuilder builder = new StringBuilder();

    void LogString(string str)
    {
        builder.Insert(0, "\n");
        builder.Insert(0, str);
        if (builder.Length > Capacity) builder.Length = Capacity;
        text.text = builder.ToString();
    }
}
