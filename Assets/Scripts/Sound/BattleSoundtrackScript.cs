using System.Collections;
using UnityEngine;

public class BattleSoundtrackScript : MonoBehaviour
{
    public GameSystemScript gameSystem;
    public AudioSource battleAudioSource;
    public float volume;

    void Start()
    {
		gameSystem.soundtracksSlider.onValueChanged.AddListener(val => ChangeVolume(val));
		battleAudioSource.volume = 0;
		volume = gameSystem.soundtracksSlider.value;
	}

	public void ChangeVolume(float value)
	{
		volume = value;
	}

	public void startBattleSoundtrack()
	{
		Debug.Log("Música de Batalla");

		SoundtracksScript.PlayBattleSoundtrack(
			gameSystem.currentLevelSO.currentZone == 0 ? 
			"BATTLE LAYER LEVEL ONE" : 
			"BATTLE LAYER", battleAudioSource);

		StartCoroutine(IncreaseVolume());
	}

	public void endBattleSoundtrack()
	{
		StartCoroutine(ReduceVolume());
	}

	private IEnumerator IncreaseVolume()
	{
		while (battleAudioSource.volume < volume)
		{
			battleAudioSource.volume += 0.002f;
			yield return null;
		}
	}

	private IEnumerator ReduceVolume()
	{
		while (battleAudioSource.volume > 0)
		{
			battleAudioSource.volume -= 0.002f;
			yield return null;
		}
	}
}
