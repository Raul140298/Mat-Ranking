using Cinemachine;
using UnityEngine;

public class CinemachineShakeScript : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
	private float shakeTimer;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
		cinemachineBasicMultiChannelPerlin =
			cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
	}

    public void ShakeCamera(float intensity, float time)
    {
    	cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
	}

    void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0f)
            {
				//Time over
				cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
			}
            else
            {
				cinemachineBasicMultiChannelPerlin.m_AmplitudeGain -= 0.1f;
			}
        }
    }
}
