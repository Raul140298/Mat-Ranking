using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private FromLevelSO fromLevelSO;
    [SerializeField] private SaveSystemScript saveSystem;
    [SerializeField] private Text version;

    public void StartScene()
    {
        version.text = Application.version;
        fromLevelSO.fromLevel = false;
        saveSystem.LoadOptions();
        SoundtracksScript.PlaySoundtrack("GARDEN OF MATH");
    }

    public void LoadAdventure()
    {
        StartCoroutine(CRTLoadAdventure(1f));
    }

    IEnumerator CRTLoadAdventure(float transitionTime)
    {
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}