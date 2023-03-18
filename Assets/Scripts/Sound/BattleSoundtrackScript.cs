using System.Collections;
using UnityEngine;

public class BattleSoundtrackScript : MonoBehaviour
{
    [SerializeField] private AudioSource battleAudioSource;
    private float volume;

    void Start()
    {
        SoundtracksScript.Slider.onValueChanged.AddListener(val => ChangeVolume(val));
        battleAudioSource.volume = 0;
        volume = SoundtracksScript.Slider.value;
    }

    public void ChangeVolume(float value)
    {
        volume = value;
    }

    public void StartBattleSoundtrack()
    {
        SoundtracksScript.PlayBattleSoundtrack(
            GameSystemScript.Instance.CurrentLevelSO.currentZone == 0 ?
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
