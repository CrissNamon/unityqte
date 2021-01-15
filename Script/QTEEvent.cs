using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.Experimental.Input;
#endif


public enum QTETimeType
{
    Normal,
    Slow,
    Paused
}

public enum QTEPressType
{
    Single,
    Simultaneously
}

public class QTEEvent : MonoBehaviour
{
    [Header("Event settings")]
#if ENABLE_INPUT_SYSTEM
    public List<Key> keys = new List<Key>();
#else
    public List<KeyCode> keys = new List<KeyCode>();
#endif
    public QTETimeType timeType;
    public float time = 3f;
    public bool failOnWrongKey;
    public QTEPressType pressType;
    [Header("UI")]
    public GameObject eventUI;
    public Text eventText;
    public Text eventTimerText;
    public Image eventTimerImage;
    [Header("Event actions")]
    public UnityEvent onStart;
    public UnityEvent onEnd;
    public UnityEvent onSuccess;
    public UnityEvent onFail;
}
