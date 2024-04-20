using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyBattleRestartAction : Action
{
    public EnemyModelScript EnemyModel;

    public override TaskStatus OnUpdate()
    {
        //LevelController.Instance.Player.PlayerDialogueArea.enabled = false;

        //enemy.DialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = false;
        EnemyModel.IsAttacking = false;
        EnemyModel.StartQuestion = false;
        //enemy.DialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = true;

        //LevelController.Instance.Player.PlayerDialogueArea.enabled = true;

        return TaskStatus.Failure;
    }
}