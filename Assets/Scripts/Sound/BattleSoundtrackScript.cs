using System.Collections;
using UnityEngine;

public class BattleSoundtrackScript : MonoBehaviour
{
    public GameSystemScript gameSystem;
    public AudioSource battleAudioSource;
    public float volume;

    void Start()
    {
        gameSystem.SoundtracksSlider.onValueChanged.AddListener(val => ChangeVolume(val));
        battleAudioSource.volume = 0;
        volume = gameSystem.SoundtracksSlider.value;
    }

    public void ChangeVolume(float value)
    {
        volume = value;
    }

    public void startBattleSoundtrack()
    {
        SoundtracksScript.PlayBattleSoundtrack(
            gameSystem.CurrentLevelSO.currentZone == 0 ?
            "BATTLE LAYER LEVEL ONE" :
            "BATTLE LAYER", battleAudioSource);

        StartCoroutine(CRTIncreaseVolume());
    }

    public void endBattleSoundtrack()
    {
        StartCoroutine(CRTReduceVolume());
    }

    private IEnumerator CRTIncreaseVolume()
    {
        while (battleAudioSource.volume < volume)
        {
            battleAudioSource.volume += 0.01f;
            yield return null;
        }
    }

    private IEnumerator CRTReduceVolume()
    {
        while (battleAudioSource.volume > 0)
        {
            battleAudioSource.volume -= 0.01f;
            yield return null;
        }
    }
}
