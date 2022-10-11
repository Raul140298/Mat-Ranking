using System.Collections;
using UnityEngine;

public class BattleSoundtrackScript : MonoBehaviour
{
    public GameSystemScript gameSystem;
    public AudioSource BattleAudioSource;
    public float volume;

    void Start()
    {
		gameSystem.soundtracksSlider.onValueChanged.AddListener(val => ChangeVolume(val));
		BattleAudioSource.volume = 0;
	}

	public void ChangeVolume(float value)
	{
		volume = value;
	}

	public void startBattleSoundtrack()
	{
		StartCoroutine(IncreaseVolume());
	}

	public void endBattleSoundtrack()
	{
		StartCoroutine(ReduceVolume());
	}

	private IEnumerator IncreaseVolume()
	{
		while (BattleAudioSource.volume < volume)
		{
			BattleAudioSource.volume += 0.02f;
			yield return null;
		}
	}

	private IEnumerator ReduceVolume()
	{
		while (BattleAudioSource.volume > 0)
		{
			BattleAudioSource.volume -= 0.02f;
			yield return null;
		}
	}
}
