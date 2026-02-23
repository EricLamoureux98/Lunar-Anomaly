using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object; // What is this?

[RequireComponent(typeof(TMP_Text))]

public class Typewriter : MonoBehaviour
{
    TMP_Text textBox;

    [Header("Test String")]
    [SerializeField][TextArea(10,1)] string testText;

    // Typewriter functionality
    int currentVisibleCharacterIndex;
    Coroutine typewriterCoroutine;
    bool readyForNewText = true;
    
    WaitForSeconds delay;
    WaitForSeconds interpunctuationDelayWait;

    [Header("Typewriter Settings")]
    [SerializeField] float charactersPerSecond = 20f;
    [SerializeField] float interpunctuationDelay = 0.5f;

    // Skipping functionality
    public bool CurrentlySkipping { get; private set; }
    WaitForSeconds skipDelay;

    [Header("Skip Options")]
    [SerializeField] bool quickSkip;
    [SerializeField] [Min(1)] int skipSpeedup = 5; // <--- This is cool

    // Event Functionality
    WaitForSeconds textboxFulEventDelay;
    [SerializeField] [Range(0.1f, 0.5f)] float sendDoneDelay = 0.25f;

    public static event Action CompleteTextRevealed;
    public static event Action<char> CharacterRevealed;

    void Awake()
    {
        textBox = GetComponent<TMP_Text>();

        delay = new WaitForSeconds(1 / charactersPerSecond);
        interpunctuationDelayWait = new WaitForSeconds(interpunctuationDelay);

        skipDelay = new WaitForSeconds(1 / (charactersPerSecond * skipSpeedup));
        textboxFulEventDelay = new WaitForSeconds(sendDoneDelay);
    }

    void OnEnable()
    {
        // What is this?
        //TMPro_EventManager.TEXT_CHANGED_EVENT.Add(PrepareForNewText);
    }

    void OnDisable()
    {
        //TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(PrepareForNewText);
    }

    // What is this? 
    // public void PrepareForNewText(UnityEngine.Object obj)
    // {
    //     if (!readyForNewText) return;

    //     readyForNewText = false;

    //     if (typewriterCoroutine != null)
    //     {
    //         StopCoroutine(typewriterCoroutine);
    //     }


    //     textBox.maxVisibleCharacters = 0;
    //     currentVisibleCharacterIndex = 0;

    //     typewriterCoroutine = StartCoroutine(TypewriterText());
    // }

    void Start()
    {
        SetText(testText);
    }

    public void SetText(string text)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }
        
        textBox.text = text;
        textBox.maxVisibleCharacters = 0;
        currentVisibleCharacterIndex = 0;

        typewriterCoroutine = StartCoroutine(TypewriterText());
    }
    

    IEnumerator TypewriterText()
    {
        TMP_TextInfo textInfo = textBox.textInfo;

        while (currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            var lastCharacterIndex = textInfo.characterCount - 1;

            if (currentVisibleCharacterIndex == lastCharacterIndex)
            {
                textBox.maxVisibleCharacters++;
                yield return textboxFulEventDelay;
                CompleteTextRevealed?.Invoke();
                readyForNewText = true;
                yield break;
            }

            char character = textInfo.characterInfo[currentVisibleCharacterIndex].character;

            textBox.maxVisibleCharacters++;

             if (!CurrentlySkipping && (character == '?' || character == '.' || character == ',' || character == ':' || character == ';' || character == '!' || character == '-'))
            {
                yield return interpunctuationDelayWait;
            }
            else
            {
                // yield return CurrentlySkipping ? skipDelay : delay;
                if (CurrentlySkipping) 
                    yield return skipDelay;
                else 
                    yield return delay;
            }

            CharacterRevealed?.Invoke(character);
            currentVisibleCharacterIndex++;
        }
    }

    void Skip()
    {
        if (CurrentlySkipping) return;

        CurrentlySkipping = true;

        if (!quickSkip)
        {
            StartCoroutine(SkipSpeedupReset());
            return;
        }

        StopCoroutine(typewriterCoroutine);
        textBox.maxVisibleCharacters = textBox.textInfo.characterCount;
        readyForNewText = true;
        CompleteTextRevealed?.Invoke();
    }

    IEnumerator SkipSpeedupReset()
    {
        // How does this work? 
        yield return new WaitUntil(() => textBox.maxVisibleCharacters == textBox.textInfo.characterCount -1);
        CurrentlySkipping = false;
    }

    void ReadInput()
    {
        
    }
}
