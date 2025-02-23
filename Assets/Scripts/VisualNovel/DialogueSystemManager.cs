using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueSystemManager : MonoBehaviour
{
    // data
    public SpriteCache spriteCache;

    // game objects
    public GameObject[] currentActiveNPC;
    public RectTransform[] currentActiveNPCRectTransforms;
    public GameObject[] oldActiveNPC;
    public GameObject currentActiveBG;
    public GameObject oldActiveBG;
    public GameObject currentPopupMC;
    public GameObject oldPopupMC;
    public GameObject normalBackground;
    
    // dialogue box
    public GameObject normalDialogue;
    public RectTransform normalDialogueRectTransform;
    private float targetNormalDialogueWidth;
    public GameObject normalCharacterName;

    // choice boxes
    public GameObject choiceBoxes;
    public bool buttonClicked = false;
    public bool choiceClicked = false;
    public int choiceMapping = -1;

    // dialogue (move this to a separate file later, a separate file containing all of the json files for every VN scene)
    public TextAsset visualNovelJSONFile;
    private List<DialogueStruct> dialogueList = new List<DialogueStruct>();

    // main
    public List<AudioClip> voices;
    private AudioSource audioSource;
    private int characterNumber;

    public bool transitioningScene;

    private DialogueStruct currentDialogue;
    private BaseDialogueStruct currentBaseDialogue;
    private Coroutine typeWriterCoroutine;
    private int dialogueIndex = -1;
    private int skippedFromIndex = -1;
    private int jumpToIndex = -1;
    private string dialogueOnDisplay;
    private bool typeWriterInEffect = false;
    public bool finishedDialogue = false;

    public bool spaceDisabled;

    private Dictionary<int, Vector2[]> NPCPositions = new()
    {
        { 1, new Vector2[] { new Vector2(0f, 0f) } },
        { 2, new Vector2[] { new Vector2(-225f, 0f), new Vector2(225f, 0f) } },
        { 3, new Vector2[] { new Vector2(0f, 0f), new Vector2(450f, 0f), new Vector2(-450f, 0f) } }
    };

    private Vector2[] targetPositions;
    
    // data
    public GameProgressionManager GameProgressionManager;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        GameProgressionManager = GameObject.Find("GameProgressionManager").GetComponent<GameProgressionManager>();
        if (GameProgressionManager.nextSceneVisualNovelJSONFile != null)
        {
            visualNovelJSONFile = GameProgressionManager.nextSceneVisualNovelJSONFile;            
        }
        else
        {
            // allows for skipping when need to debug        
            if (GameProgressionManager.dayNumber != 0 || GameProgressionManager.sceneNumber != 0)
            {
                Debug.Log($"Debug ON. Skipping to day {GameProgressionManager.dayNumber}, scene {GameProgressionManager.sceneNumber}.");
                string visualNovelJSONFileName = GameProgressionManager.sceneProgressionLookup[GameProgressionManager.dayNumber][GameProgressionManager.sceneNumber][1];
                visualNovelJSONFile = Resources.Load<TextAsset>($"Scenes/VisualNovel/Days/{visualNovelJSONFileName}");
            }
        }  

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = visualNovelJSONFile.name;

        if (sceneName.Contains("bloody_burger"))
        {
            GameProgressionManager.PlayMusic(2);
        }
        else if (sceneName.Contains("kafe_kitty"))
        {
            GameProgressionManager.PlayMusic(0);
        }
        else if (sceneName.Contains("spaghetti_western"))
        {
            GameProgressionManager.PlayMusic(1);
        }
        else if (sceneName.Contains("jolly_roger"))
        {
            GameProgressionManager.PlayMusic(3);
        }
        else if (sceneName.Contains("fries_with_friends"))
        {
            GameProgressionManager.PlayMusic(4);
        }
        else
        {
            GameProgressionManager.StopMusic();
        }
    }

    void Start() 
    {
        LoadVNDialogueFromJSON();

        ProgressMainVNSequence(isStartDialogue: true);
    }

    void Update()
    {
        // TODO: make sure to support saving on choice menu
        if ((Input.GetKeyDown(KeyCode.Space) || buttonClicked || choiceClicked) && !spaceDisabled && (currentBaseDialogue.vnType == VNTypeEnum.Choice || currentBaseDialogue.vnType == VNTypeEnum.Normal) && normalBackground.GetComponent<Image>().color.a == 1) 
        { 
            if (currentDialogue.endOfScene && !transitioningScene)
            {
                transitioningScene = true;
                GameProgressionManager.TransitionScene(currentDialogue.flag);
            }
            else if (!currentDialogue.endOfScene && !typeWriterInEffect && !finishedDialogue && choiceBoxes.transform.childCount == 0) 
            {
                if (choiceMapping != -1) // jumping out of the main sequence
                {
                    ProgressMainVNSequence(skipToIndex: choiceMapping);
                    choiceMapping = -1;
                }
                else if (jumpToIndex != -1) // jumping back into the main sequence
                {
                    ProgressMainVNSequence(skipToIndex: jumpToIndex);
                    jumpToIndex = -1;
                }
                else
                {
                    ProgressMainVNSequence();
                }
            }
            buttonClicked = false;
            choiceClicked = false;
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || buttonClicked) && typeWriterInEffect)
        {
            SkipTypeWriterEffect(inCG: currentDialogue.baseDialogue.vnType == VNTypeEnum.CG);
        }
    }

    public class DialogueStructContainer
    {
        public List<DialogueStruct> dialogues;
    }

    void LoadVNDialogueFromJSON()
    {
        string jsonWithRoot = visualNovelJSONFile.text;

        string processedJson = ProcessJSONForEnums(jsonWithRoot);

        DialogueStructContainer container = JsonUtility.FromJson<DialogueStructContainer>(processedJson);

        dialogueList = new List<DialogueStruct>(container.dialogues);

        foreach (var dialogue in dialogueList)
        {
            // foreach (var npcSprite in dialogue.baseDialogue.npcSprite)
            // {
            //     Debug.Log($"NPC Sprite: {npcSprite}");
            // }
            // Debug.Log($"VN Type: {dialogue.baseDialogue.vnType}");
            // Debug.Log($"Character: {dialogue.baseDialogue.character}");
            // Debug.Log($"NPC Sprite: {dialogue.baseDialogue.npcSprite}");
            // Debug.Log($"MC Sprite: {dialogue.baseDialogue.mcSprite}");
            // Debug.Log($"CG Sprite: {dialogue.baseDialogue.cgSprite}");
            // Debug.Log($"Dialogue: {dialogue.baseDialogue.dialogue}");
            // Debug.Log($"Text Speed: {dialogue.baseDialogue.textSpeed}");

            // foreach (var choice in dialogue.conditionalChoices)
            // {
            //     Debug.Log($"Choice Mapping: {choice.choiceMapping}, Choice Dialogue: {choice.choiceDialogue}");
            // }

            // Debug.Log($"Jump To Index: {dialogue.jumpToIndex}");
            // Debug.Log("-----------------------");
        }
    }

    private string ProcessJSONForEnums(string json)
    {
        foreach (VNTypeEnum e in Enum.GetValues(typeof(VNTypeEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (CharacterEnum e in Enum.GetValues(typeof(CharacterEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (NPCSpriteEnum e in Enum.GetValues(typeof(NPCSpriteEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (BGSpriteEnum e in Enum.GetValues(typeof(BGSpriteEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        foreach (CGSpriteEnum e in Enum.GetValues(typeof(CGSpriteEnum)))
        {
            json = json.Replace($"\"{e}\"", ((int)e).ToString());
        }

        return json;
    }

    void ProgressMainVNSequence(bool isStartDialogue = false, int skipToIndex = -1) 
    {
        StartCoroutine(DisableSpaceInput());
      
        if (skipToIndex != -1) 
        {
            skippedFromIndex = dialogueIndex;
            dialogueIndex = skipToIndex;
        }
        else 
        {
            dialogueIndex++;
        }
        currentDialogue = dialogueList[dialogueIndex];
        currentBaseDialogue = currentDialogue.baseDialogue;

        // set sprite
        SetSprite(currentBaseDialogue, dialogueIndex);
        
        // set dialogue
        SetDialogue(currentBaseDialogue);

        // set choices
        // if Choice available
        // would have to put this outside the loop to account for loading in to a scene
        switch (currentBaseDialogue.vnType) 
        {
            case VNTypeEnum.Choice:
            case VNTypeEnum.CGAndChoice:
                var temp = choiceBoxes.GetComponent<ChoiceMappingManager>();
                temp.SetChoiceMapping(this, currentDialogue.conditionalChoices);
                break;
        }

        if (currentDialogue.jumpToIndex != -1) 
        {
            jumpToIndex = currentDialogue.jumpToIndex;
        }

        if (dialogueIndex == dialogueList.Count - 1) 
        {
            finishedDialogue = true;
        }
    }
    
    void SetSprite(BaseDialogueStruct baseDialogue, int dialogueIndexToUse) 
    {
        bool isPreviousVnTypeCG = false;
        // skipped progression
        if (skippedFromIndex >= 0)
        {
            if (dialogueList[skippedFromIndex].baseDialogue.vnType == VNTypeEnum.CG || dialogueList[skippedFromIndex].baseDialogue.vnType == VNTypeEnum.CGAndChoice) 
            {
                isPreviousVnTypeCG = true;
            }
        }
        // normal progression
        else 
        {
            if (dialogueIndexToUse > 0)
            {
                if (dialogueList[dialogueIndexToUse - 1].baseDialogue.vnType == VNTypeEnum.CG || dialogueList[dialogueIndexToUse - 1].baseDialogue.vnType == VNTypeEnum.CGAndChoice) 
                {
                    isPreviousVnTypeCG = true;
                } 
            }
        }

        bool atStart = dialogueIndexToUse == 0;
        
        // // if the very first dialogue is not CG, we don't want the black transition
        // if (atStart && baseDialogue.vnType != VNTypeEnum.CG && baseDialogue.vnType != VNTypeEnum.CGAndChoice)
        // {
        //     cgStartTransition.GetComponent<Image>().color = new Color(1, 1, 1, 0); 
        // }

        if (baseDialogue.vnType != VNTypeEnum.CG && baseDialogue.vnType != VNTypeEnum.CGAndChoice) 
        {
            for (int i = 0; i < baseDialogue.npcSprite.Count; i++)
            {
                Sprite oldActiveNPCSprite = currentActiveNPC[i].GetComponent<Image>().sprite;
                Sprite newActiveNPCSprite = spriteCache.sprites[baseDialogue.npcSprite[i].ToString()];

                if (!oldActiveNPCSprite.ToString().Equals(newActiveNPCSprite.ToString())) 
                {
                    oldActiveNPC[i].GetComponent<Image>().sprite = currentActiveNPC[i].GetComponent<Image>().sprite;
                    StartCoroutine(Fade(currentActiveNPC[i], newActiveNPCSprite, 0, 1));
                    StartCoroutine(Fade(oldActiveNPC[i], oldActiveNPCSprite, 1, -1));
                }
            }

            targetPositions = NPCPositions.TryGetValue(baseDialogue.npcSprite.Count, out var positions)
                                        ? positions
                                        : Array.Empty<Vector2>();

            for (int i = 0; i < currentActiveNPC.Length; i++)
            {
                bool npcIsActive = i < baseDialogue.npcSprite.Count;

                if (currentActiveNPC[i].activeSelf != npcIsActive)
                {
                    oldActiveNPC[i].GetComponent<Image>().sprite = currentActiveNPC[i].GetComponent<Image>().sprite;
                    StartCoroutine(Fade(currentActiveNPC[i], spriteCache.sprites["Transparent"], 0, 1));
                    StartCoroutine(Fade(oldActiveNPC[i], oldActiveNPC[i].GetComponent<Image>().sprite, 1, -1));
                }

                if (npcIsActive && i < targetPositions.Length)
                {
                    currentActiveNPCRectTransforms[i].anchoredPosition = targetPositions[i];
                }
            }

            Sprite oldActiveBGSprite = currentActiveBG.GetComponent<Image>().sprite;
            Sprite newActiveBGSprite = spriteCache.sprites[baseDialogue.bgSprite.ToString()];

            if (!oldActiveBGSprite.ToString().Equals(newActiveBGSprite.ToString())) 
            {
                if (atStart)
                {
                    oldActiveBG.GetComponent<Image>().sprite = newActiveBGSprite;
                    currentActiveBG.GetComponent<Image>().sprite = newActiveBGSprite;
                }
                else
                {
                    oldActiveBG.GetComponent<Image>().sprite = currentActiveBG.GetComponent<Image>().sprite;
                    StartCoroutine(Fade(currentActiveBG, newActiveBGSprite, 0, 1));
                    StartCoroutine(Fade(oldActiveBG, oldActiveBGSprite, 1, -1));
                }
            }
        }

        // MC
        Sprite oldPopupSpeakerSprite = currentPopupMC.GetComponent<Image>().sprite;
        Sprite newPopupSpeakerSprite;

        if (!baseDialogue.character.ToString().Equals("None")) 
        {
            newPopupSpeakerSprite = spriteCache.sprites["Little" + baseDialogue.character.ToString()];
        }
        else 
        {
            // TODO: do we ever want it to be transparent
            newPopupSpeakerSprite = spriteCache.sprites["Transparent"];
        }

        
        if (!oldPopupSpeakerSprite.ToString().Equals(newPopupSpeakerSprite.ToString())) 
        {
            oldPopupMC.GetComponent<Image>().sprite = currentPopupMC.GetComponent<Image>().sprite;
            currentPopupMC.GetComponent<Image>().sprite = newPopupSpeakerSprite;
            
            // oldPopupMC.GetComponent<Image>().sprite = currentPopupMC.GetComponent<Image>().sprite;
            // StartCoroutine(Fade(currentPopupMC, newPopupMCSprite, 0, 1));
            // StartCoroutine(Fade(oldPopupMC, oldPopupMCSprite, 1, -1));
        }

        skippedFromIndex = -1;
    }

    void SetDialogue(BaseDialogueStruct baseDialogue) 
    {
        dialogueOnDisplay = baseDialogue.dialogue;

        if (baseDialogue.vnType != VNTypeEnum.CG && baseDialogue.vnType != VNTypeEnum.CGAndChoice)
        { 
            // set character name
            normalCharacterName.GetComponent<TextMeshProUGUI>().text = baseDialogue.character.GetParsedName();

            // baseDialogue.character.GetParsedName().Equals("Tubby") ?
            targetNormalDialogueWidth = 500f;

            if (normalDialogueRectTransform.sizeDelta.x != targetNormalDialogueWidth)
            {
                normalDialogueRectTransform.sizeDelta = new Vector2(targetNormalDialogueWidth, normalDialogueRectTransform.sizeDelta.y);
            }

            // set dialogue
            typeWriterCoroutine = StartCoroutine(TypeWriterEffect(baseDialogue.character.ToString(), baseDialogue.dialogue));
        }
        else 
        {
            if (baseDialogue.vnType != VNTypeEnum.CG && baseDialogue.vnType != VNTypeEnum.CGAndChoice) 
            {
                // set character name
                normalCharacterName.GetComponent<TextMeshProUGUI>().text = baseDialogue.character.GetParsedName();

                // set dialogue
                normalDialogue.GetComponent<TextMeshProUGUI>().text = baseDialogue.dialogue;
            }
        }
    }

    void SkipTypeWriterEffect(bool inCG = false) 
    {
        StopCoroutine(typeWriterCoroutine);
        TextMeshProUGUI normalDialogueTextMeshProUGUI = normalDialogue.GetComponent<TextMeshProUGUI>();
        normalDialogueTextMeshProUGUI.text = dialogueOnDisplay;
        typeWriterInEffect = false;
    }

    float GetTextSpeed() 
    {
        if (currentBaseDialogue.textSpeed != 0f) 
        {
            return currentBaseDialogue.textSpeed; // modified | can also be different for different CharacterEnum
        }
        return 0.005f; // default
    }

    IEnumerator TypeWriterEffect(string character, string dialogue)
    {
        typeWriterInEffect = true;

        TextMeshProUGUI normalDialogueTextMeshProUGUI = normalDialogue.GetComponent<TextMeshProUGUI>();
        
        for (int i = 0; i <= dialogue.Length; i++) 
        {
            normalDialogueTextMeshProUGUI.text = dialogue[..i];
            
            if (!dialogue.Equals("..."))
            {
                switch (character)
                {
                    case "KittyEmployee":
                        characterNumber = 0;
                        audioSource.pitch = 1.75f;
                        break;
                    case "Nana":
                        characterNumber = 3;
                        audioSource.pitch = 1.25f;
                        break;
                    case "Tubby":
                        characterNumber = 0;
                        audioSource.pitch = 0.85f;
                        break;
                    case "FriesManager":
                        characterNumber = 2;
                        audioSource.pitch = 2f;
                        break;
                    case "JollyWorker":
                        characterNumber = 1;
                        audioSource.pitch = 0.95f;
                        break;
                    case "SpaghettiWorker":
                        characterNumber = 1;
                        audioSource.pitch = 1.9f;
                        break;
                    case "RatWorker":
                        characterNumber = 3;
                        audioSource.pitch = 2f;
                        break;
                    // need to do this for everyone
                    default:
                        characterNumber = 0;
                        audioSource.pitch = 1.75f;
                        break;
                }

                audioSource.PlayOneShot(voices[characterNumber]);
            }

            yield return new WaitForSeconds(GetTextSpeed()); // make diff speeds
        }

        typeWriterInEffect = false;
    }
    
    // if sprite is null, indicates that it is an imageless graphic i.e. text only
    public IEnumerator Fade(GameObject currentCharacterDisplay, Sprite sprite, double fadeFrom, double fadeTo, float speed = 0.1f, bool isUIElement = false, bool getTextFromChild = false) 
    {
        double desiredOpacity = fadeTo;

        // image component
        Image characterDisplayImage = currentCharacterDisplay.GetComponent<Image>();
        Color imageColor = Color.red;
        if (characterDisplayImage != null) 
        {
            imageColor = characterDisplayImage.color;
            characterDisplayImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, imageColor.a);
            characterDisplayImage.sprite = sprite;
        }
        
        // text component
        TextMeshProUGUI characterDisplayText = currentCharacterDisplay.GetComponent<TextMeshProUGUI>();
        if (getTextFromChild) 
        {
            characterDisplayText = currentCharacterDisplay.GetComponentInChildren<TextMeshProUGUI>();
        }
        Color textColor = Color.red;
        if (characterDisplayText != null)
        {
            textColor = characterDisplayText.color;
        }

        Func<double, double, bool> lte = (a, b) => a <= b;
        Func<double, double, bool> gte = (a, b) => b <= a;

        double increment = speed;
        double until = desiredOpacity + increment;
        Func<double, double, bool> comparisonToUse = lte;

        // fading out (fading in = default)
        if (fadeFrom > fadeTo) {
            increment = -0.1f;
            until = desiredOpacity;
            comparisonToUse = gte;
        }

        // as UI elements can have varied opacity
        if (isUIElement) 
        {
            fadeFrom = imageColor.a;
        }

        for (double i = fadeFrom; comparisonToUse(i, until); i += increment) 
        {
            if (characterDisplayImage != null)
            {
                if ((isUIElement && i < increment / 2)|| !isUIElement)
                {
                    characterDisplayImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, (float) i);
                }
            }
            if (characterDisplayText != null) 
            {
                characterDisplayText.color = new Color(textColor.r, textColor.g, textColor.b, (float) i);
            }
            yield return null;
        }
    }

    public void OnClickForwardButton() 
    {
        buttonClicked = true;
    }

    public int GetDialogueIndex()
    {
        return dialogueIndex;
    }

    private IEnumerator DisableSpaceInput()
    {
        spaceDisabled = true;

        yield return new WaitForSeconds(0.60f); // TODO find the lower bound of this but for now, it works for VN glitches

        spaceDisabled = false;
    }
}
