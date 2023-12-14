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
        ResetDialogue();

        DialoguePanelManager.SetContinueButtonAlways();
        DialogueManager.displaySettings.inputSettings.responseTimeout = 0f;
        DialoguePanelManager.Timer.gameObject.SetActive(false);

        AudioManager.StartAudio(sfxSlider, bgmSlider);
        SetKnowledgePoints(knowledgePoints);

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

        Feedback.Do(eFeedbackType.Adventure);
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

        DialoguePanelManager.DialoguePanel.ResetTrigger("Hide");
        DialoguePanelManager.DialoguePanel.ResetTrigger("Show");

        Debug.Log("Dialogue was reseted");
    }
    
    public void SetZone(int id)
    {
        SaveLocal();

        PlayerLevelInfo.currentZone = id;

        LoadScene(eScreen.Level);
    }
    
    public override void LoadScene(eScreen sceneName)
    {
        player.MakeDialoguerNonClickable();

        player.CompRendering.OutlineOff();
        player.CompRendering.OutlineLocked();

        levelEntry.OutlineOff();
        levelEntry.OutlineLocked();

        base.LoadScene(sceneName);
    }

    public void ShowRanking()
    {
        GooglePlayManager.ShowRanking();
    }

    public void HideContinueButton()
    {
        DialoguePanelManager.SetContinueButtonNever();
    }

    public void ShowContinueButton()
    {
        DialoguePanelManager.SetContinueButtonAlways();
    }

    public void DownloadRemote()
    {
        //Its called by the onClick button function on the dialogue 
        RemoteManager.Instance.DownloadRemote();
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
