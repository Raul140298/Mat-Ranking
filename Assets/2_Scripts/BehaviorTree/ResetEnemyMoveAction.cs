using BehaviorDesigner.Runtime.Tasks;

public class ResetEnemyMoveAction : Action
{
    public EnemyModelScript EnemyModel;

    public override TaskStatus OnUpdate()
    {
        EnemyModel.IsMoving = false;

        return TaskStatus.Failure;
    }
}