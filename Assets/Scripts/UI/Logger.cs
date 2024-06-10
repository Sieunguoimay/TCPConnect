using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private static Logger _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        RebuildLayout();
    }

    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(text.transform.parent as RectTransform);
    }

    public static void Log(string msg)
    {
        _instance.text.text += "#" + msg + "\n";
        LayoutRebuilder.ForceRebuildLayoutImmediate(_instance.text.transform.parent as RectTransform);
    }
}