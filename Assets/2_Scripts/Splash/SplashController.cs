using System.Collections;
using UnityEngine;

public class SplashController : MonoBehaviour
{
    [SerializeField] protected Animator transitionAnimator;
    
    void Start()
    {
        StartCoroutine(ExecuteLogoSequence());
    }

    private IEnumerator ExecuteLogoSequence()
    {
		yield return new WaitForSeconds(2f);
        
        SceneLoader.Instance.ChangeScreen(eScreen.MainMenu, false);
        
        yield return new WaitForSeconds(1f);
        
        transitionAnimator.SetTrigger("end");
        
        yield return new WaitForSeconds(1f);
        SceneLoader.Instance.AllowScreenChange();
    }
}
