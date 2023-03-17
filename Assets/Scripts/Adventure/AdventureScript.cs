using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class AdventureScript : MonoBehaviour
{
    [SerializeField] private TextAsset textJSON;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private FromLevelSO fromLevelSO;
    [SerializeField] private CurrentLevelSO currentLevelSO;
    [SerializeField] private SaveSystemScript saveSystem;
    [SerializeField] private GameSystemScript gameSystem;
    [SerializeField] private DialogueSystemController dialogueSystem;
    [SerializeField] private IntroScript intro;

    [SerializeField] private Text phrase;
    [SerializeField] private Text author;

    private PhraseList myPhraseList = new PhraseList();

    [System.Serializable]
    public class Phrase
    {
        public string id;
        public string autor;
        public string frase;
        public string phrase;
    }

    [System.Serializable]
    public class PhraseList
    {
        public Phrase[] phrases;
    }

    private void Awake()
    {
        myPhraseList = JsonUtility.FromJson<PhraseList>(textJSON.text);
    }

    public void StartScene()
    {
        GameSystemScript.Instance.DialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        GameSystemScript.Instance.DialogueSystem.displaySettings.inputSettings.responseTimeout = 0f;
        GameSystemScript.Instance.Timer.SetActive(false);

        StartTransition();

        gameSystem.ResetPlayerCurrentLevel();

        saveSystem.LoadLocal();

        gameSystem.SetKnowledgePoints();

        if (gameSystem.PlayerSO.tutorial == false) StartCoroutine(CRTIntro());
    }

    private void StartTransition()
    {
        //Set which animation transition show
        if (fromLevelSO.fromLevel)
        {
            transitionAnimator.SetTrigger("fromLevel");
            ResetDialogue();
        }
        else
        {
            transitionAnimator.SetTrigger("fromMenu");

            //Set text for the transition
            int n = Random.Range(0, myPhraseList.phrases.Length);
            switch (Localization.language)
            {
                case "es":
                    phrase.text = '"' + myPhraseList.phrases[n].frase + '.' + '"';
                    break;

                case "en":
                    phrase.text = '"' + myPhraseList.phrases[n].phrase + '.' + '"';
                    break;

                case "qu":
                    break;

                default:
                    // code block
                    break;
            }
            author.text = myPhraseList.phrases[n].autor;
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
        currentLevelSO.currentZone = 0;
    }

    public void SetLevelZone1()
    {
        currentLevelSO.currentZone = 1;
    }

    public void SetLevelZone2()
    {
        currentLevelSO.currentZone = 2;
    }

    public void SetLevelZone3()
    {
        currentLevelSO.currentZone = 3;
    }

    public void LoadLevel()
    {
        StartCoroutine(CRTLoadLevel());
    }

    IEnumerator CRTLoadLevel()
    {
        yield return new WaitForSeconds(0.5f);
        saveSystem.SaveLocal();
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
        saveSystem.SaveLocal();
    }
}
