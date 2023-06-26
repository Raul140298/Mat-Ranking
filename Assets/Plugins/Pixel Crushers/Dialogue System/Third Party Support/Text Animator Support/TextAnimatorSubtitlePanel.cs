// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Use this subclass of StandardUISubtitlePanel if any of these are true:
    /// - Your subtitle panel uses Text Animator and Accumulate Text is ticked.
    /// - You want to the sequencer to receive "Typed" messages when Text Animator finishes typing.
    /// </summary>
    public class TextAnimatorSubtitlePanel : StandardUISubtitlePanel
    {
        public bool clearTextOnOpen = false;

        protected override void Start()
        {
            base.Start();
            var textAnimatorPlayer = subtitleText.gameObject.GetComponent<Febucci.UI.Core.TypewriterCore>();
            if (textAnimatorPlayer != null)
            {
                textAnimatorPlayer.onTextShowed.AddListener(OnTextShowed);
            }
        }

        public override void Open()
        {
            if (!isOpen) ClearText();
            base.Open();
        }

        public override void SetContent(Subtitle subtitle)
        {
            if (accumulateText)
            {
                var previousChars = subtitleText.textMeshProUGUI.textInfo.characterCount;
                StartCoroutine(SkipTypewriterAhead(previousChars));
            }
            base.SetContent(subtitle);
        }

        protected IEnumerator SkipTypewriterAhead(int numChars)
        {
            var textAnimator = subtitleText.gameObject.GetComponent<Febucci.UI.Core.TAnimCore>();
            if (textAnimator != null)
            {
                yield return null;
                for (int i = 0; i < numChars; i++)
                {
                    textAnimator.maxVisibleCharacters++;
                }
            }
        }

        protected void OnTextShowed()
        {
            Sequencer.Message(SequencerMessages.Typed);
        }

    }
}
