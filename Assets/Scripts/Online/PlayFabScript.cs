using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabScript : MonoBehaviour
{
    public GameObject row;
    public Transform rowsParent;
    public GameObject ranking, nameCreation, bottomBar;
    public Text nameInput;
    public PlayerSO player;
    public SaveSystemScript saveSystem;
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)//Main Menu
		{
            StartCoroutine(login());
		}
        
        if (SceneManager.GetActiveScene().buildIndex == 1)
		{
            if (player.name == null || player.name == "" || player.name == " ")
            {
                ranking.SetActive(false);
                nameCreation.SetActive(true);
            }
            else
            {
                nameCreation.SetActive(false);
                ranking.SetActive(true);
            }
        }
    }

    IEnumerator login()
	{
        yield return new WaitForSeconds(0.1f);
        Login();
	}

    void Login()
	{
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
			{
                GetPlayerProfile = true,
			} 
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
	{
        Debug.Log("Successful login/account created!");
        player.name = null;

        if (result.InfoResultPayload.PlayerProfile != null)
		{
            player.name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }       
	}

    public void SendRanking(int score)
	{
        StartCoroutine(sendRanking(score));
    }
  

    IEnumerator sendRanking(int score)
    {
        yield return new WaitForSeconds(0.1f);
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Ranking",
                    Value = score,
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnRankingUpdate, OnError);
    }

    void OnRankingUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful ranking send!");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account");
        Debug.Log(error.GenerateErrorReport());
    }

    public void GetRanking()
	{
        StartCoroutine(getRanking());
    }

    IEnumerator getRanking()
    {
        yield return new WaitForSeconds(0.1f);
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Ranking",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnRankingGet, OnError);
    }

    void OnRankingGet(GetLeaderboardResult result)
	{
        foreach(Transform item in rowsParent)
		{
            Destroy(item.gameObject);
		}

        foreach(var item in result.Leaderboard)
		{
            GameObject newGO = Instantiate(row, rowsParent);
            Text[] texts = newGO.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();
		}
    }

    public void SubmitNameButton()
	{
        StartCoroutine(submitNameButton());
	}

    IEnumerator submitNameButton()
	{
        yield return new WaitForSeconds(0.1f);
        if (nameInput.text != null && nameInput.text != "" && nameInput.text != " ")
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = nameInput.text,
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
        }
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
	{
        Debug.Log("Updated display name!");

        player.name = nameInput.text;
        saveSystem.saveLocal();

        if (player.name == null || player.name == "" || player.name == " ")
        {
            ranking.SetActive(false);
            nameCreation.SetActive(true);
        }
        else
        {
            nameCreation.SetActive(false);
            ranking.SetActive(true);
        }

        bottomBar.SetActive(true);      
    }
}
