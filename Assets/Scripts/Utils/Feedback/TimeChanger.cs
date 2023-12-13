using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DG.Tweening;

// ATTACH THIS TO THE CAMERA OR ANY PERMANENT OBJECT
public class TimeChanger : MonoBehaviour
{
	private static TimeChanger instance;

	private Coroutine crtTimeChange;
	private Tweener tweenTimeChange;

	private const float DEFAULT_FREEZE_TIME = 0.1f;

	void Awake()
	{
		instance = this;
	}

	public void FreezeTime(float duration = DEFAULT_FREEZE_TIME)
	{
		if (crtTimeChange != null)
		{
			StopCoroutine(crtTimeChange);
		}

		crtTimeChange = StartCoroutine(CRTFreezeTime(duration));
	}

	private IEnumerator CRTFreezeTime(float duration)
	{
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1;
	}

	public void TweenTime(float newTime, float duration, TweenCallback callback = null)
	{
		if (tweenTimeChange != null)
		{
			tweenTimeChange.Kill();
		}

		tweenTimeChange = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, newTime, duration);
		tweenTimeChange.SetUpdate(true).OnComplete(callback);
	}

	public void ResetTimeScale()
	{
		Time.timeScale = 1;
	}

	public static TimeChanger Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject goTimeChanger = new GameObject("Time Changer");
				instance = goTimeChanger.AddComponent<TimeChanger>();
				Debug.LogError("TIME CHANGER OBJECT WAS MISSING");
			}

			return instance;
		}
	}
}