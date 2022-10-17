using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyMoveScript : Action
{
    public Rigidbody2D enemyRB;
    public EnemyScript enemy;
    public SpriteRenderer enemySprite;

    public override void OnAwake()
    {
		enemy.isMoving = false;
	}

    public override TaskStatus OnUpdate()
    {
        if(enemy.enemyData.mobId != 0 && enemy.isMoving == false)
        {
			enemy.isMoving = true;

			Vector2 v = Random.insideUnitCircle.normalized * 1;

            enemyRB.velocity = v * 1f;

            if (enemyRB.velocity.x > 0) enemySprite.flipX = false;
            if (enemyRB.velocity.x < 0) enemySprite.flipX = true;
		}
		
        return TaskStatus.Failure;
    }
}
