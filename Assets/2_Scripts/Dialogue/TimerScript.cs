using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Tween sliderTween;

    public void StartTimer(float time)
    {
        sliderTween = slider.DOValue(0, time).OnComplete(() => DialoguePanelManager.ChooseWrongResponse());
    }

    public void StopTimer()
    {
        sliderTween.Kill();
        slider.value = 0;
    }

    public void ResetTimer()
    {
        slider.value = 1;
    }
}
