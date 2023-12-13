using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : SerializedMonoBehaviour
{
    [Header("SCENE")]
    [SerializeField] protected Animator transitionAnimator;
    [SerializeField] protected Slider bgmSlider;
    [SerializeField] protected Slider sfxSlider;

    public virtual void LoadMenu(float transitionTime = 1)
    {
        StartCoroutine(CRTLoadScene(0, transitionTime));
    }

    public virtual void LoadAdventure(float transitionTime = 1)
    {
        StartCoroutine(CRTLoadScene(1, transitionTime));
    }

    public virtual void LoadLevel(float transitionTime = 1)
    {
        StartCoroutine(CRTLoadScene(2, transitionTime));
    }

    IEnumerator CRTLoadScene(int nScene, float transitionTime)
    {
        AudioManager.FadeOutBgmVolume();
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(nScene); // 0: mainMenu, 1:adventure, 2:level
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