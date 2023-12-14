using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ShakeInfo
{
	public float shakeTime;
	public float shakeAmount;
	public float decreaseFactor;
}

public enum eShakeType
{
	Tiny,
	Small,
	Medium,
	Big,
	Huge
}

public class CameraShake : MonoBehaviour
{
	private static CameraShake instance;

	// IF NULL, TARGET IS THE GAMEOBJECT'S TRANSFORM
	public Transform target;

	private Vector3 originalPos;

	private bool canShake = false;

	[Header("SHAKE PRESETS")]
	public ShakeInfo tinyShakeInfo;
	public ShakeInfo smallShakeInfo;
	public ShakeInfo mediumShakeInfo;
	public ShakeInfo bigShakeInfo;
	public ShakeInfo hugeShakeInfo;

	private ShakeInfo currentShakeInfo;

	void Awake()
	{
		instance = this;

		if (target == null)
		{
			target = gameObject.transform;
		}
	}

	void Start()
	{
		originalPos = target.localPosition;
	}

	public void Shake(eShakeType type)
	{
		//if (Options.IsScreenshakeON())
		if (true)
		{
			canShake = true;
			SetCurrentShakeInfo(type);
			target.localPosition = originalPos;
		}
	}

	private void SetCurrentShakeInfo(eShakeType type)
	{
		switch (type)
		{
			case eShakeType.Tiny: currentShakeInfo = tinyShakeInfo; break;
			case eShakeType.Small: currentShakeInfo = smallShakeInfo; break;
			case eShakeType.Medium: currentShakeInfo = mediumShakeInfo; break;
			case eShakeType.Big: currentShakeInfo = bigShakeInfo; break;
			case eShakeType.Huge: currentShakeInfo = hugeShakeInfo; break;
		}
	}

	void Update()
	{
		if (canShake == true)
		{
			if (currentShakeInfo.shakeTime > 0)
			{
				Vector2 offsetPos = Random.insideUnitCircle * currentShakeInfo.shakeAmount;
				target.localPosition = originalPos + new Vector3(offsetPos.x, offsetPos.y);
				currentShakeInfo.shakeTime -= Time.unscaledDeltaTime * currentShakeInfo.decreaseFactor;
			}
			else
			{
				EndShake();
			}
		}
	}

	public void EndShake()
	{
		canShake = false;
		target.localPosition = originalPos;
	}

	public static CameraShake Instance
	{
		get { return instance; }
	}
}