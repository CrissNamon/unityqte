using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_2019_4_OR_NEWER && ENABLE_INPUT_SYSTEM
using KeyCode = UnityEngine.InputSystem.Key;
using UnityEngine.InputSystem.LowLevel;
#endif
#if UNITY_2018 && ENABLE_INPUT_SYSTEM
using KeyCode = UnityEngine.Experimental.Input.Key;
using UnityEngine.Experimental.Input.LowLevel;
#endif
#if !ENABLE_INPUT_SYSTEM
using GamepadButton = UnityEngine.KeyCode;
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

[System.Serializable]
public class QTEKey
{
    public KeyCode keyboardKey;
    public GamepadButton gamepadXBOXKey;
    public GamepadButton gamepadDualShockKey;
}

[System.Serializable]
public class QTEUI
{
    public GameObject eventUI;
    public Text eventText;
    public Text eventTimerText;
    public Image eventTimerImage;
}

public class QTEEvent : MonoBehaviour
{
    [Header("Event settings")]
    public List<QTEKey> keys = new List<QTEKey>();
    public QTETimeType timeType;
    public float time = 3f;
    public bool failOnWrongKey;
    public QTEPressType pressType;
    [Header("UI")]
    public QTEUI keyboardUI;
#if ENABLE_INPUT_SYSTEM
    public QTEUI gamepadXBOXUI;
    public QTEUI gamepadDualShockUI;
#else
    public QTEUI gamepadUI;
#endif
    [Header("Event actions")]
    public UnityEvent onStart;
    public UnityEvent onEnd;
    public UnityEvent onSuccess;
    public UnityEvent onFail;
}
