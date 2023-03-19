using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SoundtracksScript : MonoBehaviour
{
    static AudioClip mainMenuSoundtrack, adventureSoundtrack, level0Soundtrack, level1Soundtrack, level2Soundtrack, level3Soundtrack;
    static AudioClip battleSoundtrack, battleLevelOneSoundtrack;
    static AudioSource audioSrc;
    static Slider slider;

    void Awake()
    {
        bool exist = GameObject.FindGameObjectsWithTag("Soundtracks").Length > 1;

        if (exist)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
        }

        //SOUNDTRACKS
        //The names in quotes "" are the music tracks in the resources folder(without extension)mainMenuSoundtrack = Resources.Load<AudioClip>("GARDEN OF MATH");
        level0Soundtrack = Resources.Load<AudioClip>("LEVEL0");
        level1Soundtrack = Resources.Load<AudioClip>("LEVEL1");
        level2Soundtrack = Resources.Load<AudioClip>("LEVEL2");
        level3Soundtrack = Resources.Load<AudioClip>("LEVEL3");
        mainMenuSoundtrack = Resources.Load<AudioClip>("GARDEN OF MATH");
        adventureSoundtrack = Resources.Load<AudioClip>("ADVENTURE");
        battleSoundtrack = Resources.Load<AudioClip>("BATTLE LAYER");
        battleLevelOneSoundtrack = Resources.Load<AudioClip>("BATTLE LAYER LEVEL ONE");

        audioSrc = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (slider)
        {
            slider.value = GameSystemScript.OptionsSO.soundtracksVolume;
            ChangeVolume(GameSystemScript.OptionsSO.soundtracksVolume);
            slider.onValueChanged.AddListener(val => ChangeVolume(val));
        }
    }

    public static void ReduceVolume()
    {
        audioSrc.DOFade(0f, 0.5f);
    }

    public static void PlaySoundtrack(string clip2)
    {
        audioSrc.loop = true;
        audioSrc.Stop();

        switch (clip2)
        {
            case "GARDEN OF MATH":
                audioSrc.clip = mainMenuSoundtrack;
                break;
            case "ADVENTURE":
                audioSrc.clip = adventureSoundtrack;
                break;
            case "LEVEL0":
                audioSrc.clip = level0Soundtrack;
                break;
            case "LEVEL1":
                audioSrc.clip = level1Soundtrack;
                break;
            case "LEVEL2":
                audioSrc.clip = level2Soundtrack;
                break;
            case "LEVEL3":
                audioSrc.clip = level3Soundtrack;
                break;
        }

        audioSrc.Play();
    }

    public static void PlayBattleSoundtrack(string clip, AudioSource battleAudioSource)
    {
        battleAudioSource.loop = true;
        battleAudioSource.Stop();

        switch (clip)
        {
            case "BATTLE LAYER LEVEL ONE":
                battleAudioSource.clip = battleLevelOneSoundtrack;
                break;
            case "BATTLE LAYER":
                battleAudioSource.clip = battleSoundtrack;
                break;
        }

        battleAudioSource.Play();
    }

    public static void Stop()
    {
        audioSrc.Stop();
    }

    public static void ChangeVolume(float value)
    {
        audioSrc.volume = value;
    }

    public static Slider Slider
    {
        get { return slider; }
        set { slider = value; }
    }
}
