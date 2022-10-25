using BehaviorDesigner.Runtime.Tasks;

public class EnemyBattleConditional : Conditional
{
	public override TaskStatus OnUpdate()
	{
		if(GetComponent<EnemyScript>().startQuestion == true)
		{
			return TaskStatus.Success;
		}
		else
		{
			return TaskStatus.Failure;
		}
	}
}

