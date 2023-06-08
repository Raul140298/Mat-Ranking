// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Add this to your dialogue UI if you want the sequencer {{end}} keyword to
    /// account for Text Animator typing time.
    /// </summary>
    public class TextAnimatorEndKeyword : MonoBehaviour
    {
        
        public float waitForNormalChars = .03f;
        public float waitLong = .6f;
        public float waitMiddle = .2f;

        protected virtual void Awake()
        {
            ConversationView.overrideGetDefaultSubtitleDuration = GetTextAnimatorSubtitleDuration;
        }

        protected virtual float GetTextAnimatorSubtitleDuration(string text)
        {
            int numMiddle = 0;
            int numLong = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == ';' || c == ':' || c == '-' || c == ',') numMiddle++;
                else if (c == '!' || c == '?' || c == '.') numLong++;
            }
            int numNormal = text.Length - (numMiddle + numLong);
            return (waitForNormalChars * numNormal) + 
                (waitMiddle * numMiddle) + 
                (waitLong * numLong);
        }

    }
}
