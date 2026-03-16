using UnityEngine;

public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();
}

public class MihariState : IEnemyState
{
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

public class AttackState : IEnemyState
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