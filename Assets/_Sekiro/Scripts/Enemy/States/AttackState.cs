using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using R3;

public class EnemyAttackState : IEnemyState
{
    private EnemyAI self;
    StateAnimator selfAnimator;
    private WeaponHitbox _hitbox; // 攻擊時會用到的判定器
    private HashSet<Collider> _hasHit = new HashSet<Collider>(); // 確保一招只打中一次

    public EnemyAttackState(EnemyAI enemy, WeaponHitbox hitbox)
    {
        self = enemy;
        selfAnimator = self.GetComponent<StateAnimator>();
        _hitbox = hitbox;
    }

    public void Enter()
    {
        // selfAnimator.
        Observable.EveryUpdate().Subscribe(_ =>
        {
            // 每一幀主動檢查
            _hitbox.CheckHit(hit =>
            {
                if (!_hasHit.Contains(hit))
                {
                    _hasHit.Add(hit);
                    // 觸發傷害
                    // todo: 架開跟受傷非黑即白，一起處理才對
                    hit.GetComponent<IDamageable>()?.TakeDamage(10);
                }
            });
        });
    }

    public void Exit()
    {
        _hasHit.Clear(); // 離開狀態時清空命中清單，為下一招做準備
    }

    public void Update()
    {
    }
}
