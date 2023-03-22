using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.SceneManagement;
using PixelCrushers;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;
using PixelCrushers.DialogueSystem;
using System.Globalization;

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

public partial class Login
{
    [JsonProperty("token")]
    public string Token { get; set; }
}

public partial class Login
{
    public static Login FromJson(string json) => JsonConvert.DeserializeObject<Login>(
        json,
        new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
                {
                    //ValueConverter.Singleton,
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
        }
    );
}

public class SaveSystemScript : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private OptionsSO optionsSO;
    [SerializeField] private RemoteSO remoteSO;
    [SerializeField] private CurrentLevelSO currentLevelSO;

    private string PLAYER_PATH;
    private string REMOTE_PATH;
    private string OPTIONS_PATH;
    private string version;
    private GameObject dm;

    //Game Authoring API Adapter
    protected const string GAME_AUTHORING_SERVER = "degauthoring-env.eba-8qzg6thz.us-east-1.elasticbeanstalk.com";
    protected const string GAME_AUTHORING_SERVER_PORT = "8000";
    protected const string GAME_AUTHORING_URL_API_LOGIN = "api/api-token-auth";
    protected const string GAME_AUTHORING_URL_API_GAMES = "api/games";
    protected const string GAME_AUTHORING_URL_API_STUDENTS = "api/students";
    protected const string GAME_AUTHORING_URL_API_INVENTORY = "api/inventory";
    protected const string GAME_AUTHORING_URL_API_ACTIVE_GAMES = "api/active_games";
    protected const string GAME_AUTHORING_URL_API_GAME_CONFIG = "api/game_configs";
    protected const string GAME_AUTHORING_URL_API_STUDENT_GAME_CONFIG = "api/student_game_config";
    protected const int GAME_ID = 9;
    private string username = "johan.baldeon@yahoo.com";
    private string password = "Authoring2022";
    private string token = "383c9115a207e6888ef82d8f604f05eabf2ad927";

    private void Awake()
    {
        bool exist = GameObject.FindGameObjectsWithTag("SaveSystem").Length > 1;

        if (exist)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    public void AwakeSystem()
    {
        PLAYER_PATH = Application.persistentDataPath + "/Local.json";
        REMOTE_PATH = Application.persistentDataPath + "/Remote.json";
        OPTIONS_PATH = Application.persistentDataPath + "/Options.json";
        version = PlayerPrefs.GetString("version", "0.0.0");
    }

    public void StartSystem(GameObject dialogueManager)
    {
        dm = dialogueManager;
    }

    // LOCAL-------------------------------------------------------------------------
    public void SaveLocal(GameObject player = null)
    {
        if (player)
        {
            playerSO.playerPosition = player.transform.position;
        }

        string jsonLocal = JsonUtility.ToJson(playerSO);
        SaveLocalFile(jsonLocal);

        if (SceneManager.GetActiveScene().buildIndex == 1) dm.gameObject.GetComponent<SaveSystem>().SaveGameToSlot(1);

        Debug.Log("Se guardo la partida");
    }

    private void SaveLocalFile(string saveString)
    {
        File.WriteAllText(PLAYER_PATH, saveString);
    }

    public void LoadLocal(GameObject player = null)
    {
        string jsonLocal = LoadLocalFile();
        if (jsonLocal != null)
        {
            JsonUtility.FromJsonOverwrite(jsonLocal, playerSO); //fill playerSO with jsonLocal data

            if (player)
            {
                player.transform.position = playerSO.playerPosition;
            }
        }
    }

    private string LoadLocalFile()
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

            Debug.Log("Primera vez que se instala");
        }

        if (!string.Equals(version, Application.version))//Different from actual version
        {
            //Do something
            File.WriteAllText(PLAYER_PATH, JsonUtility.ToJson(playerSO));
            PlayerPrefs.DeleteKey("version");
            PlayerPrefs.SetString("version", Application.version);//Change 1.0.0 for actual version every want to update app
            PlayerPrefs.Save();

            Debug.Log("Primera vez que se juega (Pudo desinstalar)");
        }

        string json = File.ReadAllText(PLAYER_PATH);
        return json;
    }

    // REMOTE-------------------------------------------------------------------------
    // GAME'S DATA TO BE DOWNLOADED FROM THE CONTENT SERVER THAT WE CHOOSE
    public void DownloadRemote()
    {
        StartCoroutine(CRTGetJsonDEGA());
    }

    private IEnumerator CRTGetJsonDEGA()
    {
        int n;
        string url;

        //jsonRemote have the current data of Remote file in our device 
        string jsonRemote = LoadRemote();

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        url = String.Format(
            "http://{0}/{1}/",
            GAME_AUTHORING_SERVER,
            GAME_AUTHORING_URL_API_LOGIN);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SendWebRequest();

            n = 10;

            while (n > 0)
            {
                if (n == 0) //10f is www.timeout
                {
                    www.Abort();
                    break;
                }
                if (www.isDone) break;

                n--;
                yield return new WaitForSeconds(1f);
            }

            if (!www.isDone || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("No se logeo al jugador en EDU Game Authoring Platform");
            }
            else
            {
                Debug.Log("Se logeo al jugador en EDU Game Authoring Platform");

                Login loginData = Login.FromJson(www.downloadHandler.text);

                token = loginData.Token;
            }
        }

        url = String.Format(
            "http://{0}/{1}/{2}/{3}",
            GAME_AUTHORING_SERVER,
            GAME_AUTHORING_URL_API_GAME_CONFIG,
            username,
            GAME_ID);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Token " + token);
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
                string gameSessionsJSON = System.Text.Encoding.Default.GetString(result);

                //We get all previous configs not only the last
                List<RemoteSister> studentGameConfigs = JsonConvert.DeserializeObject<List<RemoteSister>>(gameSessionsJSON);

                if (studentGameConfigs != null)
                {
                    Debug.Log("Se obtuvo una nueva configuracion");

                    string jsonFetch = JsonConvert.SerializeObject(studentGameConfigs.Last());

                    if (jsonFetch != null) jsonRemote = jsonFetch;
                }
                else
                {
                    Debug.Log("No habia configuraciones");
                }

                //Have to actualize json
                File.WriteAllText(REMOTE_PATH, jsonRemote);
            }

            if (jsonRemote != null)
            {
                Debug.Log("Sobreescribimos el SO con el texto del archivo json");

                RemoteSister auxRemote = new RemoteSister();
                auxRemote = JsonConvert.DeserializeObject<RemoteSister>(jsonRemote);

                if (auxRemote.dgbl_features != null)
                {
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
    public void LoadOptions()
    {
        string jsonLocal = LoadOptionsFile();
        if (jsonLocal != null)
        {
            JsonUtility.FromJsonOverwrite(jsonLocal, optionsSO);
            SoundtracksScript.Slider.value = optionsSO.soundtracksVolume;
            SoundtracksScript.ChangeVolume(optionsSO.soundtracksVolume);

            SoundsScript.Slider.value = optionsSO.soundsVolume;
            SoundsScript.ChangeVolume(optionsSO.soundsVolume);
        }
    }

    public void SaveOptions()
    {
        optionsSO.soundtracksVolume = SoundtracksScript.Slider.value;
        optionsSO.soundsVolume = SoundsScript.Slider.value;

        string jsonLocal = JsonUtility.ToJson(optionsSO);
        SaveOptionsFile(jsonLocal);
        Debug.Log("Se guardo las opciones");
    }

    private void SaveOptionsFile(string saveString)
    {
        File.WriteAllText(OPTIONS_PATH, saveString);
    }

    private string LoadOptionsFile()
    {
        //If the file isn't exist
        if (!File.Exists(OPTIONS_PATH))
        {
            File.WriteAllText(OPTIONS_PATH, JsonUtility.ToJson(optionsSO));

            //Set device default language
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Spanish:
                    DialogueManager.SetLanguage("es");
                    break;
                case SystemLanguage.English:
                    DialogueManager.SetLanguage("en");
                    break;
                default:
                    DialogueManager.SetLanguage("en");
                    break;
            }
        }

        string json = File.ReadAllText(OPTIONS_PATH);
        return json;
    }
}