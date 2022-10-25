using UnityEngine;

public class OutlineScript : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void OutlineOn()
    {
        animator.SetBool("Outline", true);
	}

	public void OutlineOff()
	{
		animator.SetBool("Outline", false);
	}
}
