using UnityEngine;
using UnityEngine.UI;

public class SoundsScript : MonoBehaviour
{
    public static AudioClip selectSound, changeSelectSound, levelStartSound, exclamationSound, dialogSound, neutralSound, positiveSound, negativeSound;
    public OptionsSO optionsSO;
    static AudioSource audioSrc;
    public Slider slider;

    void Start()
    {
		//SOUNDS
		//The names in quotes "" are the music tracks in the resources folder (without extension)
		selectSound = Resources.Load<AudioClip>("SELECT");
        changeSelectSound = Resources.Load<AudioClip>("CHANGE SELECT");
        levelStartSound = Resources.Load<AudioClip>("LEVEL START");
        exclamationSound = Resources.Load<AudioClip>("EXCLAMATION 2");
        dialogSound = Resources.Load<AudioClip>("EXCLAMATION");
        positiveSound = Resources.Load<AudioClip>("POP POSITIVE");
        negativeSound = Resources.Load<AudioClip>("POP NEGATIVE");
        neutralSound = Resources.Load<AudioClip>("POP NEUTRAL");

        audioSrc = GetComponent<AudioSource>();

        if (slider)
        {
            slider.value = optionsSO.soundsVolume;
            ChangeVolume(optionsSO.soundsVolume);
            slider.onValueChanged.AddListener(val => ChangeVolume(val));
        }
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "SELECT":
                audioSrc.PlayOneShot(selectSound);
                break;
            case "CHANGE SELECT":
                audioSrc.PlayOneShot(changeSelectSound);
                break;
            case "LEVEL START":
                audioSrc.PlayOneShot(levelStartSound);
                break;
            case "EXCLAMATION 2":
                audioSrc.PlayOneShot(exclamationSound);
                break;
            case "EXCLAMATION":
                audioSrc.PlayOneShot(dialogSound);
                break;
            case "POP POSITIVE":
                audioSrc.PlayOneShot(positiveSound);
                break;
            case "POP NEGATIVE":
                audioSrc.PlayOneShot(negativeSound);
                break;
            case "POP NEUTRAL":
                audioSrc.PlayOneShot(neutralSound);
                break;
        }
    }

    public static void Stop()
    {
        audioSrc.Stop();
    }

    public static void ChangeVolume(float value)
	{
        audioSrc.volume = value;
    }

    public void selectSoundButton()
	{
        PlaySound("SELECT");
	}

    public void changeSelectSoundButton()
    {
        PlaySound("CHANGE SELECT");
    }

    public void startDialogue()
    {
        PlaySound("EXCLAMATION");
    }
}
