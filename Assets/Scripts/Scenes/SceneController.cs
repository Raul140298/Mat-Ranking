using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [Header("SCENE")]
    [SerializeField] protected Animator transitionAnimator;
    [SerializeField] protected Slider soundtracksSlider;
    [SerializeField] protected Slider soundsSlider;

    public virtual void LoadMenu(float transitionTime = 1)
    {
        StartCoroutine(CRTLoadScene(0, transitionTime));
    }

    public virtual void LoadAdventure(float transitionTime = 1)
    {
        StartCoroutine(CRTLoadScene(1, transitionTime));
    }

    public virtual void LoadLevel(float transitionTime = 1)
    {
        StartCoroutine(CRTLoadScene(2, transitionTime));
    }

    IEnumerator CRTLoadScene(int nScene, float transitionTime)
    {
        ReduceVolumeSoundtracks();
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(nScene); // 0: mainMenu, 1:adventure, 2:level
    }

    public virtual void ReduceVolumeSoundtracks()
    {
        SoundtracksManager.ReduceVolume();
    }

    public virtual void PlaySound(string sound)
    {
        SoundsManager.PlaySound(sound);
    }

    public virtual void PlaySoundtrack(string soundtrack)
    {
        SoundtracksManager.PlaySoundtrack(soundtrack);
    }

    public virtual void PlayBattleSoundtrack(string soundtrack)
    {
        SoundtracksManager.PlaySoundtrack(soundtrack);
    }

    public virtual void SaveLocal(GameObject player = null)
    {
        GameManager.SaveSystem.SaveLocal(player);
    }

    public virtual void SaveOptions()
    {
        GameManager.SaveSystem.SaveOptions();
    }

    public virtual void Exit()
    {
        Application.Quit();
    }
}