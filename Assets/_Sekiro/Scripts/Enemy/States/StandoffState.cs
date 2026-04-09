using System;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class StandoffState : IEnemyState
{
    private float _strafeDirection; // 1 或 -1 (左或右)
    private float _standoffDuration;

    public StandoffState(EnemyAI enemy) : base(enemy)
    {

    }

    public override void Enter()
    {
        _strafeDirection = Random.value > 0.5f ? 1 : -1;
        _standoffDuration = Random.Range(1.5f, 3.0f); // 對峙時間

        // 隨機切換左右橫移方向
        Observable.Interval(TimeSpan.FromSeconds(1.5f))
            .Subscribe(_ => _strafeDirection *= -1)
            .AddTo(_disposables);

        // 時間到就切換回追逐或發動攻擊
        Observable.Timer(TimeSpan.FromSeconds(_standoffDuration))
            .Subscribe(_ => DecideNextMove())
            .AddTo(_disposables);
    }

    protected override void Update()
    {
        SmoothLookAtPlayer();

        // 2. 緩慢橫移邏輯 (使用 transform.Translate 或 agent.Move)
        Vector3 strafeVec = self.transform.right * _strafeDirection * 1.5f * Time.deltaTime;
        self.transform.position += strafeVec;

        // 3. 更新動畫 (傳遞橫移速度給 Animator)
        self.StandoffStateHandler();
    }


    private void DecideNextMove()
    {
        // 決策：是要繼續衝向玩家？還是直接開砍？
        self.ChangeState(new EnemyChaseState(self, player.transform));
    }

}
