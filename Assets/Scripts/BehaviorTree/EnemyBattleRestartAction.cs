using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyBattleRestartAction : Action
{
    public EnemyScript enemy;

    public override TaskStatus OnUpdate()
    {
        enemy.DialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = false;
        enemy.IsAttacking = false;
        enemy.StartQuestion = true;
        enemy.DialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = true;

        return TaskStatus.Failure;
    }
}