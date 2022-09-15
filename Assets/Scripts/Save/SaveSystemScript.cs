using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.SceneManagement;
using PixelCrushers;

public class SaveSystemScript : MonoBehaviour
{
    public GameObject player;
    public PlayerSO playerSO;
    public OptionsSO optionsSO;
    public RemoteSO remoteSO;
    public CurrentLevelSO currentLevelSO;
    public SoundtracksScript soundtracks;
    public SoundsScript sounds;
    private string PLAYER_PATH;
    private string REMOTE_PATH;
    private string OPTIONS_PATH;
    public Text knowledgePoints;
    public PlayFabScript playFab;
    private string version;
    public GameObject dm;

    private void Start()
	{
		dm = GameObject.FindGameObjectWithTag("DialogueManager");
		PLAYER_PATH = Application.persistentDataPath + "/Local.json";
        REMOTE_PATH = Application.persistentDataPath + "/Remote.json";
        OPTIONS_PATH = Application.persistentDataPath + "/Options.json";
        version = PlayerPrefs.GetString("version", "0.0.0");
        Debug.Log(version);
    }

    public void sendRanking()
    {
		if (playFab) playFab.SendRanking(playerSO.knowledgePoints);
	}

    // LOCAL-------------------------------------------------------------------------
    public void saveLocal()
    {
        if (player)
        {
            playerSO.playerPosition = player.transform.position;
        }

        string jsonLocal = JsonUtility.ToJson(playerSO);
        SaveLocal(jsonLocal);

        if (SceneManager.GetActiveScene().buildIndex == 1) dm.gameObject.GetComponent<SaveSystem>().SaveGameToSlot(1);

        Debug.Log("Se guardó la partida");
    }

    private void SaveLocal(string saveString)
    {
        File.WriteAllText(PLAYER_PATH, saveString);
    }

    public void loadLocal()
    {
        string jsonLocal = LoadLocal();
        if (jsonLocal != null)
        {
            JsonUtility.FromJsonOverwrite(jsonLocal, playerSO); //fill playerSO with jsonLocal data

            if (player)
            {
                player.transform.position = playerSO.playerPosition;
            }
        }
    }

    private string LoadLocal()
    {
        //First time installing
        if (!File.Exists(PLAYER_PATH) || string.Equals(version, "0.0.0"))
        {
            File.WriteAllText(PLAYER_PATH, JsonUtility.ToJson(playerSO));
            PlayerPrefs.DeleteKey("version");
            PixelCrushers.SaveSystem.ResetGameState();
            PixelCrushers.SaveSystem.DeleteSavedGameInSlot(1);
            PlayerPrefs.SetString("version", "0.0.0");
            PlayerPrefs.Save();
            version = PlayerPrefs.GetString("version");

            Debug.Log("Primera vez que instaló");
        }

        if (!string.Equals(version, "1.0.4"))//Different from actual version
        {
            //Do something
            File.WriteAllText(PLAYER_PATH, JsonUtility.ToJson(playerSO));
            PlayerPrefs.DeleteKey("version");
            PlayerPrefs.SetString("version", "1.0.4");//Change 1.0.0 for actual version every want to update app
            PlayerPrefs.Save();

            Debug.Log("Primera vez que se juega (Pudo desinstalar)");
        }

        string json = File.ReadAllText(PLAYER_PATH);
        return json;
    }

    // REMOTE-------------------------------------------------------------------------
    // GAME'S DATA TO BE DOWNLOADED FROM THE API
    public void downloadRemote()
    {
        StartCoroutine(GetJson());
    }

