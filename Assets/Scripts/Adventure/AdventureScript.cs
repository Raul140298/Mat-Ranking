using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class AdventureScript : MonoBehaviour
{
    public TextAsset textJSON;
    public Animator transitionAnimator;
    public FromLevelSO fromLevelSO;
    public CurrentLevelSO currentLevelSO;
    public SaveSystemScript saveSystem;
	public GameSystemScript gameSystem;
	private Text phrase, author;
    public Animator dialoguePanel;
    public GameObject timer;
	public DialogueSystemController dialogueSystemController;
    public IntroScript intro;

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

    public PhraseList myPhraseList = new PhraseList();

    void Start()
    {
        //Set which animation transition show
        if ( fromLevelSO.fromLevel)
		{
            transitionAnimator.SetTrigger("fromLevel");
            StartCoroutine(resetDialogue());
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

        StartCoroutine(Init());

		gameSystem.resetPlayerCurrentLevel();

        saveSystem.loadLocal();

		gameSystem.setKnowledgePoints();

        if(gameSystem.playerSO.tutorial == false) StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
		yield return new WaitForSeconds(5f);
        intro.GetComponent<OutlineScript>().OutlineOff();
		yield return new WaitForSeconds(2f);
        intro.startTutorial();
	}

    IEnumerator Init()
    {
        yield return new WaitForSeconds(0.2f);

		dialogueSystemController = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueSystemController>();
		timer = GameObject.FindGameObjectWithTag("DialogueManager").transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        timer.SetActive(false);
        dialogueSystemController.displaySettings.inputSettings.responseTimeout = 0f;

		saveSystem.downloadRemote();// -> GET jsonRemote.json if had internet
    }

    IEnumerator resetDialogue()
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
        StartCoroutine(loadLevel());
    }

    IEnumerator loadLevel()
    {
        yield return new WaitForSeconds(0.5f);
        saveSystem.saveLocal();
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    public void LoadMenu()
    {
        StartCoroutine(loadMenu());
    }

    IEnumerator loadMenu()
    {
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0); // 0: mainMenu, 1:adventure, 2:level
    }

    public void Exit()
    {
        Application.Quit();
    }

	private void OnApplicationPause()//if not -> OnDestroy()
	{
        saveSystem.saveLocal();
	}
}
