using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.SceneManagement;
using PixelCrushers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Newtonsoft.Json.Converters;
//using System.Globalization;

[System.Serializable]
public class AreaSister
{
	public string label;
}

[System.Serializable]
public class IloParameterSister
{
	public string label;
	public string description;
	public string parameter_type;
	public int default_value;
	public int min_value;
	public int max_value;
	public bool is_active;
	public string text;
}

[System.Serializable]
public class IloSister
{
	public string label;
	public string description;
	public bool selectable;
	public bool selected;
	public IloSister[] ilos;
	public IloParameterSister[] ilo_parameters;
}

[System.Serializable]
public class DGBLFeaturesSister
{
	public AreaSister[] learning_areas;
	public IloSister[] ilos;
}

[System.Serializable]
public class UrlSister
{
	public string label;
	public string description;
	public string url;
}

[System.Serializable]
public class GameDescriptionSister
{
	public string label;
	public string short_description;
	public string long_description;
	public UrlSister[] images;
	public UrlSister[] urls;
}

[System.Serializable]
public class RemoteSister
{
	public GameDescriptionSister game_description;
	public DGBLFeaturesSister dgbl_features;
}

//public partial class Login
//{
//	[JsonProperty("token")]
//	public string Token { get; set; }
//}

//public partial class Login
//{
//	public static Login FromJson(string json) => JsonConvert.DeserializeObject<Login>(
//        json, 
//        new JsonSerializerSettings
//	    {
//		    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
//		    DateParseHandling = DateParseHandling.None,
//		    Converters =
//			    {
//                    //ValueConverter.Singleton,
//                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
//			    },
//	    }
//    );
//}


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

 //   //Game Authoring API Adapter
	//protected const string GAME_AUTHORING_SERVER = "79.137.77.50"; //"localhost"
	//protected const string GAME_AUTHORING_SERVER_PORT = "8000";
	//protected const string GAME_AUTHORING_URL_API_LOGIN = "api/api-token-auth";
	//protected const string GAME_AUTHORING_URL_API_GAMES = "api/games";
	//protected const string GAME_AUTHORING_URL_API_STUDENTS = "api/students";
	//protected const string GAME_AUTHORING_URL_API_INVENTORY = "api/inventory";
	//protected const string GAME_AUTHORING_URL_API_ACTIVE_GAMES = "api/active_games";
	//protected const string GAME_AUTHORING_URL_API_GAME_CONFIG = "api/game_configs";
	//protected const string GAME_AUTHORING_URL_API_STUDENT_GAME_CONFIG = "api/student_game_config";
 //   protected const int GAME_ID = 9;
	//private string username = "matranking@gmail.com";
	//private string password = "matranking1998";
	//private string token = "";

	private void Start()
	{
		dm = GameObject.FindGameObjectWithTag("DialogueManager");
		PLAYER_PATH = Application.persistentDataPath + "/Local.json";
        REMOTE_PATH = Application.persistentDataPath + "/Remote.json";
        OPTIONS_PATH = Application.persistentDataPath + "/Options.json";
        version = PlayerPrefs.GetString("version", "0.0.0");
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

            Debug.Log("Primera vez que se instaló");
        }

        if (!string.Equals(version, "1.0.7"))//Different from actual version
        {
            //Do something
            File.WriteAllText(PLAYER_PATH, JsonUtility.ToJson(playerSO));
            PlayerPrefs.DeleteKey("version");
            PlayerPrefs.SetString("version", "1.0.7");//Change 1.0.0 for actual version every want to update app
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
        int n;

		//WWWForm form = new WWWForm();
		//form.AddField("username", username);
		//form.AddField("password", password);

		//using(UnityWebRequest www = UnityWebRequest.Post(String.Format("http://{0}:{1}/{2}/",
		//		GAME_AUTHORING_SERVER, GAME_AUTHORING_SERVER_PORT, GAME_AUTHORING_URL_API_LOGIN), form))
  //      {
		//	www.SendWebRequest();

  //          n = 10;

		//	while (n > 0)
  //          {
		//		if (n == 0) //10f is www.timeout
  //              {
  //                  www.Abort();
  //                  break;
  //              }
  //              if (www.isDone) break;

  //              n--;
		//		yield return new WaitForSeconds(1f);
		//	}

		//	if (!www.isDone || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
  //          {
		//		Debug.Log("No se logeo al jugador en EDU Game Authoring Platform");
		//	}
		//	else
		//	{
		//		Debug.Log("Se logeo al jugador en EDU Game Authoring Platform");

		//		Login loginData = Login.FromJson(www.downloadHandler.text);

  //              token = loginData.Token;
		//	}
		//}

		//string url = String.Format(
  //          "http://{0}:{1}/{2}/{3}/{4}", 
  //          GAME_AUTHORING_SERVER, 
  //          GAME_AUTHORING_SERVER_PORT, 
  //          GAME_AUTHORING_URL_API_GAME_CONFIG, 
  //          username,
		//	GAME_ID);

		//using (UnityWebRequest request = UnityWebRequest.Get(url))
		using (UnityWebRequest request = UnityWebRequest.Get("https://matranking-configurationserver.herokuapp.com/config"))
        {
			// request.SetRequestHeader("Authorization", "Token " + token);

			request.SendWebRequest();

			n = 10;

			while (n > 0)
			{
				if (n == 0) //10f is www.timeout
				{
					request.Abort();
					break;
				}
				if (request.isDone) break;

				n--;
				yield return new WaitForSeconds(1f);
			}

			if (!request.isDone || request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("No se pudo obtener el archivo json de configuracion educativa");
            }
            else
            {
                Debug.Log("Se obtuvo satisfactoriamente la lista de jsons de configuracion educativa");

                byte[] result = request.downloadHandler.data;
                //string gameSessionsJSON = System.Text.Encoding.Default.GetString(result);
                string jsonFetch = System.Text.Encoding.Default.GetString(result);

                //List<RemoteSister> studentGameConfigs = JsonConvert.DeserializeObject<List<RemoteSister>>(gameSessionsJSON);

                //if (studentGameConfigs != null)
                //{
                //    Debug.Log("Se utiliza la última configuración");

                //    string jsonFetch = JsonConvert.SerializeObject(studentGameConfigs.Last());

                //    if (jsonFetch != null) jsonRemote = jsonFetch;
                //}
                //else
                //{
                //    Debug.Log("No había configuraciones");
                //}

                if (jsonFetch != null) jsonRemote = jsonFetch;

                //Have to actualize json
                File.WriteAllText(REMOTE_PATH, jsonRemote);
            }

            if (jsonRemote != null)
            {
                //Debug.Log(jsonRemote);
				Debug.Log("Sobreescribimos el SO con el texto del json");

				RemoteSister auxRemote = new RemoteSister();
				//RemoteSO auxRemote = ScriptableObject.CreateInstance("RemoteSO") as RemoteSO;

				auxRemote = JsonConvert.DeserializeObject<RemoteSister>(jsonRemote);
                //auxRemote = JsonConvert.DeserializeObject<RemoteSO>(jsonRemote);

                //Competence 1-----------------------------------------------------------------------------------------------------------------------------------
                //L1
                remoteSO.dgbl_features.ilos[0].ilos[0].selected = auxRemote.dgbl_features.ilos[0].ilos[0].selected;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[3].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[3].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[4].default_value = auxRemote.dgbl_features.ilos[0].ilos[0].ilo_parameters[4].default_value;

                //L2
                remoteSO.dgbl_features.ilos[0].ilos[1].selected = auxRemote.dgbl_features.ilos[0].ilos[1].selected;

				//L2.1
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].selected = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[0].selected;
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[0].default_value;
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;
                remoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active;

				//l2.2
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].selected = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[1].selected;
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[0].default_value;
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;
				remoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[3].default_value = auxRemote.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[3].default_value;

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

				//L13.1
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].selected = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[0].selected;
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value;
                remoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value;

				//L13.2
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].selected = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[1].selected;
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[0].default_value;
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value;
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value;

				//L13.3
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].selected = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[2].selected;
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[0].default_value;
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[1].default_value;
				remoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[2].default_value;

				//L16

				//Competence 4-----------------------------------------------------------------------------------------------------------------------------------
				//L19
				remoteSO.dgbl_features.ilos[3].ilos[1].selected = auxRemote.dgbl_features.ilos[3].ilos[1].selected;
				remoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[3].ilos[1].ilo_parameters[0].default_value;
				remoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[3].ilos[1].ilo_parameters[1].default_value;
				remoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[3].ilos[1].ilo_parameters[2].default_value;

				//L21
				remoteSO.dgbl_features.ilos[3].ilos[3].selected = auxRemote.dgbl_features.ilos[3].ilos[3].selected;

				//L21.1
				remoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].selected = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[0].selected;
				remoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[0].default_value;
                remoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[1].default_value;
                remoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[2].default_value;

				//L21.2
				remoteSO.dgbl_features.ilos[3].ilos[3].ilos[1].selected = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[1].selected;
				remoteSO.dgbl_features.ilos[3].ilos[3].ilos[1].ilo_parameters[0].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[1].ilo_parameters[0].default_value;
				remoteSO.dgbl_features.ilos[3].ilos[3].ilos[1].ilo_parameters[1].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[1].ilo_parameters[1].default_value;
				remoteSO.dgbl_features.ilos[3].ilos[3].ilos[1].ilo_parameters[2].default_value = auxRemote.dgbl_features.ilos[3].ilos[3].ilos[1].ilo_parameters[2].default_value;
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