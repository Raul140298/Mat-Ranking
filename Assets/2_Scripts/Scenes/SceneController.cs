using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : SerializedMonoBehaviour
{
    [Header("SCENE")]
    [SerializeField] protected Animator transitionAnimator;
    [SerializeField] protected Slider bgmSlider;
    [SerializeField] protected Slider sfxSlider;
    
    public virtual void LoadScene(string sceneName)
    {
        eScreen scene = (eScreen)Enum.Parse(typeof(eScreen), sceneName);
        StartCoroutine(CRTLoadScene(scene));
    }
    
    public virtual void LoadScene(eScreen scene)
    {
        StartCoroutine(CRTLoadScene(scene));
    }

    IEnumerator CRTLoadScene(eScreen scene)
    {
        AudioManager.FadeOutBgm();
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);

        SceneLoader.Instance.SetTargetScreen(scene);
        SceneLoader.Instance.ChangeScreen(eScreen.Loading, true);
    }
    
    public void ChangeKnowledgePoints(int n, Text knowledgePoints)
    {
        if (PlayerSessionInfo.knowledgePoints + n >= 0)
        {
            PlayerSessionInfo.knowledgePoints += n;
            GooglePlayManager.OpenSavedGameForSave("MatRanking");
            GooglePlayManager.SendRanking(PlayerSessionInfo.knowledgePoints);
            SetKnowledgePoints(knowledgePoints);
        }
    }
    
    protected void SetKnowledgePoints(Text knowledgePoints)
    {
        knowledgePoints.text = PlayerSessionInfo.knowledgePoints.ToString("D3");
    }

    public void PlayAudio(string feedbackType)
    {
        Feedback.Do((eFeedbackType)Enum.Parse(typeof(eFeedbackType), feedbackType));
    }
    
    public void PlayAudio(eFeedbackType feedbackType)
    {
        Feedback.Do(feedbackType);
    }
    
    public virtual void SaveLocal(GameObject player = null)
    {
        if (player) PlayerSessionInfo.playerPosition = player.transform.position;
        
        GooglePlayManager.OpenSavedGameForSave("MatRanking");
    }

    public virtual void SaveOptions()
    {
        PlayerSessionInfo.sfxVolume = AudioController.GetCategoryVolume("SFX");
        PlayerSessionInfo.bgmVolume = AudioController.GetCategoryVolume("BGM");
        
        GooglePlayManager.OpenSavedGameForSave("MatRanking");
    }

    public virtual void Exit()
    {
        Application.Quit();
    }
}