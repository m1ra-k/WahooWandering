using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Puzzle1GameplayManager : MonoBehaviour
{
    public TextAsset beatmapFile;
    public GameObject approachCircles;
    public GameObject done;

    public AudioSource audioSource;

    // make private after
    private int frameCount = 51;
    private int framesToWait = 52;
    private int beatmapListIndex;
    private List<List<(int, ApproachCircleTypeEnum?)>> beatmapList = new List<List<(int, ApproachCircleTypeEnum?)>>();
    private ApproachCircle approachCircle;
    
    void Awake()
    {
        // TODO: ALWAYS NEED Application.targetFrameRate OF 60
        // eventually put this in game state?
        Application.targetFrameRate = 60;

        print("now playing... " + beatmapFile.name + "!");

        string[] lines = beatmapFile.text.Split('\n');

        ApproachCircleTypeEnum? approachCircleTypeEnum = null;

        for (int i = 1; i < lines.Length; i++)
        {
            List<(int, ApproachCircleTypeEnum?)> beatsForFrameList = new List<(int, ApproachCircleTypeEnum?)>();
           
            if (lines[i].Length > 1)
            {
                string[] beatsForFrame = lines[i].Split('|');

                foreach (string beatForFrame in beatsForFrame)
                {
                    string[] beatDetails = beatForFrame.Trim().Split(',');

                    approachCircleTypeEnum = null;
                    switch (beatDetails[1])
                    {
                        case "Normal":
                            approachCircleTypeEnum = ApproachCircleTypeEnum.Normal;
                            break;
                        case "Fast":
                            approachCircleTypeEnum = ApproachCircleTypeEnum.Fast;
                            break;
                        case "Slow":
                            approachCircleTypeEnum = ApproachCircleTypeEnum.Slow;
                            break;
                    }

                    beatsForFrameList.Add((int.Parse(beatDetails[0]), approachCircleTypeEnum));
                }

                beatmapList.Add(beatsForFrameList);
            }
            else
            {
                beatmapList.Add(new List<(int, ApproachCircleTypeEnum?)> { (0, approachCircleTypeEnum) });
            }
        }

        approachCircle = approachCircles.GetComponent<ApproachCircle>();

        StartCoroutine(EnableAudioAfterWait());
    }

    void Update()
    {
        if (beatmapListIndex < beatmapList.Count)
        {
            // print($"frame count is {frameCount} and frames to wait is {framesToWait}");
            frameCount++;

            if (frameCount == framesToWait)
            {
                foreach (var tuple in beatmapList[beatmapListIndex])
                {
                    if (tuple.Item1 != 0)
                    {
                        approachCircle.GenerateApproachCircle(tuple.Item2.Value, tuple.Item1);
                    }

                    ApproachCircleTypeEnum? circleType = tuple.Item2;

                    if (circleType.HasValue)
                    {
                        switch (circleType.Value)
                        {
                            case ApproachCircleTypeEnum.Fast:
                                framesToWait = 26;
                                break;

                            case ApproachCircleTypeEnum.Normal:
                                framesToWait = 52;
                                break;                

                            case ApproachCircleTypeEnum.Slow:
                                framesToWait = 104;
                                break;
                        }
                    }
                }
                
                frameCount = 0;
                beatmapListIndex++;
            }
        }
        else
        {
            StartCoroutine(DisplayDone());
        }
    }

    IEnumerator EnableAudioAfterWait()
    {
        float waitTime = 0.86f;

        print(beatmapList.Count);
        ApproachCircleTypeEnum? circleType = beatmapList[0][0].Item2;

        if (circleType.HasValue)
        {
            switch (circleType.Value)
            {
                case ApproachCircleTypeEnum.Fast:
                    waitTime = 0.43f;
                    break;             

                case ApproachCircleTypeEnum.Slow:
                    waitTime = 1.72f;
                    break;
            }
        }
        print($"wait time is {waitTime}");
        yield return new WaitForSeconds(waitTime);

        audioSource.enabled = true;
    }
    
    IEnumerator DisplayDone()
    {
        yield return new WaitForSeconds(3f);

        done.SetActive(true);
    }

}
