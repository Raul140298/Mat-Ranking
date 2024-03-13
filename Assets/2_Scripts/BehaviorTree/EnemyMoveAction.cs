using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyMoveAction : Action
{
    public Rigidbody2D enemyRB;
    public EnemyModelScript EnemyModel;

    public override void OnAwake()
    {
        EnemyModel.IsMoving = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (EnemyModel.IsMoving)
        {
            return TaskStatus.Success;
        }

        if (EnemyModel.EnemyData.mobId != 0 && !EnemyModel.IsMoving)
        {
            EnemyModel.IsMoving = true;

            float angle = Random.value * (2 * Mathf.PI) - Mathf.PI;

            Vector2 v = new Vector2(1f * Mathf.Cos(angle), 1f * Mathf.Sin(angle));


            if (v.magnitude < 0.1f)
            {
                enemyRB.velocity = Vector2.zero;
            }
            else
            {
                enemyRB.velocity = v * EnemyModel.Velocity;
            }
        }

        return TaskStatus.Failure;
    }
}
