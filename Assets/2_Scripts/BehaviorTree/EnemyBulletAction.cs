using BehaviorDesigner.Runtime.Tasks;

public class EnemyBulletAction : Action
{
    public EnemyScript enemy;

    public override TaskStatus OnUpdate()
    {
        enemy.ShootBullets();

        return TaskStatus.Success;
    }
}