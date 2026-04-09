using System;
using System.Threading;
using R3;
using UnityEngine;
using Zenject;

public abstract class IEnemyState
{
    protected EnemyAI self;

    protected CompositeDisposable _disposables = new();

    // 玩家
    [Inject] protected PlayerMovement player;

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

    protected void SmoothLookAtPlayer()
    {
        Vector3 direction = player.transform.position - self.transform.position;
        direction.y = 0;

        // 2. 只有在方向向量有效時才旋轉 (防止目標在正上方或重疊時噴錯)
        if (direction.sqrMagnitude > 0.001f)
        {
            // 3. 計算目標旋轉角度
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 4. 使用 Slerp 進行平滑插值 (5f 是轉向速度，數字越大轉越快)
            self.transform.rotation = Quaternion.Slerp(self.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
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