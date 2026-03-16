using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using R3;

public class EnemyAttackState : IEnemyState
{
    private EnemyAI self;
    private WeaponHitbox _hitbox; // 攻擊時會用到的判定器
    private HashSet<Collider> _hasHit = new(); // 確保一招只打中一次
    private CompositeDisposable _disposables = new();
    private float _delay;
    private float _duration;

    public EnemyAttackState(EnemyAI enemy, WeaponHitbox hitbox, float delay, float duration)
    {
        self = enemy;
        _hitbox = hitbox;
        _delay = delay;
        _duration = duration;
    }

    public void Enter()
    {
        self.AttackStateHandler();
        StartAttackLogic(_delay, _duration);
    }

    private void StartAttackLogic(float delay, float duration)
    {
        Observable.Timer(TimeSpan.FromSeconds(delay))
            .Subscribe(_ =>
            {
                // 進入判定視窗
                Observable.EveryUpdate()
                    .Take(TimeSpan.FromSeconds(duration))
                    .Subscribe(_ => CheckHitbox())
                    .AddTo(_disposables); // 關鍵：綁定到狀態生命週期
            })
            .AddTo(_disposables);
    }

    private void CheckHitbox()
    {
        _hitbox.CheckHit(hit =>
        {
            if (!_hasHit.Contains(hit))
            {
                _hasHit.Add(hit);
                // todo: 格檔與受傷的判斷中心。架開跟受傷非黑即白，一起處理才對

                // hit.GetComponent<IDamageable>()?.HandleCombatInteraction(_player);
            }
        });
    }

    public void Exit()
    {
        _disposables.Dispose();
        _hasHit.Clear(); // 離開狀態時清空命中清單，為下一招做準備
    }

    public void Update()
    {
    }
}