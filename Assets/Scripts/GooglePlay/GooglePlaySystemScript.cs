using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;

public enum eAchievements
{
    Complete1Challenge = 0, //CgkIlve8wrUJEAIQAg
}

public class GooglePlaySystemScript : MonoBehaviour
{
    public static GooglePlaySystemScript instance;
    [SerializeField] private String[] achievements;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID

        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);

#endif
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }

    public void SendRanking(int score)
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, "CgkIlve8wrUJEAIQAQ", success => { });
        }
#endif
    }

    public void ShowRanking()
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIlve8wrUJEAIQAQ");
        }
#endif
    }

    public void UnlockAchievement(eAchievements achieve)
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(achievements[(int)achieve], 100f, success => { });
        }
#endif
    }

    public void ShowAchievements()
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
#endif
    }
}
