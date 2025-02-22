using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class HitCircle : MonoBehaviour
{
    public List<GameObject> hitCircles;
    public ScoreMeter scoreMeter;
    private List<Button> hitCircleButtons;
    private AudioSource audioSource;

    void Start()
    {
        // hitCircleButtons
        hitCircleButtons = new List<Button>();

        foreach (GameObject hitCircle in hitCircles)
        {
            hitCircleButtons.Add(hitCircle.GetComponent<Button>());
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        DetermineButtonPress(1);
    }

    private void DetermineButtonPress(int number)
    {
        KeyCode triggerKey = KeyCode.None;
        switch (number)
        {
            case 1:
                triggerKey = KeyCode.Alpha1;
                break;
            case 2:
                triggerKey = KeyCode.Alpha2;
                break;
            case 3:
                triggerKey = KeyCode.Alpha3;
                break;
            case 4:
                triggerKey = KeyCode.Alpha4;
                break;
            case 5:
                triggerKey = KeyCode.Alpha5;
                break;
        }

        if (Input.GetKeyDown(triggerKey)) 
        {    
            hitCircleButtons[number - 1].onClick.Invoke();
            audioSource.PlayOneShot(audioSource.clip);
        }

        // color
        if (Input.GetKey(triggerKey)) 
        {
            if (GetButtonColor(hitCircleButtons[number - 1]) != Color.red)
            {
                SetButtonColor(hitCircleButtons[number - 1], pressed: true);
            }
        }
        else 
        {
            if (GetButtonColor(hitCircleButtons[number - 1]) != Color.gray)
            {
                SetButtonColor(hitCircleButtons[number - 1], pressed: false);
            }
        }
    }
    
    private Color GetButtonColor(Button button)
    {
        return button.colors.normalColor;
    }

    private void SetButtonColor(Button hitCircleButton, bool pressed)
    {
        ColorBlock cb = hitCircleButton.colors;
        if (pressed) 
        {
            cb.normalColor = Color.red;
        }
        else
        {
            cb.normalColor = Color.gray;
        }

        hitCircleButton.colors = cb;
    }
}
