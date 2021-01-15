using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float currentTime;
    private float smoothTimeUpdate;
    private List<KeyCode> keys = new List<KeyCode>();

    protected void Update()
    {
        if (!isEventStarted || eventData == null) return;
        updateTimer();
        if (keys.Count == 0 || isFail)
        {
            doFinally();
        }
        else
        {
            for (int i = 0; i < eventData.keys.Count; i++)
            {
                if (Input.GetKeyDown(eventData.keys[i]))
                {
                    keys.Remove(eventData.keys[i]);
                }
            }
        }
    }

    public void startEvent(QTEEvent eventScriptable)
    {
        eventData = eventScriptable;
        if(eventData.onStart != null)
        {
            eventData.onStart.Invoke();
        }
        isAllButtonsPressed = false;
        isEnded = false;
        isFail = false;
        keys = new List<KeyCode>(eventData.keys);
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
        if (eventData.eventTimerImage != null)
        {
            eventData.eventTimerImage.fillAmount = 1;
        }
        if (eventData.eventText != null)
        {
            eventData.eventText.text = "";
            eventData.keys.ForEach(key => eventData.eventText.text += key + "+");
            eventData.eventText.text = eventData.eventText.text.Remove(eventData.eventText.text.Length - 1);
        }
        if (eventData.eventUI != null)
        {
            eventData.eventUI.SetActive(true);
        }
        StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        isEventStarted = true;
        while(currentTime > 0 && isEventStarted && !isEnded)
        {
            if(eventData.eventTimerText != null)
            {
                eventData.eventTimerText.text = currentTime.ToString();
            }
            currentTime--;
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
        Time.timeScale = 1f;
        if (eventData.eventUI != null)
        {
            eventData.eventUI.SetActive(false);
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
        if (!isEventStarted || eventData == null || isEnded || isFail) return;
        if (Event.current.isKey && Event.current.type == EventType.KeyUp && eventData.pressType == QTEPressType.Simultaneously)
        {
            if (eventData.keys.Contains(Event.current.keyCode))
            {
                keys.Add(Event.current.keyCode);
            }
        }
        if (Event.current.isKey && Event.current.type == EventType.KeyDown && eventData.failOnWrongKey)
        {
            if (!eventData.keys.Contains(Event.current.keyCode) && Event.current.keyCode!=KeyCode.None) {
                isFail = true;
            }
        }
    }

    protected void updateTimer()
    {
        smoothTimeUpdate -= Time.unscaledDeltaTime;
        if (eventData.eventTimerImage != null)
        {
            eventData.eventTimerImage.fillAmount = smoothTimeUpdate / eventData.time;
        }
    }
}
