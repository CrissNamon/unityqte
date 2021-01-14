using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public float time;
    public bool failOnWrongKey;
    public QTEPressType pressType;
    [Header("Event actions")]
    public UnityEvent onStart;
    public UnityEvent onEnd;
    public UnityEvent onSuccess;
    public UnityEvent onFail;
}
