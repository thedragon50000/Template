using Zenject;

public class ShibaAI : EnemyAI
{
    
    void Start()
    {
        // _currentState = new EnemyAttackState(this, weaponHitbox, 0.3f, 0.5f);
        _currentState = new EnemyChaseState(this, player.transform);
        _currentState.Enter();
    }

    public override void AttackStateHandler()
    {
        PlayAnimationFromState("Attack01");
    }
    public override void RecoveryStateHandler()
    {
        PlayAnimationFromState("Dizzy");
    }

}