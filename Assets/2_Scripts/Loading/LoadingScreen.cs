﻿using System;
using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;

    [Header("Phrases")]
    [SerializeField] private TextAsset textJSON;

    private PhraseList myPhraseList;
    [SerializeField] private GameObject phrasesContainer;
    [SerializeField] private Text phrase;
    [SerializeField] private Text author;

    [Header("Level Intro")]
    [SerializeField] private GameObject levelIntroContainer;
    [SerializeField] private Text zone;
    [SerializeField] private Text floor;

    [Header("Level Outro")]
    [SerializeField] private GameObject levelOutroContainer;

    private float timeInLoadingScreen;

    void Awake()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        eScreen targetScreen = SceneLoader.Instance.GetTargetScreen();

        if (SceneLoader.Instance.PreviousScreen == eScreen.MainMenu &&
            targetScreen == eScreen.Adventure)
        {
            SetPhrasesText();

            phrasesContainer.SetActive(true);
            levelIntroContainer.SetActive(false);
            levelOutroContainer.SetActive(false);

            timeInLoadingScreen = 4;
        }
        else if (SceneLoader.Instance.PreviousScreen == eScreen.Adventure &&
                 targetScreen == eScreen.MainMenu)
        {
            phrasesContainer.SetActive(false);
            levelIntroContainer.SetActive(false);
            levelOutroContainer.SetActive(false);

            timeInLoadingScreen = 0;
        }
        else if ((SceneLoader.Instance.PreviousScreen == eScreen.MainMenu ||
                    SceneLoader.Instance.PreviousScreen == eScreen.Adventure ||
                    SceneLoader.Instance.PreviousScreen == eScreen.Level) &&
                 targetScreen == eScreen.Level)
        {
            SetLevelIntroText();
            Feedback.Do(eFeedbackType.LevelStart);
            
            phrasesContainer.SetActive(false);
            levelIntroContainer.SetActive(true);
            levelOutroContainer.SetActive(false);
            
            timeInLoadingScreen = 1.75f;
        }
        else if (SceneLoader.Instance.PreviousScreen == eScreen.Level &&
                 targetScreen == eScreen.Adventure)
        {
            SetLevelOutroText();
            
            phrasesContainer.SetActive(false);
            levelIntroContainer.SetActive(false);
            levelOutroContainer.SetActive(true);

            timeInLoadingScreen = 7;
        }

        SceneLoader.Instance.ChangeScreen(targetScreen, false);
        StartCoroutine(CRTAllowScreenChange(timeInLoadingScreen));
    }

    IEnumerator CRTAllowScreenChange(float time = 1)
    {
        float startTime = Time.time;

        yield return new WaitUntil(() => SceneLoader.Instance.IsScreenLoadingCompleted(true));

        float elapsedTime = Time.time - startTime;
        float waitForSecondsTime = Mathf.Max(0, time - elapsedTime);

        yield return new WaitForSeconds(waitForSecondsTime);
        transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);

        SceneLoader.Instance.AllowScreenChange();
    }

    private void SetPhrasesText()
    {
        myPhraseList = JsonUtility.FromJson<PhraseList>(textJSON.text);

        //Set text for the transition
        int n = Random.Range(0, myPhraseList.phrases.Length);
        switch (Enum.Parse<eLanguage>(Localization.language))
        {
            case eLanguage.es:
                phrase.text = '"' + myPhraseList.phrases[n].frase + '.' + '"';
                break;

            case eLanguage.en:
                phrase.text = '"' + myPhraseList.phrases[n].phrase + '.' + '"';
                break;

            case eLanguage.qu:
                break;
        }

        author.text = myPhraseList.phrases[n].autor;
    }

    private void SetLevelIntroText()
    {
        if (RemoteManager.Instance.IsLevelDataEmpty())
        {
            zone.text = DialogueManager.GetLocalizedText("challengeOff");
            floor.text = DialogueManager.GetLocalizedText("noEnemies");
        }
        else
        {
            zone.text = DialogueManager.GetLocalizedText("challenge");
            floor.text = DialogueManager.GetLocalizedText("floor");
            zone.text += " " + (PlayerLevelInfo.currentZone + 1).ToString();
            floor.text += " " + PlayerLevelInfo.currentLevel.ToString();
        }
    }

    private void SetLevelOutroText()
    {
        
    }
}