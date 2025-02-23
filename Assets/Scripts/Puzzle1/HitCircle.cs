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
}
