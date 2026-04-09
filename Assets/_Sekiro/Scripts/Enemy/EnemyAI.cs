using System;
using UnityEngine;
using Zenject;

public abstract class EnemyAI : baseCharacterAnimation
{
    protected IEnemyState _currentState;

    public float recoverTime = 1.5f;
    public bool bCanAttack = false;

    public WeaponHitbox weaponHitbox;

    [Inject] protected PlayerMovement player;

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
    protected void SmoothLookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0;

        // 2. 只有在方向向量有效時才旋轉 (防止目標在正上方或重疊時噴錯)
        if (direction.sqrMagnitude > 0.001f)
        {
            // 3. 計算目標旋轉角度
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 4. 使用 Slerp 進行平滑插值 (5f 是轉向速度，數字越大轉越快)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    internal void StandoffStateHandler()
    {
        throw new NotImplementedException();
    }
}