using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SpeechTextUI))]
[RequireComponent(typeof(QuestionUIManager))]
public class UIManager : MonoBehaviour
{
    // constants
    private int MAX_NUM_RULEBOOKS = 5;

    // Singleton Members
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        speechTextComponent = GetComponent<SpeechTextUI>();
        questionComponent = GetComponent<QuestionUIManager>();
    }

    [SerializeField] TextMeshProUGUI scoreText = null;
    [SerializeField] TextMeshProUGUI LevelStatus = null;
    [SerializeField] TextMeshProUGUI GameOver = null;
    TextMeshProUGUI[] TextElements = null;
    //[SerializeField] GameObject LevelStatusObject = null;

    private Animator animatorScoreText = null;

    [SerializeField] Button resetButton = null;
    [SerializeField] Button backToMenuButton = null;

    // Character Info UI members
    [Space]
    [SerializeField] Text characterNameTxt = null;
    [SerializeField] Image portraitImage = null;
    [SerializeField] Image fullImage = null;
    [SerializeField] Image collectibleImage = null;
    [SerializeField] Text objectiveText = null;
    [SerializeField] Text updateText = null;

    // Rule UI Members
    [Space]
    [SerializeField] GameObject ruleBookUI = null;
    [SerializeField] Toggle ruleSelectionTrue = null;
    [SerializeField] Toggle ruleSelectionFalse = null;
    [SerializeField] Text ruleText = null;
    [SerializeField] Text ruleNo = null;
    [SerializeField] Button ruleOkButton = null;
    private SO_RuleInfo ruleInfo = null;
    private int numRulebooksCollected = 0;

    // Question UI Members
    [Space]
    private QuestionUIManager questionComponent = null;
    [SerializeField] Button aButton = null;
    [SerializeField] Button bButton = null;
    [SerializeField] Button cButton = null;
    [SerializeField] Button okButton = null;
    [SerializeField] List<GameObject> donuts = new List<GameObject>();
    [SerializeField] Button starButton = null;
    [SerializeField] List<GameObject> starImages = new List<GameObject>();
    [SerializeField] Button starImageCloseButton = null;
    private int currentQuestionIndex;

    private SpeechTextUI speechTextComponent;
    [SerializeField] GameObject smallMenu = null;
    private bool levelTextSet = false;

    private void Start()
    {
        if (!CheckMissingRefs())
            return;

        animatorScoreText = scoreText.gameObject.GetComponent<Animator>();

        resetButton.onClick.AddListener(OnResetButtonClicked);
        backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);

        ruleSelectionTrue.onValueChanged.AddListener(OnRuleSelectionTrue);
        ruleSelectionFalse.onValueChanged.AddListener(OnRuleSelectionFalse);
        ruleOkButton.onClick.AddListener(OnRuleOKClicked);
        aButton.onClick.AddListener(OnQuestionAClicked);
        bButton.onClick.AddListener(OnQuestionBClicked);
        cButton.onClick.AddListener(OnQuestionCClicked);
        okButton.onClick.AddListener(OnOkButtonClicked);
        starButton.onClick.AddListener(OnStarButtonClicked);
        starImageCloseButton.onClick.AddListener(OnStarButtonCloseClicked);


        TextElements = FindObjectsOfType<TextMeshProUGUI>();
        //smallMenu = GameObject.FindGameObjectWithTag("menu");
        //smallMenu.SetActive(false);
        
    }

    private void OnStarButtonClicked()
    {
        currentQuestionIndex = questionComponent.GetCurrentQuestionIndex();

        starImages[currentQuestionIndex].SetActive(true);
        starImageCloseButton.gameObject.SetActive(true);
    }

    private void OnStarButtonCloseClicked()
    {
        starImages[currentQuestionIndex].SetActive(false);
        starImageCloseButton.gameObject.SetActive(false);
    }



    private void OnRuleSelectionTrue(bool isSlected)
    {
        MainManager.Instance.OnRuleSelect(isSlected, ruleInfo, true);
    }

    private void OnRuleSelectionFalse(bool isSlected)
    {
        MainManager.Instance.OnRuleSelect(isSlected, ruleInfo, false);
    }

    public bool AllRuleBooksCollected()
    {
        return numRulebooksCollected == MAX_NUM_RULEBOOKS;
    }

    public void UpdateRulebookText(string inText)
    {
        ruleText.text = inText;
        ruleSelectionFalse.gameObject.SetActive(false);
        ruleSelectionTrue.gameObject.SetActive(false);
        ruleOkButton.gameObject.SetActive(true);
    }

    public void OnRuleOKClicked()
    {
        ruleText.text = "";
        ResetRulebookUI();
        ruleBookUI.gameObject.SetActive(false);
        if (MainManager.Instance.GetNumSelectedRules() == MAX_NUM_RULEBOOKS)
            MainManager.Instance.SetState(EGameState.QUEST_COMPLETE);
    }

    private void ResetRulebookUI()
    {
        ruleOkButton.gameObject.SetActive(false);
        ruleSelectionTrue.gameObject.SetActive(true);
        ruleSelectionTrue.isOn = false;
        ruleSelectionFalse.gameObject.SetActive(true);
        ruleSelectionFalse.isOn = false;
        starButton.gameObject.SetActive(false);
    }

    private void OnQuestionAClicked()
    {
        Debug.Log("A Clicked");
        questionComponent.OnQuestionButtonClicked('A');
    }

    private void OnQuestionBClicked()
    {
        Debug.Log("B Clicked");
        questionComponent.OnQuestionButtonClicked('B');

    }

    private void OnQuestionCClicked()
    {
        Debug.Log("C Clicked");
        questionComponent.OnQuestionButtonClicked('C');
    }

    private void OnOkButtonClicked()
    {
        HideQuestionUI();
    }

    private void OnDestroy()
    {
        if (resetButton != null)
            resetButton.GetComponent<Button>().onClick.RemoveListener(OnResetButtonClicked);

        if (backToMenuButton != null)
            backToMenuButton.GetComponent<Button>().onClick.RemoveListener(OnBackToMenuButtonClicked);

        if (ruleSelectionTrue != null)
            ruleSelectionTrue.GetComponent<Toggle>().onValueChanged.RemoveListener(OnRuleSelectionTrue);

        if (ruleSelectionFalse != null)
            ruleSelectionFalse.GetComponent<Toggle>().onValueChanged.RemoveListener(OnRuleSelectionFalse);
    }

    #region MainMenuUI Methods

    // Enact reset game actions when selected
    private void OnResetButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Enact back to menu actions when selected
    private void OnBackToMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    // Display intro/outro message when starting/ending a level
    public void DisplayLevelStatusMessage(EGameState inLevelState, EMissionType inMission)
    {
        if (!levelTextSet)
        {
            //LevelStatus = getTextElement("LevelMessage");
            LevelStatus.gameObject.SetActive(true);
            levelTextSet = true;
        }
        int level = (int)inMission;

        if (inLevelState == EGameState.QUEST_START)
        {
            LevelStatus.SetText("Level " + ++level);
        }
        else if (inLevelState == EGameState.QUEST_COMPLETE)
        {
            LevelStatus.SetText("Level " + ++level + " Completed");
        }
    }

    // Obtain specified text element
    public TextMeshProUGUI getTextElement(string inLabel)
    {
        TextMeshProUGUI result = null;

        GameObject obj = GameObject.FindGameObjectWithTag(inLabel);
        result = obj.GetComponent<TextMeshProUGUI>();
        Debug.Log("RESULT: " + result);
        return result;
    }

    // Initialise "Game Over" message, set to invisible
    public void InitGameOverMessage()
    {
        //GameOver = getTextElement("GameOver");
        GameOver.gameObject.SetActive(false);
    }
    
    // Display "Game Over" message for 3 seconds
    public void DisplayGameOverMessage()
    {
        GameOver.text = "Game Over!";
        GameOver.gameObject.SetActive(true);
        StartCoroutine(DoGameOverDelay());
    }

    // Display "Game Finished" for 3 seconds
    public void DisplayGameFinishedMessage()
    {
        HideLevelStatusText();
        GameOver.text = "Game Finished!";
        GameOver.gameObject.SetActive(true);
        StartCoroutine(DoGameOverDelay());
    }

    // Coroutine to switch "Game Over" message off and show end game menu
    private IEnumerator DoGameOverDelay()
    {
        yield return new WaitForSeconds(3.0f);
        smallMenu.SetActive(true);
        GameOver.gameObject.SetActive(false);
    }

    // Initialise "Level" message, set to invisible
    public void HideLevelStatusText()
    {
        LevelStatus.gameObject.SetActive(false);
        LevelStatus.text = "level";
    }

    // Display "Level" message
    public void ShowLevelStatusText()
    {
        LevelStatus.gameObject.SetActive(true);
    }

    public void SetRuleInfo(SO_RuleInfo info)
    {
        ruleInfo = info;
        ruleNo.text = "#" + ruleInfo.ruleNo;
        ruleText.text = ruleInfo.ruleText;
        ruleSelectionTrue.isOn = ruleInfo.IsSelected;
        ResetRulebookUI();
        ruleBookUI.SetActive(true);
    }

    public void SetPlayerInfo(SO_PlayerInfo info)
    {
        characterNameTxt.text = info.characterName;
        portraitImage.sprite = info.portraitImage;
        fullImage.sprite = info.fullImage;
        collectibleImage.sprite = info.collectibleImage;
        objectiveText.text = info.objectivesText;
       
        UpdateRules(0);
    }

    public void StartIntroSpeech(string[] speechText, Sprite characterPortrait, int startIndex)
    {
        speechTextComponent.StartIntroSpeech(speechText, characterPortrait, startIndex);
    }

    public void UpdateRules(int no)
    {
        string text = no + " / "
            + SpawnManager.Instance.getNoOfSpawns(ESpawnSelection.RULES)
            + " Rules Selected";
        updateText.text = text;
    }

    public void UpdateScore(int score)
    {
        Debug.Log("UI Manager Update Score");
        scoreText.text = score.ToString();
        animatorScoreText.SetBool("isScoreEvent", true);
    }

    public void ShowQuestionUI(int inIndex)
    {
        questionComponent.SetActive(true);
        questionComponent.SetCurrentQuestion(inIndex);
        SetScenarioText();
        HideDonut(inIndex);
    }

    public void HideQuestionUI()
    {
        questionComponent.SetActive(false);
    }

    public void SetScenarioText()
    {
        questionComponent.SetScenarioText();
    }

    public void ShowHideABCButtons(bool toggle)
    {
        aButton.gameObject.SetActive(toggle);
        bButton.gameObject.SetActive(toggle);
        cButton.gameObject.SetActive(toggle);
        okButton.gameObject.SetActive(!toggle);
        starButton.gameObject.SetActive(!toggle);
    }

     public void ShowDonuts(bool show)
    {
        foreach (GameObject d in donuts)
        {
            d.gameObject.SetActive(show);
        }
    }

    public void HideDonut(int index)
    {
        donuts[index].gameObject.SetActive(false);
    }

    private bool CheckMissingRefs()
    {
        if (resetButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'resetButton'");
            return false;
        }
        if (backToMenuButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'backToMenuButton'");
            return false;
        }
        return true;
    }
}
