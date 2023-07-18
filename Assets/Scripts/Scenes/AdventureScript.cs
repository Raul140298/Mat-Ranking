using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEditor;

public class AdventureScript : SceneScript
{
    private static AdventureScript instance;

    [Header("UI")]
    [SerializeField] private Text knowledgePoints;
    [SerializeField] private Text phrase;
    [SerializeField] private Text author;

    [Header("PLAYER")]
    [SerializeField] AdventureInteractionsScript player;

    [Header("LEVEL ENTRY")]
    [SerializeField] GameObject levelEntry;

    [Header("INTRO")]
    [SerializeField] private IntroScript intro;

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

        GameSystemScript.StartSounds(base.soundsSlider);
        GameSystemScript.StartSoundtracks(base.soundtracksSlider);
        GameSystemScript.SaveSystem.LoadLocal(player.gameObject);

        GameSystemScript.ResetPlayerCurrentLevel();
        GameSystemScript.SetKnowledgePoints(knowledgePoints);

        //CheckRanking();

        if (GameSystemScript.PlayerSO.tutorial == false)
        {
            StartCoroutine(CRTIntro());
        }
        else
        {
            intro.gameObject.SetActive(false);
        }

        SoundtracksScript.PlaySoundtrack("ADVENTURE");
    }

    private void StartTransition()
    {
        //Set which animation transition show
        if (GameSystemScript.FromLevelSO.fromLevel
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

    public override void LoadLevel(float transitionTime = 1)
    {
        player.MakeDialoguerNonClickable();
        player.CompRendering.OutlineLocked();

        base.LoadLevel();
    }

    public void DownloadRemote()//Its called by the onClick button function on the dialogue 
    {
        GameSystemScript.SaveSystem.DownloadRemote();
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

        GameSystemScript.DialoguePanel.ResetTrigger("Hide");
        GameSystemScript.DialoguePanel.ResetTrigger("Show");

        Debug.Log("Se reseteo el dialogue");
    }

    public void SetLevelZone(int id)
    {
        SaveLocal();

        GameSystemScript.CurrentLevelSO.currentZone = id;

        LoadLevel(1);
    }

    public void ShowRanking()
    {
        GameSystemScript.ShowRanking();
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
