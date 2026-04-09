using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMovement : baseCharacterAnimation, IDamageable
{
    float moveSpeed = 5f;
    float backOffSpeed = 1;
    float isBackOffSpeed = 4;

    public Transform cameraTransform; // 拖入 Main Camera
    private CharacterController _controller;
    private Vector2 _moveInput;

    private Vector3 _velocity; // 這裡存的是垂直速度

    private float _gravity = -9.81f; // 自定義重力
    private float jumpForce = 8;

    // public CinemachineCamera lockCamera;
    public CinemachineClearShot clearShot;
    // ReactiveProperty<float> atkPoint;

    private IState _currentState;

    [Inject]    // Warning: Zenject not installed yet.
    LockCameraPosition lockCameraPosition;

    private bool _lockingMode = false;

    public bool LockingMode
    {
        get => _lockingMode;
        private set
        {
            if (_lockingMode == value) return;

            var p = value == true ? 10 : 0;
            _lockingMode = value;

            clearShot.Priority = p;
        }
    }
    // 鎖定的對象
    public ReactiveProperty<Collider> Target = new(null);


    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        // 當 Target 變動時觸發
        Target.Subscribe(newTarget =>
        {
            if (newTarget == null)
            {
                Debug.Log("目標遺失，停止鎖定邏輯");

                lockCameraPosition.target = null;

                LockingMode = false;
            }
            else
            {
                Debug.Log($"瞄準新目標: {newTarget.name}");
                lockCameraPosition.target = newTarget.transform;

                CinemachineCamera c = clearShot.ChildCameras[0] as CinemachineCamera;
                c.Target.TrackingTarget = newTarget.transform;
                
                LockingMode = true;
            }
        });

        // 將傳統 Update 轉為響應式流
        Observable.EveryUpdate(destroyCancellationToken)
            .Subscribe(_ =>
            {
                // 1. 處理狀態機的更新邏輯
                _currentState.Update();

                // 2. 處理移動物理計算
                TranslationUpdate();
            })
            .AddTo(this);

        // 初始狀態為 Idle
        ChangeState(new IdleState(this));
    }

    public void ChangeState(IState newState)
    {
        Debug.Log($"new State: {newState}");
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    // 讓 State 呼叫這個來告訴主體：「我想往這跑」
    public void SetHorizontalMove(Vector3 direction)
    {
        _moveInput = direction;
    }

    // 讓 State 呼叫這個來告訴主體：「我想跳」
    public void SetVerticalVelocity(float y)
    {
        _velocity = Vector3.up * y;
    }

    public void OnMove(InputAction.CallbackContext ctx) // 接收 Input System 訊號
    {
        _moveInput = ctx.ReadValue<Vector2>();
        _currentState.HorizonInput(_moveInput);
        // Debug.Log($"axis: {_moveInput}");
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _currentState.VerticalInput(Vector3.up * jumpForce);
        }
    }

    public void OnGuard(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("OnGuard press");
            _currentState.GuardInput();
        }
    }

    public void OnLock(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnLock press");

        if (ctx.performed)
        {
            if (LockingMode)
            {
                // 已在鎖定中就解除
                LockingMode = false;
                return;
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, 10);

            if (hits.Length == 0)
            {
                LockingMode = false;
                Debug.Log("附近沒有敵人");
                return;
            }

            // 1. 先把所有的敵人撈出來並按距離排序（只做一次）
            var allEnemies = hits
                .Where(h => h.CompareTag("Enemy"))
                .OrderBy(h => (h.transform.position - transform.position).sqrMagnitude) // 使用 sqrMagnitude 比 Distance 快，因為不用開根號
                .ToList();

            // 過濾出角色前方 150 度內的敵人
            Collider validEnemy = allEnemies.FirstOrDefault(h =>
                Vector3.Angle(transform.forward, h.transform.position - transform.position) < 75f);

            if (validEnemy == null)
            {
                // 退而求次找範圍內的
                validEnemy = allEnemies.FirstOrDefault();
            }

            if (validEnemy != null)
            {
                LockingMode = true;
                SetLockTarget(validEnemy);
            }
            else
            {
                LockingMode = false;
                SetLockTarget(null);
                Debug.Log("附近有人但沒有敵人");
            }
        }
    }

    private void SetLockTarget(Collider collider)
    {
        Target.Value = collider;
    }

    // void Update()
    // {

    //     // _currentState.Update();

    //     // TranslationUpdate();

    // }


    private void TranslationUpdate()
    {
        // 計算相對於攝影機的前後左右
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; // 忽略垂直方向
        right.y = 0;

        Vector3 moveDirection = (forward * _moveInput.y + right * _moveInput.x).normalized;

        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 給一個小小的負值，確保它緊貼地面
        }

        // 加上重力加速度
        _velocity.y += _gravity * Time.deltaTime;

        // 被擊飛的時候 backOffSpeed 會變大，也就是會加速
        var total = moveDirection * moveSpeed * backOffSpeed + _velocity;

        // 移動角色
        _controller.Move(total * Time.deltaTime);

        // 角色轉向移動方向
        if (moveDirection != Vector3.zero)
        {
            if (backOffSpeed == 1)
            {
                transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.15f);
            }
            else
            {
                transform.forward = -moveDirection;
            }
        }
    }

    public void CalculateDamage(float enemyAtk)
    {
        Debug.Log("Player OnHit !");
        _currentState.OnHitbyEnemy(enemyAtk);
    }

    public void DefendStateHandler()
    {
        PlayAnimationFromState("Defense02");
    }

    public void ParryHandler(bool perfect)
    {
        if (perfect)
        {
            PlayAnimationFromState("Parry");
            BackOff(4, 0.2f);
            ChangeState(new StunnedState(this, 0.3f));  // Note: 敵人攻擊間隔不能小於這個值，不然就不是格檔遊戲了
        }
        else
        {
            PlayAnimationFromState("Parry");
            // todo: 有更大的硬直，累積失衡值（尚未實作）
            BackOff(3, 0.2f);
            ChangeState(new StunnedState(this, 0.8f));
        }
    }

    /// <summary>
    /// 擊退、擊飛
    /// </summary>
    /// <param name="force"></param>
    public void BackOff(float force, float delay)
    {
        _moveInput = Vector2.down * force;

        Observable.Return(Unit.Default).Do(
        _ => backOffSpeed = force)
        .Delay(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
        {
            backOffSpeed = 1;
            _moveInput = Vector2.zero;
        }
        );

    }

    public void TakeDamageHandler(float damage)
    {
        if (damage > 90)
        {
            PlayAnimationFromState("Down02");
            cameraTransform.DOPunchPosition(cameraTransform.right, 0.3f);

            ChangeState(new StunnedState(this, 1f));

        }
        else
        {
            PlayAnimationFromState("hit_body");
            cameraTransform.DOPunchPosition(cameraTransform.right * 50, 0.3f);
            ChangeState(new StunnedState(this, 0.5f));
        }

    }

    public void IdleStateHandler()
    {
        PlayAnimationFromState("idle01");
    }
}