using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEditor;

public class AdventureScript : SceneScript
{
    private static AdventureScript instance;

    [Header("PLAYER")]
    [SerializeField] AdventureInteractionsScript player;

    [Header("LEVEL ENTRY")]
    [SerializeField] GameObject levelEntry;

    [Header("INTRO")]
    [SerializeField] private IntroScript intro;

    [Header("UI")]
    [SerializeField] private Text phrase;
    [SerializeField] private Text author;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartTransition();

        GameSystemScript.DialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        GameSystemScript.DialogueSystem.displaySettings.inputSettings.responseTimeout = 0f;
        GameSystemScript.Timer.gameObject.SetActive(false);
        GameSystemScript.StartSounds(SoundsSlider);
        GameSystemScript.StartSoundtracks(SoundtracksSlider);
        GameSystemScript.ResetPlayerCurrentLevel();
        GameSystemScript.SaveSystem.LoadLocal(player.gameObject);
        GameSystemScript.SetKnowledgePoints(KnowledgePoints);

        //CheckRanking();

        if (GameSystemScript.PlayerSO.tutorial == false) StartCoroutine(CRTIntro());
        else intro.gameObject.SetActive(false);

        SoundtracksScript.PlaySoundtrack("ADVENTURE");
    }

    public override void LoadLevel(float transitionTime = 1)
    {
        player.GetComponent<OutlineScript>().OutlineLocked();
        levelEntry.GetComponent<OutlineScript>().OutlineLocked();

        base.LoadLevel();
    }

    public void DownloadRemote()
    {
        GameSystemScript.SaveSystem.DownloadRemote();
    }

    private void StartTransition()
    {
        //Set which animation transition show
        if (GameSystemScript.FromLevelSO.fromLevel || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            TransitionAnimator.SetTrigger("fromLevel");
            ResetDialogue();
        }
        else
        {
            TransitionAnimator.SetTrigger("fromMenu");

            //Set text for the transition
            int n = Random.Range(0, GameSystemScript.MyPhraseList.phrases.Length);
            switch (Localization.language)
            {
                case "es":
                    phrase.text = '"' + GameSystemScript.MyPhraseList.phrases[n].frase + '.' + '"';
                    break;

                case "en":
                    phrase.text = '"' + GameSystemScript.MyPhraseList.phrases[n].phrase + '.' + '"';
                    break;

                case "qu":
                    break;

                default:
                    // code block
                    break;
            }
            author.text = GameSystemScript.MyPhraseList.phrases[n].autor;
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

        GameSystemScript.DialoguePanel.ResetTrigger("Hide");
        GameSystemScript.DialoguePanel.ResetTrigger("Show");

        Debug.Log("Se reseteo el dialogue");
    }

    public void SetLevelZone0()
    {
        GameSystemScript.CurrentLevelSO.currentZone = 0;
    }

    public void SetLevelZone1()
    {
        GameSystemScript.CurrentLevelSO.currentZone = 1;
    }

    public void SetLevelZone2()
    {
        GameSystemScript.CurrentLevelSO.currentZone = 2;
    }

    public void SetLevelZone3()
    {
        GameSystemScript.CurrentLevelSO.currentZone = 3;
    }

    public void OnApplicationPause()//if not -> OnDestroy()
    {
        GameSystemScript.SaveSystem.SaveLocal();
    }

    void OnDestroy()
    {
        instance = null;
    }

    public AdventureInteractionsScript Player => player;

    public static AdventureScript Instance
    {
        get { return instance; }
    }
}
