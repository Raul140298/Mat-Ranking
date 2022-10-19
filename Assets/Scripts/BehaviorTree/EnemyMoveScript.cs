using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyMoveScript : Action
{
    public Rigidbody2D enemyRB;
    public EnemyScript enemy;

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


            if(v.magnitude < 0.1f)
            {
                enemyRB.velocity = Vector2.zero;
            }
            else
            {
				enemyRB.velocity = v * 1.5f;
			}
		}
		
        return TaskStatus.Failure;
    }
}
