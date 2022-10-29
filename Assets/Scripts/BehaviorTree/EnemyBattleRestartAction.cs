using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyBattleRestartAction : Action
{
	public EnemyScript enemy;

	public override TaskStatus OnUpdate()
	{
		enemy.dialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = false;
		enemy.isAttacking = false;
		enemy.startQuestion = true;
		enemy.dialogueSystemTrigger.GetComponent<CircleCollider2D>().enabled = true;

		return TaskStatus.Failure;
	}
}