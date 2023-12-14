using PixelCrushers.DialogueSystem;

using UnityEngine;

public static class Creator
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnLoad()
    {
        if (GameObject.FindObjectOfType<SceneLoader>() == null)
        {
            GameObject goSceneLoader = new GameObject("DDOL_Scene Loader", typeof(SceneLoader));
            GameObject.DontDestroyOnLoad(goSceneLoader);
        }
        
        if (GameObject.FindObjectOfType<AudioManager>() == null)
        {
            GameObject audioManagerPrefab = Resources.Load("Prefabs/Audio/PFB_Audio Manager") as GameObject;
            GameObject goAudioManager = GameObject.Instantiate(audioManagerPrefab);
            goAudioManager.name = "DDOL_Audio Manager";
            GameObject.DontDestroyOnLoad(goAudioManager);
        }
        
        if (GameObject.FindObjectOfType<DialogueSystemController>() == null)
        {
            GameObject dialogueManagerPrefab = Resources.Load("Prefabs/Dialogue/PFB_Dialogue Manager") as GameObject;
            GameObject goDialogueManager = GameObject.Instantiate(dialogueManagerPrefab);
            goDialogueManager.name = "DDOL_Dialogue Manager";
            GameObject.DontDestroyOnLoad(goDialogueManager);
        }
        
        if (GameObject.FindObjectOfType<RemoteManager>() == null)
        {
            GameObject remoteManagerPrefab = Resources.Load("Prefabs/Remote/PFB_Remote Manager") as GameObject;
            GameObject goRemoteManager = GameObject.Instantiate(remoteManagerPrefab);
            goRemoteManager.name = "DDOL_Remote Manager";
            GameObject.DontDestroyOnLoad(goRemoteManager);
        }
    }
}
