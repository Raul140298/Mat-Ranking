using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyBulletScript : Action
{
	public EnemyScript enemy;

	public override TaskStatus OnUpdate()
	{
		enemy.ShootBullets();

		return TaskStatus.Success;
	}
}