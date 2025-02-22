using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    private ScoreMeter scoreMeter;
    private int newScore;
    private int oldScore;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        scoreMeter = GetComponentInParent<ScoreMeter>(); 
    }

    public void UpdateScore()
    {
        newScore = scoreMeter.newScore;
        oldScore = scoreMeter.oldScore;

        tmp.text = "score: " + newScore.ToString("D4");
    }
}
