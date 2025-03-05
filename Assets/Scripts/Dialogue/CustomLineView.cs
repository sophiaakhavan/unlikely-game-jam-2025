using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Yarn.Unity
{
    public static class EffectsUpdated
    {
        public static IEnumerator Typewriter(TextMeshProUGUI text, float lettersPerSecond, Action onCharacterTyped, Effects.CoroutineInterruptToken stopToken = null)
        {
            stopToken?.Start();

            // Start with everything invisible
            text.maxVisibleCharacters = 0;

            // Wait a single frame to let the text component process its content, otherwise characterCount info won't be accurate
            yield return null;

            // How many visible characters are present in the text?
            var characterCount = text.textInfo.characterCount;

            // Early out if letter speed is zero, text length is zero
            if (lettersPerSecond <= 0 || characterCount == 0)
            {
                // Show everything and return
                text.maxVisibleCharacters = characterCount;
                stopToken?.Complete();
                yield break;
            }

            // Convert 'letters per second' into its inverse
            float secondsPerLetter = 1.0f / lettersPerSecond;

            // If lettersPerSecond is larger than the average framerate, we need to show more than one letter per frame
            // We'll accumulate time every frame and display as many letters in that frame as we need to achieve the requested speed
            var accumulator = Time.deltaTime;

            while (text.maxVisibleCharacters < characterCount)
            {
                if (stopToken?.WasInterrupted ?? false)
                {
                    yield break;
                }

                // Show as many letters as we have accumulated time for
                while (accumulator >= secondsPerLetter)
                {
                    text.maxVisibleCharacters += 1;
                    onCharacterTyped?.Invoke();
                    accumulator -= secondsPerLetter;

                    // Don't pause on the last character
                    if (text.maxVisibleCharacters >= characterCount) continue;

                    // Pause on punctuation
                    if (text.text[text.maxVisibleCharacters - 1].Equals('.') ||
                        text.text[text.maxVisibleCharacters - 1].Equals(',') ||
                        text.text[text.maxVisibleCharacters - 1].Equals('?') ||
                        text.text[text.maxVisibleCharacters - 1].Equals('!'))
                    {
                        if (!text.text[text.maxVisibleCharacters].Equals(' '))
                        {
                            continue;
                        }

                        yield return new WaitForSeconds(0.2f);
                    }
                }

                accumulator += Time.deltaTime;

                yield return null;
            }

            // We either finished displaying everything or were interrupted -- display everything now
            text.maxVisibleCharacters = characterCount;

            stopToken?.Complete();
        }
    }

    public class CustomLineView : DialogueViewBase
    {
        [SerializeField] internal CanvasGroup canvasGroup;
        [SerializeField] internal bool useFadeEffect = true;
        [SerializeField][Min(0)] internal float fadeInTime = 0.25f;
        [SerializeField][Min(0)] internal float fadeOutTime = 0.05f;

        [SerializeField] internal AudioSource sound = null;
        [SerializeField] internal AudioSource click = null;

        [SerializeField] internal TextMeshProUGUI lineText = null;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("showCharacterName")]
        internal bool showCharacterNameInLineView = true;

        [SerializeField] internal TextMeshProUGUI characterNameText = null;

        [SerializeField] internal bool useTypewriterEffect = false;
        [SerializeField] internal UnityEngine.Events.UnityEvent onCharacterTyped;
        [SerializeField][Min(0)] internal float typewriterEffectSpeed = 0f;
        [SerializeField][Min(0)] internal int talkingSpeed;

        [SerializeField] internal GameObject continueButton = null;

        [SerializeField][Min(0)] internal float holdTime = 1f;
        [SerializeField] internal bool autoAdvance = false;

        LocalizedLine currentLine = null;

        Effects.CoroutineInterruptToken currentStopToken = new Effects.CoroutineInterruptToken();

        private bool isOpen = false;

        private void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        private void Reset()
        {
            canvasGroup = GetComponentInParent<CanvasGroup>();
        }

        public override void DismissLine(Action onDismissalComplete)
        {
            click.Play();
            currentLine = null;
            StartCoroutine(DismissLineInternal(onDismissalComplete));
        }

        private IEnumerator DismissLineInternal(Action onDismissalComplete)
        {
            // Disable interaction temporarily while dismissing the line
            var interactable = canvasGroup.interactable;
            canvasGroup.interactable = false;

            yield return new WaitForSeconds(0.1f);
            currentStopToken.Complete();

            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            // Turn interaction back on, if needed
            canvasGroup.interactable = interactable;
            onDismissalComplete();
        }

        public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished)
        {
            currentLine = dialogueLine;

            // Cancel all coroutines that we're currently running, including RunLineInternal
            StopAllCoroutines();

            // Immediately show everything
            lineText.gameObject.SetActive(true);
            canvasGroup.gameObject.SetActive(true);

            int length;

            if (characterNameText == null)
            {
                if (showCharacterNameInLineView)
                {
                    lineText.text = dialogueLine.Text.Text;
                    length = dialogueLine.Text.Text.Length;
                }
                else
                {
                    lineText.text = dialogueLine.TextWithoutCharacterName.Text;
                    length = dialogueLine.TextWithoutCharacterName.Text.Length;
                }
            }
            else
            {
                characterNameText.text = dialogueLine.CharacterName;
                lineText.text = dialogueLine.TextWithoutCharacterName.Text;
                length = dialogueLine.TextWithoutCharacterName.Text.Length;
            }

            // Show the entire line's text immediately
            lineText.maxVisibleCharacters = length;

            // Make the canvas group fully visible immediately
            canvasGroup.alpha = 1;

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            onInterruptLineFinished();
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // Stop any coroutines currently running on this line view
            StopAllCoroutines();

            // Begin running the line as a coroutine
            StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
        }

        private IEnumerator RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            IEnumerator PresentLine()
            {
                lineText.gameObject.SetActive(true);
                canvasGroup.gameObject.SetActive(true);

                // Hide continue button until presentation is complete 
                if (continueButton != null)
                {
                    continueButton.SetActive(false);
                }

                if (characterNameText != null)
                {
                    characterNameText.text = dialogueLine.CharacterName;
                    lineText.text = dialogueLine.TextWithoutCharacterName.Text;
                }
                else
                {
                    // If we don't have a character name text view...
                    if (showCharacterNameInLineView)
                    {
                        // Show the entire text
                        lineText.text = dialogueLine.Text.Text;
                    }
                    else
                    {
                        // Show just the text without the character name
                        lineText.text = dialogueLine.TextWithoutCharacterName.Text;
                    }
                }

                if (useTypewriterEffect)
                {
                    // If we're using the typewriter effect, hide all of the text before we begin any possible fade
                    lineText.maxVisibleCharacters = 0;
                }
                else
                {
                    // Ensure that the max visible characters is effectively unlimited
                    lineText.maxVisibleCharacters = int.MaxValue;
                }

                // Fade in effect if dialogue view was not visible before
                if (!isOpen)
                {
                    isOpen = true;

                    Debug.Log("Fading in");
                    yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeInTime, currentStopToken));
                    if (currentStopToken.WasInterrupted)
                    {
                        // Fade effect was interrupted, stop this entire coroutine
                        yield break;
                    }
                }

                // If we're using the typewriter effect, start it and wait fo it to finish
                if (useTypewriterEffect)
                {
                    // Set canvas back to defaults
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                    yield return StartCoroutine(
                        EffectsUpdated.Typewriter(
                            lineText,
                            typewriterEffectSpeed,
                            () => onCharacterTyped.Invoke(),
                            currentStopToken
                        )
                    );
                    if (currentStopToken.WasInterrupted)
                    {
                        // Typewriter effect was interrupted
                        yield break;
                    }
                }
            }
            currentLine = dialogueLine;

            // Run any presentations as a single coroutine
            yield return StartCoroutine(PresentLine());

            currentStopToken.Complete();

            // All of our text should now be visible
            lineText.maxVisibleCharacters = int.MaxValue;

            // Our view should at be at full opacity
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            // Show the continue button, if we have one
            if (continueButton != null)
            {
                continueButton.SetActive(true);
            }

            // If we have a hold time, wait that amount of time, and then continue
            if (holdTime > 0)
            {
                yield return new WaitForSeconds(holdTime);
            }

            if (autoAdvance == false)
            {
                // Don't call completion handler, wait for user advancement
                yield break;
            }

            // Our presentation is complete; call the completion handler
            onDialogueLineFinished();
        }

        public override void DialogueComplete()
        {
            StartCoroutine(EndDialogue());
        }

        public IEnumerator EndDialogue()
        {
            if (isOpen)
            {
                yield return StartCoroutine(CloseDialogue());
            }
        }

        public IEnumerator CloseDialogue()
        {
            isOpen = false;

            yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeOutTime, currentStopToken));
            currentStopToken.Complete();
        }

        public void OnCharacterTyped()
        {
            if (lineText.maxVisibleCharacters > 0)
            {
                if (lineText.maxVisibleCharacters % talkingSpeed == 1)
                {
                    if (!lineText.text[lineText.maxVisibleCharacters - 1].Equals('.') &&
                        !lineText.text[lineText.maxVisibleCharacters - 1].Equals(',') &&
                        !lineText.text[lineText.maxVisibleCharacters - 1].Equals('?') &&
                        !lineText.text[lineText.maxVisibleCharacters - 1].Equals('!'))
                    {
                        // If not on punctuation, play randomly pitched 'dialgue' sfx
                        if (sound.clip != null)
                        {
                            sound.pitch = UnityEngine.Random.Range(1f, 1.2f);
                            sound.Play();
                        }
                    }
                }
            }
        }

        public override void UserRequestedViewAdvancement()
        {
            // We received a request to advance the view. If we're in the middle of
            // an animation, skip to the end of it. If we're not current in an
            // animation, interrupt the line so we can skip to the next one.

            // We have no line, so the user just mashed randomly
            if (currentLine == null)
            {
                return;
            }

            // Is an animation running that we can stop?
            if (currentStopToken.CanInterrupt)
            {
                // Stop the current animation and skip to the end
                currentStopToken.Interrupt();
            }
            else
            {
                // No animation is now running, interrupt the line instead
                requestInterrupt?.Invoke();
            }
        }

        public void OnContinueClicked()
        {
            UserRequestedViewAdvancement();
        }
    }
}
