using System.Collections.Generic;
using PowerTools;
using Sirenix.OdinInspector;
using UnityEngine;

public class RenderingScript : SerializedMonoBehaviour
{
    [Header("INFO")]
    [SerializeField] private eAnimation currentAnimation;

    [Header("COMPONENTS")]
    [SerializeField] protected SpriteRenderer compRnd;
    [SerializeField] protected SpriteAnim compAnim;

    [Header("ANIMATIONS")]
    [SerializeField] private Dictionary<eAnimation, AnimationClip> animations;

    Animator animator;
    Material material;

    bool outlineLocked;

    private void Awake()
    {
        material = compRnd.material;
        outlineLocked = false;
    }

    public void SetAnimations(Dictionary<eAnimation, AnimationClip> animations)
    {
        this.animations = animations;
    }

    public void PlayAnimation(eAnimation nextAnimation)
    {
        if (compAnim.IsPaused())
        {
            compAnim.Resume();
        }

        if (currentAnimation == nextAnimation) return;

        currentAnimation = nextAnimation;

        compAnim.Play(animations[nextAnimation]);
    }

    public void OutlineLocked()
    {
        outlineLocked = true;
    }

    public void OutlineOn()
    {
        if (outlineLocked == true) return;

        material.ShowOutline();
    }

    public void OutlineOff()
    {
        if (outlineLocked == true) return;

        material.HideOutline();
    }

    public void FlipX(bool b)
    {
        compRnd.flipX = b;
    }
}
