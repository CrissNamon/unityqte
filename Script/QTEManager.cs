using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2019_4_OR_NEWER && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using DualShockGamepadPS4 = UnityEngine.InputSystem.DualShock.DualShock4GamepadHID;
#endif
#if UNITY_2018 && ENABLE_INPUT_SYSTEM
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PS4;
#endif

public class QTEManager : MonoBehaviour
{
    [Header("Configuration")]
    public float slowMotionTimeScale = 0.1f;

    [HideInInspector]
    private bool isEventStarted;
    private QTEEvent eventData;
    private bool isAllButtonsPressed;
    private bool isFail;
    private bool isEnded;
    private bool isPaused;
    private bool wrongKeyPressed;
    private float currentTime;
    private float smoothTimeUpdate;
    private float rememberTimeScale;
    private List<QTEKey> keys = new List<QTEKey>();

    protected void Update()
    {
        if (!isEventStarted || eventData == null || isPaused) return;
        updateTimer();
        if (keys.Count == 0 || isFail)
        {
            doFinally();
        }
        else
        {
            for (int i = 0; i < eventData.keys.Count; i++)
            {
                if(isGamePadConnected())
                {
                    checkGamepadInput(eventData.keys[i]);
                }
                else
                {
                    checkKeyboardInput(eventData.keys[i]);
                }
            }
        }
    }

    public void startEvent(QTEEvent eventScriptable)
    {
        eventData = eventScriptable;
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null)
        {
            Debug.Log("No keyboard connected. Gamepad input in QTE events is not supported now");
            return;
        }
        keys = new List<QTEKey>(eventData.keys);
#else
        keys = new List<QTEKey>(eventData.keys);
#endif
        if (eventData.onStart != null)
        {
            eventData.onStart.Invoke();
        }
        isAllButtonsPressed = false;
        isEnded = false;
        isFail = false;
        isPaused = false;
        rememberTimeScale = Time.timeScale;
        switch (eventScriptable.timeType)
        {
            case QTETimeType.Slow:
                Time.timeScale = slowMotionTimeScale;
                break;
            case QTETimeType.Paused:
                Time.timeScale = 0;
                break;
        }
        currentTime = eventData.time;
        smoothTimeUpdate = currentTime;
        setupGUI();
        StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        isEventStarted = true;
        while(currentTime > 0 && isEventStarted && !isEnded)
        {
            if(eventData.keyboardUI.eventTimerText != null)
            {
                eventData.keyboardUI.eventTimerText.text = currentTime.ToString();
            }
            currentTime--;
            yield return new WaitWhile(() => isPaused);
            yield return new WaitForSecondsRealtime(1f);
        }
        if(!isAllButtonsPressed && !isEnded)
        {
            isFail = true;
            doFinally();
        }
    }

    protected void doFinally()
    {
        if (keys.Count == 0)
        {
            isAllButtonsPressed = true;
        }
        isEnded = true;
        isEventStarted = false;
        Time.timeScale = rememberTimeScale;
        var ui = getUI();
        if (ui.eventUI != null)
        {
            ui.eventUI.SetActive(false);
        }
        if (eventData.onEnd != null)
        {
            eventData.onEnd.Invoke();
        }
        if(eventData.onFail != null && isFail)
        {
            eventData.onFail.Invoke();
        }
        if(eventData.onSuccess != null && isAllButtonsPressed)
        {
            eventData.onSuccess.Invoke();
        }
        eventData = null;
    }

    protected void OnGUI()
    {
        if (eventData == null || isEnded) return;
        if (Event.current.isKey && Event.current.type == EventType.KeyDown && eventData.failOnWrongKey && !Event.current.keyCode.ToString().Equals("None"))
        {
            wrongKeyPressed = true;
            if (isGamePadConnected())
            {
                eventData.keys.ForEach(key => wrongKeyPressed = wrongKeyPressed && (!key.gamepadDualShockKey.ToString().Equals(Event.current.keyCode.ToString()) || !key.gamepadDualShockKey.ToString().Equals(Event.current.keyCode.ToString())));
            }
            else
            {
                eventData.keys.ForEach(key => wrongKeyPressed = wrongKeyPressed && !key.keyboardKey.ToString().Equals(Event.current.keyCode.ToString()));
            }            
            
            isFail = wrongKeyPressed;
        }
    }

    protected void updateTimer()
    {
        smoothTimeUpdate -= Time.unscaledDeltaTime;
        var ui = getUI();
        if (ui.eventTimerImage != null)
        {
            ui.eventTimerImage.fillAmount = smoothTimeUpdate / eventData.time;
        }
    }

    public void pause()
    {
        isPaused = true;
    }

    public void play()
    {
        isPaused = false;
    }
