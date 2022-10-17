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
        if(enemy.isMoving == true)
        {
			return TaskStatus.Success;
		}

        if(enemy.enemyData.mobId != 0 && enemy.isMoving == false)
        {
			enemy.isMoving = true;

			float angle = Random.value * (2 * Mathf.PI) - Mathf.PI;

            Vector2 v = new Vector2(1f * Mathf.Cos(angle), 1f * Mathf.Sin(angle));

			enemyRB.velocity = v * 1.5f;

            if (enemyRB.velocity.x > 0) enemySprite.flipX = false;
            if (enemyRB.velocity.x < 0) enemySprite.flipX = true;
		}
		
        return TaskStatus.Failure;
    }
}
