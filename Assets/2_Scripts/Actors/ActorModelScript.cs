using UnityEngine;

public class ActorModelScript : MonoBehaviour
{
    [Header("INFO")]
    [SerializeField] protected RenderingScript compRendering;
	protected eDirection lastDirection;
	protected eDirection direction;
    protected bool isThrowingProjectile = false;
	protected bool isAttacking = false;
	protected bool isDashing = false;
    
    public void SetDirection(eDirection dir)
    {
        lastDirection = direction;
        direction = dir;
    }
    		
    protected eDirection GetDirectionFromVector(Vector2 vector)
    {
    	float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

    	if (angle > -45 && angle < 45)
    	{
    		return eDirection.Right;
    	}
    	else if (angle >= 45 && angle <= 135)
    	{
    		return eDirection.Up;
    	}
    	else if (angle > 135 || angle < -135)
    	{
    		return eDirection.Left;
    	}
    	else
    	{
    		return eDirection.Down;
    	}
    }

    protected void LookTarget(GameObject target)
    {
        if (this.gameObject.transform.position.x > target.gameObject.transform.position.x)
        {
            if (target.tag == "Enemy") target.transform.parent.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x)
        {
            if (target.tag == "Enemy") target.transform.parent.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    
	protected void ThrowingProjectileEnd()
    {
        isThrowingProjectile = false;
    }
	
	protected void AttackEnd()
    {
        isAttacking = false;
    }
	
	protected void DashEnd()
    {
        isDashing = false;
    }

    public bool IsThrowingProjectile => isThrowingProjectile;
    public bool IsDashing => isDashing;
	public RenderingScript CompRendering => compRendering;
	public eDirection LastDirection => lastDirection;
	public eDirection Direction => direction;
    
	public bool IsAttacking
	{
		get { return isAttacking; }
		set { isAttacking = value; }
	}
}
