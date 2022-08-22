using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionsScript : MonoBehaviour
{
	public TextAsset textJSON;
	public Animator transitionAnimator;
	public float transitionTime;

	private static TransitionsScript instance;
	private Text phrase, author;

	[System.Serializable]
	public class Phrase
	{
		public string id;
		public string autor;
		public string frase;
	}

	[System.Serializable]
	public class PhraseList
	{
		public Phrase[] phrases;
	}

	public PhraseList myPhraseList = new PhraseList();

	void Start()
	{
		instance = this;
		transitionAnimator = this.GetComponent<Animator>();
		myPhraseList = JsonUtility.FromJson<PhraseList>(textJSON.text);
		phrase = this.transform.GetChild(0).GetComponent<Text>();
		author = this.transform.GetChild(1).GetComponent<Text>();
	}

	public static void StartTransition()
	{
		instance.StartCoroutine(instance.startTrasition(instance.transitionTime));
	}

	IEnumerator startTrasition(float transitionTime)
	{
		int n = UnityEngine.Random.Range(0, myPhraseList.phrases.Length);
		phrase.text = '"' + myPhraseList.phrases[n].frase + '.' + '"';
		author.text = myPhraseList.phrases[n].autor;
		transitionAnimator.SetTrigger("start");
		yield return new WaitForSeconds(transitionTime);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
