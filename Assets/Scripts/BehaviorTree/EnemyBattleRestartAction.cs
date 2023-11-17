using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyBattleRestartAction : Action
{
    public EnemyScript enemy;

    public override TaskStatus OnUpdate()
    {
        LevelController.Instance.Player.PlayerDialogueArea.enabled = false;

        //enemy.DialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = false;
        enemy.IsAttacking = false;
        enemy.StartQuestion = false;
        //enemy.DialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = true;

        LevelController.Instance.Player.PlayerDialogueArea.enabled = true;

        return TaskStatus.Failure;
    }
}