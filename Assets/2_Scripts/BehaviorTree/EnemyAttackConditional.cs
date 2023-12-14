using BehaviorDesigner.Runtime.Tasks;

public class EnemyAttackConditional : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (GetComponent<EnemyScript>().IsAttacking == true)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}

