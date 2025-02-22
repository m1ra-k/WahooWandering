using UnityEngine;
using UnityEngine.UI;

public class ScoreMeter : MonoBehaviour
{
    public int maxScore = 100;
    public int newScore;
    public int oldScore;

    private Slider scoreMeter;
    private float scoreMeterMaxValue;
    private Score score;

    void Start()
    {
        scoreMeter = GetComponent<Slider>();  
        scoreMeter.maxValue = maxScore;
        scoreMeterMaxValue = scoreMeter.maxValue;
        
        score = GetComponentInChildren<Score>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && newScore < scoreMeterMaxValue)
        {
            UpdateScoreMeter(15);
        }

        if (newScore != oldScore)
        {
            score.UpdateScore();
        }

        oldScore = newScore;
    }

    // TODO
    // have this referenced in the HitCircle script
    // for okay, good, great
    public void UpdateScoreMeter(int points)
    {
        newScore += points;

        if (newScore > scoreMeterMaxValue)
        {
            newScore = (int) scoreMeterMaxValue;
        }

        float progress = newScore;
        scoreMeter.value = progress;
    }
}
