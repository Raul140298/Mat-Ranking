using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	public Animator transitionAnimator;
	public FromLevelSO fromLevelSO;
	public SaveSystemScript saveSystem;

	private void Start()
	{
		fromLevelSO.fromLevel = false;
		Init();
	}

	public void Init()
	{
		StartCoroutine(init());
	}

	IEnumerator init()
	{
		yield return new WaitForSeconds(0.1f);
		// Clear Player prefs before BUILD
		// PlayerPrefs.SetInt("firstTimeInstalled", 0);
		// We load the local data
		saveSystem.loadOptions();

		// We load the remote data
		// download jsonRemote.json and change local data
		// saveSystem.downloadRemote();// -> GET jsonRemote.json if had internet
		SoundtracksScript.PlaySoundtrack("GARDEN OF MATH");
	}


	public void LoadAdventure()
    {
        StartCoroutine(loadAdventure(1f));
    }

	IEnumerator loadAdventure(float transitionTime)
	{
		transitionAnimator.SetTrigger("end");
		yield return new WaitForSeconds(transitionTime);
		SceneManager.LoadScene(1);
	}

	public void Exit()
	{
        Application.Quit();
    }
}