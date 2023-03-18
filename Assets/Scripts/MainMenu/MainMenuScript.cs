using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private Text version;
    [SerializeField] private Slider soundtracksSlider;
    [SerializeField] private Slider soundsSlider;

    private GameSystemScript gameSystem;

    private void Start()
    {
        gameSystem = GameSystemScript.Instance;

        gameSystem.FromLevelSO.fromLevel = false;
        gameSystem.StartSounds(soundsSlider);
        gameSystem.StartSoundtracks(soundtracksSlider);
        gameSystem.SaveSystem.LoadOptions();

        version.text = Application.version;
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