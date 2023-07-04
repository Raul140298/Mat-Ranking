using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneScript : MonoBehaviour
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
        SoundtracksScript.ReduceVolume();
    }

    public virtual void PlaySound(string sound)
    {
        SoundsScript.PlaySound(sound);
    }

    public virtual void PlaySoundtrack(string soundtrack)
    {
        SoundtracksScript.PlaySoundtrack(soundtrack);
    }

    public virtual void PlayBattleSoundtrack(string soundtrack)
    {
        SoundtracksScript.PlaySoundtrack(soundtrack);
    }

    public virtual void SaveLocal(GameObject player = null)
    {
        GameSystemScript.SaveSystem.SaveLocal(player);
    }

    public virtual void SaveOptions()
    {
        GameSystemScript.SaveSystem.SaveOptions();
    }

    public virtual void Exit()
    {
        Application.Quit();
    }
}