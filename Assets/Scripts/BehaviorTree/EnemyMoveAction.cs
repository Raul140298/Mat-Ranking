using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyMoveAction : Action
{
    public Rigidbody2D enemyRB;
    public EnemyScript enemy;

    public override void OnAwake()
    {
        enemy.IsMoving = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy.IsMoving == true)
        {
            return TaskStatus.Success;
        }

        if (enemy.EnemyData.mobId != 0 && enemy.IsMoving == false)
        {
            enemy.IsMoving = true;

            float angle = Random.value * (2 * Mathf.PI) - Mathf.PI;

            Vector2 v = new Vector2(1f * Mathf.Cos(angle), 1f * Mathf.Sin(angle));


            if (v.magnitude < 0.1f)
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