    private IEnumerator GetJson()
	{
        string jsonRemote = LoadRemote();

        using (UnityWebRequest request = UnityWebRequest.Get("uri")) // <- change to the original uri
		{
            yield return request.SendWebRequest();
            if(request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
			{
                Debug.Log("No se pudo obtener el archivo json de configuracion educativa");
			}
			else
			{
                Debug.Log("Se obtuvo satisfactoriamente el archivo json de configuracion educativa");
                string jsonFetch = request.downloadHandler.text;
                if (jsonFetch != null) jsonRemote = jsonFetch;
            }

            if (jsonRemote != null)
            {
                //Debug.Log(jsonRemote);
				Debug.Log("Sobreescribimos el SO con el texto del json");

				RemoteSO auxRemote = ScriptableObject.CreateInstance("RemoteSO") as RemoteSO;

				Debug.unityLogger.logEnabled = false;
				auxRemote = JsonConvert.DeserializeObject<RemoteSO>(jsonRemote);
				Debug.unityLogger.logEnabled = true;

				//Competence 1-----------------------------------------------------------------------------------------------------------------------------------
                //L1
				remoteSO.dgbl_features.ilos[0].ilos[0].selected = auxRemote.dgbl_features.ilos[0].ilos[0].selected;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[0].default_value;

                //L2
                remoteSO.dgbl_features.ilos[0].ilos[1].selected = auxRemote.dgbl_features.ilos[0].ilos[1].selected;
                remoteSO.dgbl_features.ilos[0].ilos[1].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[1].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[1].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilo_parameters[0].default_value;

                //L5
				remoteSO.dgbl_features.ilos[0].ilos[4].selected = auxRemote.dgbl_features.ilos[0].ilos[4].selected;

				//Competence 2-----------------------------------------------------------------------------------------------------------------------------------
				//L8
				remoteSO.dgbl_features.ilos[1].ilos[0].selected = auxRemote.dgbl_features.ilos[1].ilos[0].selected;
                remoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[1].ilos[0].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value;
                remoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value;

                //L9
				remoteSO.dgbl_features.ilos[1].ilos[1].selected = auxRemote.dgbl_features.ilos[1].ilos[1].selected;
				remoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[1].ilos[1].ilo_parameters[0].default_value;
				remoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[1].ilos[1].ilo_parameters[1].default_value;
				remoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[1].ilos[1].ilo_parameters[2].default_value;
				
                //Competence 3-----------------------------------------------------------------------------------------------------------------------------------
				//L13
				remoteSO.dgbl_features.ilos[2].ilos[0].selected = auxRemote.dgbl_features.ilos[2].ilos[0].selected;
                remoteSO.dgbl_features.ilos[2].ilos[0].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[2].ilos[0].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilo_parameters[1].default_value;
                remoteSO.dgbl_features.ilos[2].ilos[0].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilo_parameters[2].default_value;

				//Competence 4-----------------------------------------------------------------------------------------------------------------------------------
				//L21
				remoteSO.dgbl_features.ilos[3].ilos[3].selected = auxRemote.dgbl_features.ilos[3].ilos[3].selected;
                remoteSO.dgbl_features.ilos[3].ilos[3].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[3].ilos[3].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilo_parameters[1].default_value;
                remoteSO.dgbl_features.ilos[3].ilos[3].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilo_parameters[2].default_value;

                Destroy(auxRemote);
            }
        }
	}

    private string LoadRemote()
    {
        if (!File.Exists(REMOTE_PATH))
        {
			Debug.Log("Creamos el archivo json");
			File.WriteAllText(REMOTE_PATH, JsonConvert.SerializeObject(remoteSO));
        }

        string json = File.ReadAllText(REMOTE_PATH);
        return json;
    }

    // OPTIONS-------------------------------------------------------------------------
    // OPTIONS'DATA TO BE SAVED
    public void loadOptions()
    {
        string jsonLocal = LoadOptions();
        if (jsonLocal != null)
        {
            JsonUtility.FromJsonOverwrite(jsonLocal, optionsSO);
            soundtracks.slider.value = optionsSO.soundtracksVolume;
            SoundtracksScript.ChangeVolume(optionsSO.soundtracksVolume);

            sounds.slider.value = optionsSO.soundsVolume;
            SoundsScript.ChangeVolume(optionsSO.soundsVolume);
        }
    }

    public void saveOptions()
    {
        optionsSO.soundtracksVolume = soundtracks.slider.value;
        optionsSO.soundsVolume = sounds.slider.value;

        string jsonLocal = JsonUtility.ToJson(optionsSO);
        SaveOptions(jsonLocal);
        Debug.Log("Se guardó las opciones");
    }

    private void SaveOptions(string saveString)
    {
        File.WriteAllText(OPTIONS_PATH, saveString);
    }

    private string LoadOptions()
    {
        //If the file isn't exist
        if (!File.Exists(OPTIONS_PATH))
        {
            File.WriteAllText(OPTIONS_PATH, JsonUtility.ToJson(optionsSO));
        }

        string json = File.ReadAllText(OPTIONS_PATH);
        return json;
    }
}