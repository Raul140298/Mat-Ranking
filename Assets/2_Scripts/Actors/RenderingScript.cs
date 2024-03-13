using PowerTools;
using UnityEngine;

public class RenderingScript : MonoBehaviour
{
	[SerializeField] private ActorModelScript model;
	
    [Header("INFO")] 
	[SerializeField] private eAnimation currentAnimation;

    [Header("COMPONENTS")]
    [SerializeField] private SpriteRenderer compRnd;
    [SerializeField] private SpriteAnim compAnim;
    
    [Header("ANIMATIONS")]
    [SerializeField] private AnimationData animData;

    public void PlayAnimation(eAnimation nextAnimation)
    {
    	if (ShouldSkipAnimation(nextAnimation))
    	{
    		return;
    	}

    	currentAnimation = nextAnimation;

		switch (nextAnimation)
		{
			case eAnimation.Idle:
				
				SetOrientation(model.LastDirection);
				
				compAnim.Play(animData.animations[nextAnimation].animations[model.LastDirection]);
				break;
			
			case eAnimation.Walk:

				SetOrientation(model.Direction);
				
				compAnim.Play(animData.animations[nextAnimation].animations[model.Direction]);
				break;
			
			/*case eAnimation.Dash:
				isDashing = true;
				SetOrientation(model.Direction);
				compAnim.Play(animData.animations[nextAnimation].animations[model.Direction]);
				Invoke("DashEnd", 0.4f);
				break;*/
			
			/*case eAnimation.Attack:
				isAttacking = true;
				SetOrientation(model.Direction);
				compAnim.Play(animData.animations[nextAnimation].animations[model.Direction]);
				Invoke("AttackEnd", 0.9f);
				break;*/
		}
    }


    public void Show()
    {
    	compRnd.ShowSprite();
    }

    public void Hide()
    {
    	compRnd.HideSprite();
    }

    
    private bool ShouldSkipAnimation(eAnimation nextAnimation)
    {
    	return (currentAnimation == nextAnimation && model.LastDirection == model.Direction ||
				model.IsThrowingProjectile || model.IsDashing || model.IsAttacking);
    }

    public eAnimation GetCurrentAnimation()
    {
    	return currentAnimation;
    }

    public void SetOrientation(eDirection dir)
    {
    	bool left = (dir == eDirection.Left);
    	compRnd.flipX = left;
    }
	
	
	public void FlipX(bool isLookingPlayer)
	{
        
	}

	public void OutlineOn()
	{
        
	}

	public void OutlineOff()
	{
        
	}

	public void OutlineLocked()
	{
		
	}

	public void SetAnimData(AnimationData data)
	{
		animData = data;
	}
}
