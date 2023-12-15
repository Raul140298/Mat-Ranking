using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SFXPlayer
{
    public static void PlayBGM(eFeedbackType bgm)
    {
        if (AudioController.Instance != null)
        {
            string bgmName = bgm.ToString();

            if (AudioController.IsValidAudioID(bgmName))
            {
                AudioController.PlayAmbienceSound(bgmName);
            }
            else
            {
                Debug.LogError("BGM NOT FOUND: " + bgmName);
            }
        }
    }
    
    public static void PlaySFX(eFeedbackType sfx)
    {
        if (AudioController.Instance != null)
        {
            string sfxName = sfx.ToString();

            if (AudioController.IsValidAudioID(sfxName))
            {
                AudioController.Play(sfxName);
            }
            else
            {
                Debug.LogError("SFX NOT FOUND: " + sfxName);
            }
        }
    }
}
