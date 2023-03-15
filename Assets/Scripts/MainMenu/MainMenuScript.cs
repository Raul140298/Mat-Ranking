using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private FromLevelSO fromLevelSO;
    [SerializeField] private SaveSystemScript saveSystem;

    private void Start()
    {
        fromLevelSO.fromLevel = false;
        saveSystem.loadOptions();
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