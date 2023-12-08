using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEditor;

public class AdventureController : SceneController
{
    private static AdventureController instance;

    [Header("UI")]
    [SerializeField] private Text knowledgePoints;
    [SerializeField] private Text phrase;
    [SerializeField] private Text author;

    [Header("PLAYER")]
    [SerializeField] PlayerModelScript player;

    [Header("LEVEL ENTRY")]
    [SerializeField] RenderingScript levelEntry;

    [Header("INTRO")]
    [SerializeField] private IntroScript intro;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartTransition();

        GameManager.SetContinueButtonAlways();
        GameManager.DialogueSystem.displaySettings.inputSettings.responseTimeout = 0f;
        GameManager.Timer.gameObject.SetActive(false);

        GameManager.StartSounds(base.soundsSlider);
        GameManager.StartSoundtracks(base.soundtracksSlider);
        GameManager.SetKnowledgePoints(knowledgePoints);

        player.transform.position = PlayerSessionInfo.playerPosition;

        PlayerLevelInfo.ResetLevelInfo();

        if (PlayerSessionInfo.tutorial == false)
        {
            //StartCoroutine(CRTIntro());
        }
        else
        {
            intro.gameObject.SetActive(false);
        }

        SoundtracksManager.PlaySoundtrack("ADVENTURE");
    }

    private void StartTransition()
    {
        //Set which animation transition show
        if (PlayerLevelInfo.fromLevel
#if UNITY_EDITOR
            || EditorApplication.isPlayingOrWillChangePlaymode
#endif
        )
        {
            base.transitionAnimator.SetTrigger("fromLevel");
            ResetDialogue();
        }
        else
        {
            base.transitionAnimator.SetTrigger("fromMenu");

            //Set text for the transition
            int n = Random.Range(0, GameManager.MyPhraseList.phrases.Length);
            switch (Localization.language)
            {
                case "es":
                    phrase.text = '"' + GameManager.MyPhraseList.phrases[n].frase + '.' + '"';
                    break;

                case "en":
                    phrase.text = '"' + GameManager.MyPhraseList.phrases[n].phrase + '.' + '"';
                    break;

                case "qu":
                    break;
            }
            author.text = GameManager.MyPhraseList.phrases[n].autor;
        }
    }

    IEnumerator CRTIntro()
    {
        yield return new WaitForSeconds(5f);
        intro.OutlineOff();
        yield return new WaitForSeconds(2f);
        intro.StartTutorial();
    }

    private void ResetDialogue()
    {
        DialogueManager.StopConversation();

        GameManager.DialoguePanel.ResetTrigger("Hide");
        GameManager.DialoguePanel.ResetTrigger("Show");

        Debug.Log("Dialogue was reseted");
    }
    
    public void SetZone(int id)
    {
        SaveLocal();

        PlayerLevelInfo.currentZone = id;

        LoadLevel();
    }
    
    public override void LoadLevel(float transitionTime = 1)
    {
        player.MakeDialoguerNonClickable();

        player.CompRendering.OutlineOff();
        player.CompRendering.OutlineLocked();

        levelEntry.OutlineOff();
        levelEntry.OutlineLocked();

        base.LoadLevel();
    }

    public void ShowRanking()
    {
        GooglePlayManager.ShowRanking();
    }

    public void HideContinueButton()
    {
        GameManager.SetContinueButtonNever();
    }

    public void ShowContinueButton()
    {
        GameManager.SetContinueButtonAlways();
    }

    public void DownloadRemote()
    {
        //Its called by the onClick button function on the dialogue 
        GameManager.RemoteSystem.DownloadRemote();
    }

    public PlayerModelScript Player => player;

    public void OnApplicationPause()
    {
        //if not -> OnDestroy()
        PlayerSessionInfo.playerPosition = player.transform.position;
        GooglePlayManager.OpenSavedGameForSave("MatRanking");
    }

    void OnDestroy()
    {
        instance = null;
    }

    public static AdventureController Instance
    {
        get { return instance; }
    }
}
