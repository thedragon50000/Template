using System;
using R3;

public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();
}

public class MihariState : IEnemyState
{
    EnemyAI self;

    public MihariState(EnemyAI enemy)
    {
        self = enemy;
    }

    public void Enter()
    {
        // todo: 開啟視線、感應等collider
        throw new System.NotImplementedException();
    }
    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class ChaseState : IEnemyState
{
    public void Enter()
    {
        throw new System.NotImplementedException();
    }
    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}

/// <summary>
/// 硬直狀態
/// </summary>
public class RecoveryState : IEnemyState
{
    EnemyAI self;
    public RecoveryState(EnemyAI enemy)
    {
        self = enemy;
        
    }
    public void Enter()
    {

        Observable.Timer(TimeSpan.FromSeconds(self.recoverTime))
        .Subscribe(_ =>
            {
                self.ChangeState(new MihariState(self));
            }
        );
    }
    public void Update()
    {
        
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

}