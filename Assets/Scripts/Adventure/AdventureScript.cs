using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class AdventureScript : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private IntroScript intro;
    [SerializeField] private Text phrase;
    [SerializeField] private Text author;
    [SerializeField] private Text knowledgePoints;
    [SerializeField] private Slider soundtracksSlider;
    [SerializeField] private Slider soundsSlider;


    private GameSystemScript gameSystem;

    private void Start()
    {
        gameSystem = GameSystemScript.Instance;

        gameSystem.DialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        gameSystem.DialogueSystem.displaySettings.inputSettings.responseTimeout = 0f;
        gameSystem.Timer.SetActive(false);
        gameSystem.StartSounds(soundsSlider);
        gameSystem.StartSoundtracks(soundtracksSlider);
        gameSystem.ResetPlayerCurrentLevel();
        gameSystem.SaveSystem.LoadLocal();
        gameSystem.SetKnowledgePoints(knowledgePoints);

        StartTransition();

        intro.StartIntro();
        if (gameSystem.PlayerSO.tutorial == false) StartCoroutine(CRTIntro());

        SoundtracksScript.PlaySoundtrack("ADVENTURE");
    }

    private void StartTransition()
    {
        //Set which animation transition show
        if (gameSystem.FromLevelSO.fromLevel)
        {
            transitionAnimator.SetTrigger("fromLevel");
            ResetDialogue();
        }
        else
        {
            transitionAnimator.SetTrigger("fromMenu");

            //Set text for the transition
            int n = Random.Range(0, gameSystem.MyPhraseList.phrases.Length);
            switch (Localization.language)
            {
                case "es":
                    phrase.text = '"' + gameSystem.MyPhraseList.phrases[n].frase + '.' + '"';
                    break;

                case "en":
                    phrase.text = '"' + gameSystem.MyPhraseList.phrases[n].phrase + '.' + '"';
                    break;

                case "qu":
                    break;

                default:
                    // code block
                    break;
            }
            author.text = gameSystem.MyPhraseList.phrases[n].autor;
        }
    }

    IEnumerator CRTIntro()
    {
        yield return new WaitForSeconds(5f);
        intro.GetComponent<OutlineScript>().OutlineOff();
        yield return new WaitForSeconds(2f);
        intro.StartTutorial();
    }

    private void ResetDialogue()
    {
        DialogueManager.StopConversation();

        GameSystemScript.Instance.DialoguePanel.ResetTrigger("Hide");
        GameSystemScript.Instance.DialoguePanel.ResetTrigger("Show");

        Debug.Log("Se reseteo el dialogue");
    }

    public void SetLevelZone0()
    {
        gameSystem.CurrentLevelSO.currentZone = 0;
    }

    public void SetLevelZone1()
    {
        gameSystem.CurrentLevelSO.currentZone = 1;
    }

    public void SetLevelZone2()
    {
        gameSystem.CurrentLevelSO.currentZone = 2;
    }

    public void SetLevelZone3()
    {
        gameSystem.CurrentLevelSO.currentZone = 3;
    }

    public void LoadLevel()
    {
        StartCoroutine(CRTLoadLevel());
    }

    IEnumerator CRTLoadLevel()
    {
        yield return new WaitForSeconds(0.5f);
        gameSystem.SaveSystem.SaveLocal();
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    public void LoadMenu()
    {
        StartCoroutine(CRTLoadMenu());
    }

    IEnumerator CRTLoadMenu()
    {
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0); // 0: mainMenu, 1:adventure, 2:level
    }

    public void OnApplicationPause()//if not -> OnDestroy()
    {
        gameSystem.SaveSystem.SaveLocal();
    }
}
