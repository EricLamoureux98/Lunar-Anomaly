using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LunarAnomaly.UI
{
    //[RequireComponent(typeof(TMP_Text))]

    public class Typewriter : MonoBehaviour
    {
        TMP_Text textBox;

        [Header("Test String")]
        [SerializeField][TextArea(10,1)] string testText;

        // Typewriter functionality
        int currentVisibleCharacterIndex;
        Coroutine typewriterCoroutine;
        
        WaitForSeconds delay;
        WaitForSeconds interpunctuationDelayWait;

        [Header("Typewriter Settings")]
        [SerializeField] float charactersPerSecond = 26f;
        [SerializeField] float interpunctuationDelay = 0.4f;

        // Skipping functionality
        public bool CurrentlySkipping { get; private set; }
        WaitForSeconds skipDelay;

        [Header("Skip Options")]
        [SerializeField] bool quickSkip;
        [SerializeField] [Min(1)] int skipSpeedupMultiplier = 5; // <--- This is cool

        // Event Functionality
        WaitForSeconds textboxFullEventDelay;
        [SerializeField] [Range(0.1f, 0.5f)] float sendDoneDelay = 0.25f;

        // To NotificationController
        public static event Action OnCompleteTextRevealed;

        void Awake()
        {
            //textBox = GetComponent<TMP_Text>();

            // Cached for performance        
            UpdateDelays();
            interpunctuationDelayWait = new WaitForSeconds(interpunctuationDelay);
            textboxFullEventDelay = new WaitForSeconds(sendDoneDelay);
        }

        void OnValidate()
        {
            UpdateDelays();
        }

        void UpdateDelays()
        {
            delay = new WaitForSeconds(1 / charactersPerSecond);
            skipDelay = new WaitForSeconds(1 / (charactersPerSecond * skipSpeedupMultiplier));
        }

        void Start()
        {
            //SetText(testText);
        }

        public void SetText(string text, TMP_Text currentTextBox)
        {          
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            if (text == null || currentTextBox == null) return;
            textBox = currentTextBox;
            
            textBox.text = text;
            textBox.maxVisibleCharacters = 0;
            currentVisibleCharacterIndex = 0;

            typewriterCoroutine = StartCoroutine(TypewriterText());
        }     

        public void SetTextInstant(string text, TMP_Text currentTextBox)
        {           
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            if (text == null || currentTextBox == null) return;
            textBox = currentTextBox;

            textBox.text = text;
            //textBox.maxVisibleCharacters = textBox.textInfo.characterCount;
            textBox.maxVisibleCharacters = int.MaxValue;
        }   

        IEnumerator TypewriterText()
        {
            // Ensure characterCount is accurate
            textBox.ForceMeshUpdate();
            TMP_TextInfo textInfo = textBox.textInfo;

            //while (currentVisibleCharacterIndex < textInfo.characterCount + 1)
            while (currentVisibleCharacterIndex < textInfo.characterCount)
            {
                var lastCharacterIndex = textInfo.characterCount - 1;

                if (currentVisibleCharacterIndex == lastCharacterIndex)
                {
                    textBox.maxVisibleCharacters++;
                    yield return textboxFullEventDelay;
                    OnCompleteTextRevealed?.Invoke();
                    yield break;
                }

                char character = textInfo.characterInfo[currentVisibleCharacterIndex].character;

                textBox.maxVisibleCharacters++;
                // Fix later. Needs to know when to stop
                SoundManager.PlaySound(SoundType.Typewriter, 0.1f, false);

                //if (!CurrentlySkipping && (character == '?' || character == '.' || character == ',' || character == ':' || character == ';' || character == '!' || character == '-'))
                if (!CurrentlySkipping && (character == '?' || character == '.' || character == ',' || character == ':' || character == ';' || character == '!'))
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

                currentVisibleCharacterIndex++;
            }
        }

        public void Stop()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                currentVisibleCharacterIndex = 0;
            }
        }

        public void Skip()
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
            OnCompleteTextRevealed?.Invoke();
        }

        IEnumerator SkipSpeedupReset()
        {
            // Pause coroutine until the condition becomes true
            yield return new WaitUntil(() => textBox.maxVisibleCharacters == textBox.textInfo.characterCount -1);
            // Then sets back to false
            CurrentlySkipping = false;
        }
    }
}