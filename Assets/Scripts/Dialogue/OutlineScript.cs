using UnityEngine;

public class OutlineScript : MonoBehaviour
{
    Animator animator;
    Material material;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        material = this.GetComponent<SpriteRenderer>().material;
    }

    public void OutlineOn()
    {
        if(this.gameObject.name == "Tower Entry") animator.SetBool("Outline", true);
        else
        {
			material.SetFloat("_OutlineAlpha", 1f);
		}
	}

	public void OutlineOff()
	{
		if (this.gameObject.name == "Tower Entry") animator.SetBool("Outline", false);
		else
		{
			material.SetFloat("_OutlineAlpha", 0f);
		}
	}
}
