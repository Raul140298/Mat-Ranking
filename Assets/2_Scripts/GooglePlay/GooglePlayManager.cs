using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;

public static class GooglePlayManager
{
    // LOGIN ==========================================

    public static void Authenticate()
    {
#if !UNITY_EDITOR
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
#endif
    }

    internal static void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            OpenSavedGameForLoad("MatRanking");
        }
        else
        {
            // Disable integration with Play Games Services or display a login button
            // to prompt users to log in. Clicking should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
    
    // LOAD ==========================================
    
    private static void OpenSavedGameForLoad(string filename)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedForLoad);
    }

    private static void OnSavedGameOpenedForLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            LoadGame(game);
        }
        else
        {
            if (game == null)
            {
                SaveInitialGameData();
            }
        }
    }
    
    private static void LoadGame(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private static void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PlayerSessionInfo.Deserialize(data);
        }
        else
        {
            // Manage the error
        }
    }
    
    // SAVE ==========================================
    
    public static void OpenSavedGameForSave(string filename)
    {
#if !UNITY_EDITOR
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedForSave);
#endif
    }

    private static void OnSavedGameOpenedForSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            SaveGame(game, PlayerSessionInfo.timePlayed);
        }
        else
        {
        }
    }
    
    private static void SaveInitialGameData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        
        byte[] initialData = PlayerSessionInfo.Serialize(true);
        
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(TimeSpan.Zero)
            .WithUpdatedDescription("Initial saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        
        savedGameClient.CommitUpdate(null, updatedMetadata, initialData, OnFirstSavedGameWritten);
    }

    private static void SaveGame(ISavedGameMetadata game, TimeSpan totalPlaytime)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        
        byte[] savedData = PlayerSessionInfo.Serialize();

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }
    
    private static void OnFirstSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            LoadGame(game);
        }
        else
        {
            // Manage the error
        }
    }

    private static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // Manage the saved game written
        }
        else
        {
            // Manage the error
        }
    }
    
    // DELETE ==========================================

    /*public static void DeleteGameData(string filename)
    {
        // Open the file to get the metadata.
        
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, DeleteSavedGame);
    }

    private static void DeleteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.Delete(game);
        }
        else
        {
            // Manage the error
        }
    }*/

    // RANKING ==========================================

    public static void SendRanking(int score)
    {
#if !UNITY_EDITOR
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, WorldValues.GOOGLE_LEADERBOARD_ID, success => { });
        }
#endif
    }

    public static void ShowRanking()
    {
#if !UNITY_EDITOR
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
#endif
    }

    // ACHIEVEMENTS ==========================================

    public static void UnlockAchievement(eAchievements achieve)
    {
#if !UNITY_EDITOR
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(WorldValues.GOOGLE_ACHIEVEMENTS[achieve], 100f, success => { });
        }
#endif
    }

    public static void ShowAchievements()
    {
#if !UNITY_EDITOR
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
#endif
    }
}