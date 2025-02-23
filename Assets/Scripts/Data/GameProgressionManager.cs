using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameProgressionManager : MonoBehaviour
{
    public static GameProgressionManager GameProgressionManagerInstance;

    // Transition    
    private FadeEffect fadeEffect;
    private GameObject blackTransition;

    [Header("[Moment]")]
    public int dayNumber;
    public int sceneNumber;

    [Header("[Music]")]
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSourceBGM;
    private int currentTrack;

    // Data
    public TextAsset nextSceneVisualNovelJSONFile;    
    public Dictionary<string, bool> locationsVisited = new Dictionary<string, bool>
    {
        { "dairyMarket", false },
        { "downtownMall", false },
        { "bodosBagels", false }
    };
    public Dictionary <int, Dictionary<int, List<string>>> sceneProgressionLookup = new Dictionary<int, Dictionary<int, List<string>>> 
    {
        {
            0, new Dictionary<int, List<string>>
            {
                { 0, new List<string> { "VisualNovel", "scene0_begin" } },
                { 1, new List<string> { "VisualNovel", "sceneX_choose_location" } },
                { 2, new List<string> { "VisualNovel", "scene4_end" } }
            }
        }
    };

    void Awake()
    {        
        if (GameProgressionManagerInstance == null)
        {
            Application.targetFrameRate = 60;
            GameProgressionManagerInstance = this;
            DontDestroyOnLoad(gameObject);

            fadeEffect = GetComponent<FadeEffect>();

            // TODO CHANGE, JUST FOR DEBUGGING (expected -1, can change to 0, 1, or 2 for debug)
            dayNumber = 0; 
        }
        else
        {            
            Destroy(gameObject);
            return;
        }

        audioSourceBGM = GetComponent<AudioSource>();
        currentTrack = -1;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // print(scene.name);

        blackTransition = GameObject.Find("Canvas").transform.Find("BlackTransition").gameObject;

        fadeEffect.FadeOut(blackTransition, 0.5f);
    }
        
    public void TransitionScene(string possibleFlag)
    {
        if (string.IsNullOrEmpty(possibleFlag))
        {
            sceneNumber += 1;

            if (!sceneProgressionLookup[dayNumber].ContainsKey(sceneNumber))
            {
                dayNumber++;
                sceneNumber = 0;
            }

            string sceneType = sceneProgressionLookup[dayNumber][sceneNumber][0];
            switch (sceneType)
            {
                case "VisualNovel":
                    string nextSceneVisualNovelJSONFileName = sceneProgressionLookup[dayNumber][sceneNumber][1];
                    nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Dialogue/{nextSceneVisualNovelJSONFileName}");

                    fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "VisualNovel");
                    break;
            }
        }
        else if (possibleFlag.Contains("location")) // route determination stuff
        {               
            switch (possibleFlag)
            {
                case "location: dairy market":
                    nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Dialogue/scene1_dairy_market_begin_dialogue");
                    locationsVisited["dairyMarket"] = true;
                    break;

                case "location: downtown mall":
                    nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Dialogue/scene2_downtown_mall_begin_dialogue");
                    locationsVisited["downtownMall"]  = true;
                    break;

                case "location: bodosBagels":
                    nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Visits/scene3_bodos_bagel_begin_dialogue");
                    locationsVisited["bodosBagels"]  = true;
                    break;
            }

            fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "VisualNovel");
        }
    }

    public void FindLocationVisitJSONFile(List<string> restaurantVisitLookup, int restaurantVisitNumber)
    {
        string nextSceneVisualNovelJSONFileName = restaurantVisitLookup[restaurantVisitNumber];
        nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Scenes/VisualNovel/Visits/{nextSceneVisualNovelJSONFileName}");
    }

    public void StopMusic()
    {
        audioSourceBGM.Pause();
    }

    public void PlayMusic(int index)
    {
        audioSourceBGM.UnPause();

        if (index != currentTrack)
        {
            audioSourceBGM.clip = audioClips[index];
            audioSourceBGM.Play();

            currentTrack = index;
        }
    }
}
