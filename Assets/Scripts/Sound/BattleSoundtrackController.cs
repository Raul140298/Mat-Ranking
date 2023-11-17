using System.Collections;
using UnityEngine;

public class BattleSoundtrackController : MonoBehaviour
{
    [SerializeField] private AudioSource battleAudioSource;
    private float volume;

    void Start()
    {
        SoundtracksManager.Slider.onValueChanged.AddListener(val => ChangeVolume(val));
        battleAudioSource.volume = 0;
        volume = SoundtracksManager.Slider.value;
    }

    public void ChangeVolume(float value)
    {
        volume = value;
    }

    public void StartBattleSoundtrack()
    {
        SoundtracksManager.PlayBattleSoundtrack(
            PlayerLevelInfo.currentZone == 0 ?
            "BATTLE LAYER LEVEL ONE" :
            "BATTLE LAYER", battleAudioSource);

        StartCoroutine(CRTIncreaseVolume());
    }

    public void EndBattleSoundtrack()
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
