using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionUIManager : MonoBehaviour
{
    // constants
    private const char ANSWER_A = 'A';
    private const char ANSWER_B = 'B';
    private const char ANSWER_C = 'C';
    private const int MAX_NUM_QUESTIONS = 5;

    [SerializeField] GameObject questionUI = null;
    [SerializeField] TextMeshProUGUI scenarioText = null;
    [SerializeField] TextMeshProUGUI scenarioNumberText = null;
    [SerializeField] int activeQuestionIndex = 0;
    [SerializeField] int numberOfQuestionsAnswered = 0;
    private SO_QuestionInfo currentQuestion = null;
    [SerializeField] TextMeshProUGUI aOption;
    [SerializeField] TextMeshProUGUI bOption;
    [SerializeField] TextMeshProUGUI cOption;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentQuestionIndex()
    {
        return activeQuestionIndex;
    }

    public void SetActive(bool inActive)
    {
        questionUI.SetActive(inActive);
    }

    public void SetScenarioText()
    {
        aOption.gameObject.SetActive(true);
        scenarioText.text = currentQuestion.questionText;
        scenarioNumberText.text = (currentQuestion.questionNo + 1).ToString();
        aOption.text = currentQuestion.aOption;
        bOption.text = currentQuestion.bOption;
        cOption.text = currentQuestion.cOption;
    }

    public void SetCurrentQuestion(int index)
    {
        activeQuestionIndex = index;
        GetCurrentQuestioInfo();
    }

    private void GetCurrentQuestioInfo()
    {
        currentQuestion = SpawnManager.Instance.GetQuestion(activeQuestionIndex);
    }

    public void DisplayCurrentQuestion()
    {
        questionUI.SetActive(true);
        scenarioText.text = currentQuestion.questionText;
        scenarioNumberText.text = (activeQuestionIndex + 1).ToString();
    }

    public void OnQuestionButtonClicked(char inButtonLetter)
    {
        if (currentQuestion.answer == inButtonLetter)
            DisplayCorrectText(true);
        else
            DisplayCorrectText(false);
    }

    private void DisplayCorrectText(bool inCorrect)
    {
        // Hide A,B & C buttons
        UIManager.Instance.ShowHideABCButtons(false);

        if (inCorrect)
        {
            scenarioText.text = currentQuestion.correctText;
            MainManager.Instance.UpdateScore(EScoreEvent.CORRECT_QUESTION);
        }
        else
        {
            scenarioText.text = currentQuestion.incorrectText;
        }
        ShowOptionsText(false);
        scenarioText.text += "<br><br>" + currentQuestion.InformationText;
    }

    public void ShowOptionsText(bool toggle)
    {
        aOption.gameObject.SetActive(toggle);
        bOption.gameObject.SetActive(toggle);
        cOption.gameObject.SetActive(toggle);
    }


}
