using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ResetEnemyMoveScript : Action
{
	public EnemyScript enemy;

	public override TaskStatus OnUpdate()
	{
		enemy.isMoving = false;

		return TaskStatus.Failure;
	}
}