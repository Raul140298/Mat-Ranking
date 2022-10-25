using UnityEngine;
using UnityEngine.UI;

public class SoundsScript : MonoBehaviour
{
    public static AudioClip selectSound, changeSelectSound, levelStartSound, exclamationSound,
        lockSound, keySound, losingHeartSound, winHeartSound, winPointsSound, winPointsSequenceSound, neutralSound, positiveSound, negativeSound,
        hitSound;
    public static AudioClip mob0Sound, mob1Sound, mob2Sound, mob3Sound, mob4Sound, mob5Sound, mob6Sound, mob7Sound, mob8Sound;
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
        exclamationSound = Resources.Load<AudioClip>("EXCLAMATION");
        lockSound = Resources.Load<AudioClip>("LOCK");
        positiveSound = Resources.Load<AudioClip>("POP POSITIVE");
        negativeSound = Resources.Load<AudioClip>("POP NEGATIVE");
        neutralSound = Resources.Load<AudioClip>("POP NEUTRAL");
		winHeartSound = Resources.Load<AudioClip>("WIN HEART");
		winPointsSound = Resources.Load<AudioClip>("WIN POINTS");
		keySound = Resources.Load<AudioClip>("KEY UNLOCKING");
		winPointsSequenceSound = Resources.Load<AudioClip>("WIN POINTS SEQUENCE");
		losingHeartSound = Resources.Load<AudioClip>("LOSING HEART");
        hitSound = Resources.Load<AudioClip>("HIT");

		mob1Sound = Resources.Load<AudioClip>("MOB1");
		mob2Sound = Resources.Load<AudioClip>("MOB2");
		mob3Sound = Resources.Load<AudioClip>("MOB3");
		mob4Sound = Resources.Load<AudioClip>("MOB4");
		mob5Sound = Resources.Load<AudioClip>("MOB5");
		mob6Sound = Resources.Load<AudioClip>("MOB6");
		mob7Sound = Resources.Load<AudioClip>("MOB7");
        mob8Sound = Resources.Load<AudioClip>("MOB8");

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
            case "EXCLAMATION":
                audioSrc.PlayOneShot(exclamationSound);
                break;
            case "LOCK":
                audioSrc.PlayOneShot(lockSound);
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
			case "KEY UNLOCKING":
				audioSrc.PlayOneShot(keySound);
				break;
			case "LOSING HEART":
				audioSrc.PlayOneShot(losingHeartSound);
				break;
			case "WIN HEART":
				audioSrc.PlayOneShot(winHeartSound);
				break;
			case "WIN POINTS":
				audioSrc.PlayOneShot(winPointsSound);
				break;
			case "WIN POINTS SEQUENCE":
				audioSrc.PlayOneShot(winPointsSequenceSound);
				break;
            case "HIT":
				audioSrc.PlayOneShot(hitSound);
				break;
		}
    }

	public static void PlayEnemySound(string clip, AudioSource enemyAudioSource)
	{
		switch (clip)
		{
			case "MOB0":
				enemyAudioSource.PlayOneShot(mob0Sound);
				break;
			case "MOB1":
				enemyAudioSource.PlayOneShot(mob1Sound);
				break;
			case "MOB2":
				enemyAudioSource.PlayOneShot(mob2Sound);
				break;
			case "MOB3":
				enemyAudioSource.PlayOneShot(mob3Sound);
				break;
			case "MOB4":
				enemyAudioSource.PlayOneShot(mob4Sound);
				break;
			case "MOB5":
				enemyAudioSource.PlayOneShot(mob5Sound);
				break;
			case "MOB6":
				enemyAudioSource.PlayOneShot(mob6Sound);
				break;
			case "MOB7":
				enemyAudioSource.PlayOneShot(mob7Sound);
				break;
			case "MOB8":
				enemyAudioSource.PlayOneShot(mob8Sound);
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
