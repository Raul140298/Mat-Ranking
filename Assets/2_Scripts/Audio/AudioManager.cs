using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    
    void Awake()
	{
		InitializeSingleton();
	}
	
    private void InitializeSingleton()
    {
    	if (instance == null)
    	{
    		instance = this;
    	}
    	else if (instance != this)
    	{
    		Destroy(gameObject);
    	}

    	DontDestroyOnLoad(gameObject);
    }
    
    public static void StartAudio(Slider sfx, Slider bgm)
    {
        sfx.value = PlayerSessionInfo.sfxVolume;
        sfx.onValueChanged.AddListener(val => ChangeSfxVolume(val));
        
        bgm.value = PlayerSessionInfo.bgmVolume;
        bgm.onValueChanged.AddListener(val => ChangeBgmVolume(val));
    }

    private static void ChangeSfxVolume(float val)
    {
        AudioController.SetCategoryVolume("SFX", val);
    }
    
    private static void ChangeBgmVolume(float val)
    {
        AudioController.SetCategoryVolume("BGM", val);
    }
    
    public static void FadeOutSfxVolume(float time=1)
    {
        AudioController.FadeOutCategory("SFX", time);
    }
    
    public static void FadeOutBgmVolume(float time=1)
    {
        AudioController.FadeOutCategory("BGM", time);
    }
}