using System;
using UnityEngine;

public class EnemyAI : baseCharacterAnimation
{
    IEnemyState _currentState;
    public float recoverTime = 1.5f;

    public WeaponHitbox weaponHitbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentState = new EnemyAttackState(this, weaponHitbox, 0.3f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeState(IEnemyState nextState)
    {
        _currentState.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }

    public override void AttackStateHandler()
    {
    }
}