using UnityEngine;

public class OutlineScript : MonoBehaviour
{
    Animator animator;
    Material material;

    bool outlineLocked;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        material = this.GetComponent<SpriteRenderer>().material;
        outlineLocked = false;
    }

    public void OutlineLocked()
    {
        outlineLocked = true;
    }

    public void OutlineOn()
    {
        if (outlineLocked == true) return;

        if (this.gameObject.name == "Tower Entry") animator.SetBool("Outline", true);
        else
        {
            material.SetFloat("_OutlineAlpha", 1f);
        }
    }

    public void OutlineOff()
    {
        if (outlineLocked == true) return;

        if (this.gameObject.name == "Tower Entry") animator.SetBool("Outline", false);
        else
        {
            material.SetFloat("_OutlineAlpha", 0f);
        }
    }
}
