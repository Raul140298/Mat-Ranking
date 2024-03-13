using BehaviorDesigner.Runtime.Tasks;

public class EnemyBulletAction : Action
{
    public EnemyModelScript EnemyModel;

    public override TaskStatus OnUpdate()
    {
        EnemyModel.ShootBullets();

        return TaskStatus.Success;
    }
}