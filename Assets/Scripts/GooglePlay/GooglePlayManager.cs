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
#if UNITY_ANDROID
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
            // Deshabilitar la integración con Play Games Services o mostrar un botón de inicio de sesión
            // para pedir a los usuarios que inicien sesión. Hacer clic debería llamar a
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
    
    // LOAD ==========================================
    
    public static void OpenSavedGameForLoad(string filename)
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
            // Manejar el error
        }
    }
    
    // SAVE ==========================================
    
    public static void OpenSavedGameForSave(string filename)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedForSave);
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

    private static void SaveGame(ISavedGameMetadata game, TimeSpan totalPlaytime)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        // Serializar los datos de PlayerSessionInfo a un arreglo de bytes
        byte[] savedData = PlayerSessionInfo.Serialize();

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    private static void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // Manejar la lectura o escritura del juego guardado.
        }
        else
        {
            // Manejar el error
        }
    }
    
    // DELETE ==========================================

    /*public static void DeleteGameData(string filename)
    {
        // Abrir el archivo para obtener los metadatos.
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
            // Manejar el error
        }
    }*/

    // RANKING ==========================================

    public static void SendRanking(int score)
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, WorldValues.GOOGLE_LEADERBOARD_ID, success => { });
        }
#endif
    }

    public static void ShowRanking()
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
#endif
    }

    // ACHIEVEMENTS ==========================================

    public static void UnlockAchievement(eAchievements achieve)
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Social.ReportProgress(WorldValues.GOOGLE_ACHIEVEMENTS[achieve], 100f, success => { });
        }
#endif
    }

    public static void ShowAchievements()
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            Social.ShowAchievementsUI();
        }
#endif
    }
}