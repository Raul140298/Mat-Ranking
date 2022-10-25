using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyBattleRestartAction : Action
{
	public EnemyScript enemy;

	public override TaskStatus OnUpdate()
	{
		enemy.isAttacking = false;
		enemy.startQuestion = true;

		return TaskStatus.Failure;
	}
}