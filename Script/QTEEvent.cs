using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public List<KeyCode> keys = new List<KeyCode>();
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
