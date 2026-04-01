using System;
using System.Threading;
using R3;

public abstract class IEnemyState
{
    protected EnemyAI self;

    protected CompositeDisposable _disposables = new();

    public IEnemyState(EnemyAI enemy)
    {
        self = enemy;
    }

    public virtual void Enter()
    {

    }
    protected virtual void Update()
    {

    }
    public virtual void Exit()
    {
        _disposables.Dispose();
    }
}

public class MihariState : IEnemyState
{

    public MihariState(EnemyAI enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        // todo: 開啟視線、感應等collider
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

/// <summary>
/// 硬直狀態
/// </summary>
public class RecoveryState : IEnemyState
{
    EnemyAI self;

    public RecoveryState(EnemyAI enemy) : base(enemy)
    {
        self = enemy;

    }
    public override void Enter()
    {
        self.RecoveryStateHandler();
        Observable.Timer(TimeSpan.FromSeconds(self.recoverTime))
        .Subscribe(_ =>
            {
                // self.ChangeState(new MihariState(self));

                // todo: 暫定為休息完馬上砍，先做完傷害跟架開功能為優先
                self.ChangeState(new EnemyAttackState(self, self.weaponHitbox, 0.3f, 0.5f));
            }
        ).AddTo(_disposables);
    }

    public override void Exit()
    {
        base.Exit();
    }

}