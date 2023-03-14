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
    [SerializeField] private Animator dialoguePanel;
    [SerializeField] private GameObject timer;
    [SerializeField] private DialogueSystemController dialogueSystemController;
    [SerializeField] private IntroScript intro;

    private Text phrase, author;
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

    void Start()
    {
        //Set which animation transition show
        if (fromLevelSO.fromLevel)
        {
            transitionAnimator.SetTrigger("fromLevel");
            StartCoroutine(CRTResetDialogue());
        }
        else
        {
            //Set text for the transition
            myPhraseList = JsonUtility.FromJson<PhraseList>(textJSON.text);
            phrase = transitionAnimator.gameObject.transform.GetChild(0).GetComponent<Text>();
            author = transitionAnimator.gameObject.transform.GetChild(1).GetComponent<Text>();
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

            transitionAnimator.SetTrigger("fromMenu");
        }

        StartCoroutine(CRTInit());

        gameSystem.resetPlayerCurrentLevel();

        saveSystem.loadLocal();

        gameSystem.setKnowledgePoints();

        if (gameSystem.PlayerSO.tutorial == false) StartCoroutine(CRTIntro());
    }

    IEnumerator CRTIntro()
    {
        yield return new WaitForSeconds(5f);
        intro.GetComponent<OutlineScript>().OutlineOff();
        yield return new WaitForSeconds(2f);
        intro.startTutorial();
    }

    IEnumerator CRTInit()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueSystemController = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueSystemController>();
        timer = GameObject.FindGameObjectWithTag("DialogueManager").transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        timer.SetActive(false);
        dialogueSystemController.displaySettings.inputSettings.responseTimeout = 0f;

        //saveSystem.downloadRemote();// -> GET jsonRemote.json if had internet
    }

    IEnumerator CRTResetDialogue()
    {
        yield return new WaitForSeconds(0.1f);
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();

        DialogueManager.StopConversation();

        dialoguePanel.ResetTrigger("Hide");
        dialoguePanel.ResetTrigger("Show");

        Debug.Log("Se reseteo el dialogue");
    }

    public void setLevelZone0()
    {
        currentLevelSO.currentZone = 0;
    }

    public void setLevelZone1()
    {
        currentLevelSO.currentZone = 1;
    }

    public void setLevelZone2()
    {
        currentLevelSO.currentZone = 2;
    }

    public void setLevelZone3()
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
        saveSystem.saveLocal();
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

    public void Exit()
    {
        Application.Quit();
    }

    public void OnApplicationPause()//if not -> OnDestroy()
    {
        saveSystem.saveLocal();
    }
}
