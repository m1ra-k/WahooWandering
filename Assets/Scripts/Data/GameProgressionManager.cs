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

    [Header("[Restaurant Choice (Dinner)]")]
    // Day Shift - Dinner
    public int spaghettiWesternDinnerVisitNumber;
    public int jollyRogerDinnerVisitNumber;
    public int ratCityDinnerVisitNumber;
    public int kafeKittyDinnerVisitNumber;

    [Header("[Restaurant Choice (Brunch)]")]
    // Night Shift - Brunch
    public int spaghettiWesternBrunchVisitNumber;
    public int jollyRogerBrunchVisitNumber;
    public int ratCityBrunchVisitNumber;
    public int kafeKittyBrunchVisitNumber;

    [Header("[Music]")]
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioSource audioSourceBGM;
    private int currentTrack;

    // Data
    public TextAsset nextSceneVisualNovelJSONFile;
    public Dictionary <int, Dictionary<int, List<string>>> sceneProgressionLookup = new Dictionary<int, Dictionary<int, List<string>>> 
    {
        {
            0, new Dictionary<int, List<string>>
            {
                { 0, new List<string> { "VisualNovel", "day_00_scene_00_fries_with_friends_going_undercover" } },
                { 1, new List<string> { "VisualNovel", "day_00_scene_01_bloody_burger_foh_interview" } },
                { 2, new List<string> { "VisualNovel", "day_00_scene_02_restaurant_choice_dinner" } },
                { 3, new List<string> { "VisualNovel", "day_00_scene_03_nightly_reflection" } }
            }
        },
        {
            1, new Dictionary<int, List<string>>
            {
                { 0, new List<string> { "VisualNovel", "day_01_scene_00_bloody_burger_foh_clock_in" } }
            }
        }
    };
    // Ending (Lookup)
    private List<string> endingLookup = new List<string>
    {
        "ending_00_failed_the_interview",
        "ending_01_fired_from_day_shift",
        "ending_02_fired_from_night_shift"
    };
    // Day Shift - Dinner (Lookup)
    private List<string> spaghettiWesternDinnerVisitLookup = new List<string>
    {
        "visit_00_spaghetti_western_dinner",
        "visit_01_spaghetti_western_dinner",
        "visit_02_spaghetti_western_dinner",
        "visit_03_spaghetti_western_dinner",
        "visit_04_spaghetti_western_dinner",
        "visit_05_spaghetti_western_dinner",
        "visit_06_spaghetti_western_dinner",
        "visit_07_spaghetti_western_dinner"
    };
    private List<string> jollyRogerDinnerVisitLookup = new List<string>
    {
        "visit_00_jolly_roger_dinner",
        "visit_01_jolly_roger_dinner",
        "visit_02_jolly_roger_dinner",
        "visit_03_jolly_roger_dinner",
        "visit_04_jolly_roger_dinner",
        "visit_05_jolly_roger_dinner",
        "visit_06_jolly_roger_dinner",
        "visit_07_jolly_roger_dinner"
    };
    private List<string> ratCityDinnerVisitLookup = new List<string>
    {
        "visit_00_rat_city_dinner",
        "visit_01_rat_city_dinner",
        "visit_02_rat_city_dinner",
        "visit_03_rat_city_dinner",
        "visit_04_rat_city_dinner",
        "visit_05_rat_city_dinner",
        "visit_06_rat_city_dinner",
        "visit_07_rat_city_dinner"
    };
    private List<string> kafeKittyDinnerVisitLookup = new List<string>
    {
        "visit_00_kafe_kitty_dinner",
        "visit_01_kafe_kitty_dinner",
        "visit_02_kafe_kitty_dinner",
        "visit_03_kafe_kitty_dinner",
        "visit_04_kafe_kitty_dinner",
        "visit_05_kafe_kitty_dinner",
        "visit_06_kafe_kitty_dinner",
        "visit_07_kafe_kitty_dinner"
    };
    // Night Shift - Brunch (Lookup)
    private List<string> spaghettiWesternBrunchVisitLookup = new List<string>
    {
        "visit_00_spaghetti_western_brunch",
        "visit_01_spaghetti_western_brunch",
        "visit_02_spaghetti_western_brunch",
        "visit_03_spaghetti_western_brunch",
        "visit_04_spaghetti_western_brunch",
        "visit_05_spaghetti_western_brunch",
        "visit_06_spaghetti_western_brunch"
    };
    private List<string> jollyRogerBrunchVisitLookup = new List<string>
    {
        "visit_00_jolly_roger_brunch",
        "visit_01_jolly_roger_brunch",
        "visit_02_jolly_roger_brunch",
        "visit_03_jolly_roger_brunch",
        "visit_04_jolly_roger_brunch",
        "visit_05_jolly_roger_brunch",
        "visit_06_jolly_roger_brunch"
    };
    private List<string> ratCityBrunchVisitLookup = new List<string>
    {
        "visit_00_rat_city_brunch",
        "visit_01_rat_city_brunch",
        "visit_02_rat_city_brunch",
        "visit_03_rat_city_brunch",
        "visit_04_rat_city_brunch",
        "visit_05_rat_city_brunch",
        "visit_06_rat_city_brunch"
    };
    private List<string> kafeKittyBrunchVisitLookup = new List<string>
    {
        "visit_00_kafe_kitty_brunch",
        "visit_01_kafe_kitty_brunch",
        "visit_02_kafe_kitty_brunch",
        "visit_03_kafe_kitty_brunch",
        "visit_04_kafe_kitty_brunch",
        "visit_05_kafe_kitty_brunch",
        "visit_06_kafe_kitty_brunch",
    };
    // Work Shift - BOH Selection (Lookup)
    private List<string> workShiftBackOfHouseSelectionOrders = new List<string>
    {
        "day_01_boh_selection_orders", // START DAY SHIFT
        "day_02_boh_selection_orders",
        "day_03_boh_selection_orders",
        "day_04_boh_selection_orders",
        "day_05_boh_selection_orders",
        "day_06_boh_selection_orders",
        "day_07_boh_selection_orders", // END DAY SHIFT

        "day_08_boh_selection_orders", // START NIGHT SHIFT
        "day_09_boh_selection_orders",
        "day_10_boh_selection_orders",
        "day_11_boh_selection_orders",
        "day_12_boh_selection_orders",
        "day_13_boh_selection_orders",
        "day_14_boh_selection_orders", // END DAY SHIFT
    };
    // Work Shift - BOH Action (Lookup)
    private List<string> workShiftBackOfHouseActionOrders = new List<string>
    {
        "day_01_boh_action_orders", // START DAY SHIFT
        "day_02_boh_action_orders",
        "day_03_boh_action_orders",
        "day_04_boh_action_orders",
        "day_05_boh_action_orders",
        "day_06_boh_action_orders",
        "day_07_boh_action_orders", // END DAY SHIFT

        "day_08_boh_action_orders", // START NIGHT SHIFT
        "day_09_boh_action_orders",
        "day_10_boh_action_orders",
        "day_11_boh_action_orders",
        "day_12_boh_action_orders",
        "day_13_boh_action_orders",
        "day_14_boh_action_orders", // END DAY SHIFT
    };
    // Work Shift - FOH (Lookup) --> MEGA TODO, AFTER BOH THOUGH
    private List<string> workShiftFrontOfHouseActionOrders = new List<string>
    {
        "day_01_flashy_lady", // START DAY SHIFT
        "day_02_foh_orders",
        "day_03_foh_orders",
        "day_04_foh_orders",
        "day_05_foh_orders",
        "day_06_foh_orders",
        "day_07_foh_orders", // END DAY SHIFT

        "day_08_foh_orders", // START NIGHT SHIFT
        "day_09_foh_orders",
        "day_10_foh_orders",
        "day_11_foh_orders",
        "day_12_foh_orders",
        "day_13_foh_orders",
        "day_14_foh_orders", // END DAY SHIFT
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
                    nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Scenes/VisualNovel/Days/{nextSceneVisualNovelJSONFileName}");

                    fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "VisualNovel");
                    break;
                    
                // TODO focus on VisualNovel for now
                case "BOHAction":
                case "BOHSelection":
                case "FOH":
                    break;
            }
        }
        else if (possibleFlag.Contains("ending")) // ending stuff
        {
            switch (possibleFlag)
            {
                case "ending: failed the interview":
                    FindEndingJSONFile(0);
                    break;

                case "ending: fired from day shift":
                    FindEndingJSONFile(1);
                    break;

                case "ending: fired from night shift":
                    FindEndingJSONFile(2);
                    break;
            }

            fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "VisualNovel");
        }
        else if (possibleFlag.Contains("dinner")) // route determination stuff
        {               
            switch (possibleFlag)
            {
                case "dinner: spaghetti western":
                    FindRestaurantVisitJSONFile(spaghettiWesternDinnerVisitLookup, spaghettiWesternDinnerVisitNumber);
                    spaghettiWesternDinnerVisitNumber++;
                    break;

                case "dinner: jolly roger":
                    FindRestaurantVisitJSONFile(jollyRogerDinnerVisitLookup, jollyRogerDinnerVisitNumber);
                    jollyRogerDinnerVisitNumber++;
                    break;

                case "dinner: rat city":
                    FindRestaurantVisitJSONFile(ratCityDinnerVisitLookup, ratCityDinnerVisitNumber);
                    ratCityDinnerVisitNumber++;
                    break;

                case "dinner: kafe kitty":
                    FindRestaurantVisitJSONFile(kafeKittyDinnerVisitLookup, kafeKittyDinnerVisitNumber);
                    kafeKittyDinnerVisitNumber++;
                    break;
            }

            fadeEffect.FadeIn(blackTransition, fadeTime: 0.5f, scene: "VisualNovel");
        }
    }

    public void FindEndingJSONFile(int endingNumber)
    {
        string nextSceneVisualNovelJSONFileName = endingLookup[endingNumber];
        nextSceneVisualNovelJSONFile = Resources.Load<TextAsset>($"Scenes/VisualNovel/Endings/{nextSceneVisualNovelJSONFileName}");
    }

    public void FindRestaurantVisitJSONFile(List<string> restaurantVisitLookup, int restaurantVisitNumber)
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
