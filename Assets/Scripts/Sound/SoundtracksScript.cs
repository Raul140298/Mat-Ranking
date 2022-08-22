using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundtracksScript : MonoBehaviour
{
    public static AudioClip mainMenuSoundtrack, level0Soundtrack, level1Soundtrack, level2Soundtrack, level3Soundtrack;
    public OptionsSO optionsSO;
    static AudioSource audioSrc;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        //SOUNDTRACKS
        //Los nombres en comillas "" son las pistas de musica en la carpeta resources(sin extension)
        mainMenuSoundtrack = Resources.Load<AudioClip>("GARDEN OF MATH");
        level0Soundtrack = Resources.Load<AudioClip>("LEVEL0");
        level1Soundtrack = Resources.Load<AudioClip>("LEVEL1");
        level2Soundtrack = Resources.Load<AudioClip>("LEVEL2");
        level3Soundtrack = Resources.Load<AudioClip>("LEVEL3");

        audioSrc = GetComponent<AudioSource>();
     
        if (slider)
        {
            slider.value = optionsSO.soundtracksVolume;
            ChangeVolume(optionsSO.soundtracksVolume);
            slider.onValueChanged.AddListener(val => ChangeVolume(val));
        }
    }

    public void reduceVolume()
	{
        StartCoroutine(ReduceVolume());
    }

    private IEnumerator ReduceVolume()
    {
        while (audioSrc.volume > 0f)
        {
            audioSrc.volume -= 0.02f;
            yield return null;
        }
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
    public static void Stop()
    {
        audioSrc.Stop();
    }

    public static void ChangeVolume(float value)
    {
        audioSrc.volume = value;
    }
}
