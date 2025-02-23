using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle2GameplayManager : MonoBehaviour
{
    public GameObject primaryCountdown;

    public int action;

    public int smallTalkIndex;
    public Image qteButtonPrompt;
    public Image qteButtonInnerCircle;
    public TextMeshProUGUI qteButtonText;
    private List<KeyCode> qteKeyCodes = new List<KeyCode>
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
    };
    private Coroutine qteCoroutine;
    private bool recievedInput;

    public GameObject done;

    public GameProgressionManager GameProgressionManager;
    private bool transitioned;

    private bool minigameDone;
    
    void Awake()
    {
        // TODO: ALWAYS NEED Application.targetFrameRate OF 60
        // eventually put this in game state?
        Application.targetFrameRate = 60;

        GameProgressionManager = GameObject.Find("GameProgressionManager").GetComponent<GameProgressionManager>();
    }

    void Update()
    {
        InitiateQTE();    
    }

    private void InitiateQTE()
    {
        if (qteCoroutine == null && !minigameDone)
        {
            qteCoroutine = StartCoroutine(QTE());
        }        
    }

    private IEnumerator QTE()
    {
        recievedInput = false;

        int randomWaitTime = Random.Range(5, 11);
        yield return new WaitForSeconds(randomWaitTime);
        // yield return new WaitForSeconds(1f);

        KeyCode randomQuickTimeEventKeyCode = qteKeyCodes[Random.Range(0, 8)];

        float originalDuration = 1f;
        float duration = originalDuration;

        qteButtonPrompt.fillAmount = 1f;

        QTEButtonDisplay(true, randomQuickTimeEventKeyCode);
        
        while (duration > 0)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(randomQuickTimeEventKeyCode))
                {
                    recievedInput = true;

                    if (!minigameDone)
                    {
                        StartCoroutine(DisplayDone());
                        minigameDone = true;
                    }
                }
                else
                {
                    print("Skipping, did not press the correct key.");
                    recievedInput = false;
                }
                
                QTEButtonDisplay(false);

                yield break;
            }

            duration -= Time.deltaTime;
            qteButtonPrompt.fillAmount = duration / originalDuration;
            yield return null;
        }

        action++;

        QTEButtonDisplay(false);

        print("Skipping, did not press the correct key in time.");
        recievedInput = false;
    }

    private void QTEButtonDisplay(bool display, KeyCode randomQuickTimeEventKeyCode = KeyCode.None)
    {
        // countdown
        qteButtonPrompt.enabled = display;

        // inner circle
        qteButtonInnerCircle.enabled = display;

        // text
        qteButtonText.text = randomQuickTimeEventKeyCode != KeyCode.None
                            ? randomQuickTimeEventKeyCode.ToString()[^1].ToString()
                            : "";
    }
    
    IEnumerator DisplayDone()
    {
        yield return new WaitForSeconds(3f);

        done.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (!transitioned)
        {
            GameProgressionManager.TransitionScene("finished: claw machine"); 
            transitioned = true;
        }
    }

}
