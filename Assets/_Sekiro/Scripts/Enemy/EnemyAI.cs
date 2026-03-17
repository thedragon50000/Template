using System;
using UnityEngine;

public abstract class EnemyAI : baseCharacterAnimation
{
    protected IEnemyState _currentState;
    public float recoverTime = 1.5f;

    public WeaponHitbox weaponHitbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    public virtual void AttackStateHandler()
    {
    }
    public virtual void RecoveryStateHandler()
    {
    }

}