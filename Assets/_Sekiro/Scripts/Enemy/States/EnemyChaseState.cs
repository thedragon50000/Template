using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using R3;
using UnityEngine.AI;


public class EnemyChaseState : IEnemyState
{
    private float _delay;
    private float _duration;
    NavMeshAgent agent;
    private Transform _target;


    public EnemyChaseState(EnemyAI enemy, Transform target) : base(enemy)
    {
        agent = enemy.GetComponent<NavMeshAgent>();
        _target = target;
    }

    public override void Enter()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
        {
            if (_target != null)
            {
                agent.SetDestination(_target.position);
            }
            else
            {
                self.ChangeState(new MihariState(self));
            }

        }).AddTo(_disposables);

        // 每一幀檢查距離
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                float dist = Vector3.Distance(self.transform.position, _target.transform.position);

                // 1. 到達攻擊範圍 -> 切換到攻擊或準備狀態
                if (dist <= agent.stoppingDistance)
                {
                    // todo: 
                    if (self.bCanAttack)
                    {
                        self.ChangeState(new EnemyAttackState(self, self.weaponHitbox, 0.3f, 0.5f));
                    }
                    else
                    {
                        self.ChangeState(new EnemyAttackState(self, self.weaponHitbox, 0.3f, 0.5f));
                    }

                }

                // 2. (選做) 玩家跑太遠 -> 脫戰回歸 Idle
                if (dist > 15f)
                {
                    self.ChangeState(new MihariState(self));
                }
            })
            .AddTo(_disposables);
    }
    protected override void Update()
    {
        // Debug.Log($"isOnNavMesh:{agent.isOnNavMesh}");
    }

    public override void Exit()
    {
        base.Exit();
        agent.ResetPath();
    }

}