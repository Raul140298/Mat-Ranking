using BehaviorDesigner.Runtime.Tasks;

public class ResetEnemyMoveAction : Action
{
	public EnemyScript enemy;

	public override TaskStatus OnUpdate()
	{
		enemy.isMoving = false;

		return TaskStatus.Failure;
	}
}