#if !ENABLE_INPUT_SYSTEM
    private bool isGamePadConnected()
    {
        string[] temp = Input.GetJoystickNames();
        bool result = false;
        if (temp.Length > 0)
        {
            for (int i = 0; i < temp.Length; ++i)
            {
                result = result || !string.IsNullOrEmpty(temp[i]);
            }
        }
        return result;
    }

    public void checkKeyboardInput(QTEKey key)
    {
        if (Input.GetKeyDown(key.keyboardKey))
        {
            keys.Remove(key);
        }
        if (Input.GetKeyUp(key.keyboardKey) && eventData.pressType == QTEPressType.Simultaneously)
        {
            keys.Add(key);
        }
    }

    public void checkGamepadInput(QTEKey key)
    {
        if (Input.GetKeyDown(key.gamepadXBOXKey) || Input.GetKeyDown(key.gamepadDualShockKey))
        {
            keys.Remove(key);
        }
        if ((Input.GetKeyUp(key.gamepadXBOXKey) || Input.GetKeyUp(key.gamepadDualShockKey)) && eventData.pressType == QTEPressType.Simultaneously)
        {
            keys.Add(key);
        }
    }
#else
    public bool isGamePadConnected()
    {
        return Gamepad.current != null;
    }

    public bool isXBOXGamePad()
    {
        return DualShockGamepadPS4.current == null;
    }

    public void checkKeyboardInput(QTEKey key)
    {
        var inputType = Keyboard.current;
        if (inputType != null)
        {
            if (inputType[key.keyboardKey].wasPressedThisFrame)
            {
                keys.Remove(key);
            }
            if (inputType[key.keyboardKey].wasReleasedThisFrame && eventData.pressType == QTEPressType.Simultaneously)
            {
                keys.Add(key);
            }
        }
    }

    public void checkGamepadInput(QTEKey key)
    {
        var inputType = Gamepad.current;
        if (inputType != null)
        {
            if (inputType[key.gamepadXBOXKey].wasPressedThisFrame || inputType[key.gamepadDualShockKey].wasPressedThisFrame)
            {
                keys.Remove(key);
            }
            if ((inputType[key.gamepadXBOXKey].wasReleasedThisFrame || inputType[key.gamepadDualShockKey].wasReleasedThisFrame) && eventData.pressType == QTEPressType.Simultaneously)
            {
                keys.Add(key);
            }
        }
    }
#endif

    protected void setupGUI()
    {
        var ui = getUI();
        
        if (ui.eventTimerImage != null)
        {
            ui.eventTimerImage.fillAmount = 1;
        }
        if (ui.eventText != null)
        {
            ui.eventText.text = "";
            eventData.keys.ForEach(key => ui.eventText.text += key.keyboardKey + "+");
            eventData.keyboardUI.eventText.text = ui.eventText.text.Remove(ui.eventText.text.Length - 1);
        }
        if (ui.eventUI != null)
        {
            ui.eventUI.SetActive(true);
        }
    }

    protected QTEUI getUI()
    {
        var ui = eventData.keyboardUI;
        if (isGamePadConnected())
        {
#if ENABLE_INPUT_SYSTEM
            if (isXBOXGamePad())
            {
                ui = eventData.gamepadXBOXUI;
            }
            else
            {
                ui = eventData.gamepadDualShockUI;
            }
#else
            ui = eventData.gamepadUI;
#endif
        }
        return ui;
    }
}
