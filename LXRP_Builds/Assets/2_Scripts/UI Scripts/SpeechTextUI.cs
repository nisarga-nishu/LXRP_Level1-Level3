using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class SpeechTextUI : MonoBehaviour
{
    // Intro Character Speech UI members
    [SerializeField] GameObject speechUI = null;
    [SerializeField] Button introSpeechNext = null;
    [SerializeField] TextMeshProUGUI introSpeechText = null;
    [SerializeField] Image introSpeechPortrait = null;

    [Space]
    [SerializeField] float typeSpeed = 0.09f;
    
    private int index;
    private string[] speechArray;
    private bool isTalking;

    AudioSource audioSource = null;
    Animator animatorPortrait = null;

    private void Awake()
    {
        audioSource = speechUI.GetComponent<AudioSource>();
        animatorPortrait = speechUI.GetComponent<Animator>();

        introSpeechNext.onClick.AddListener(NextSentence);
    }

    public void StartIntroSpeech(string[] speechText, Sprite characterPortrait, int inStartIndex)
    {
        index = inStartIndex;
        introSpeechPortrait.sprite = characterPortrait;
        speechArray = speechText;

        introSpeechText.text = "";
        isTalking = false;
        animatorPortrait.SetBool("isTalking", isTalking);

        speechUI.SetActive(true);

        StartCoroutine(Talk());
    }

    private void NextSentence()
    {
        if (isTalking)
            return;      

        if (index < speechArray.Length - 1)
        {
            index++;
            introSpeechText.text = "";

            StartCoroutine(Talk());
            // Play Animation
        }
        else
        {
            introSpeechText.text = "";
            speechUI.SetActive(false);
        }
    }

    IEnumerator Talk()
    {
        isTalking = true;
        //Play sound
        audioSource.Play();
        animatorPortrait.SetBool("isTalking", isTalking);

        foreach (char letter in speechArray[index].ToCharArray())
        {
            introSpeechText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTalking = false;
        animatorPortrait.SetBool("isTalking", isTalking);
        // Player animation
    }

    private void OnDestroy()
    {
        if(introSpeechNext != null)
            introSpeechNext.onClick.RemoveListener(NextSentence);
    }

}
