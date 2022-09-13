using Cinemachine;
using UnityEngine;

public class CinemachineShakeScript : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float shakeTimer;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

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
				CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
			        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

				cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
			}
            else
            {
				CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
					cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

				cinemachineBasicMultiChannelPerlin.m_AmplitudeGain -= 0.1f;
			}
        }
    }
}
