using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
// using Zenject;

public class PlayerMovement : baseCharacterAnimation, IDamageable
{
    public float moveSpeed = 5f;
    public Transform cameraTransform; // 拖入 Main Camera
    private CharacterController _controller;
    private Vector2 _moveInput;

    private Vector3 _velocity; // 這裡存的是垂直速度

    private float _gravity = -9.81f; // 自定義重力
    private float jumpForce = 8;

    // ReactiveProperty<float> atkPoint;

    private IState _currentState;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    void Start()
    {
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

        var total = moveDirection * moveSpeed + _velocity;

        // 移動角色
        _controller.Move(total * Time.deltaTime);

        // 角色轉向移動方向
        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.15f);
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
            ChangeState(new IdleState(this));
        }
        else
        {
            PlayAnimationFromState("Parry");
            ChangeState(new StunnedState(this, 0.8f));
        }
    }

    public void TakeDamageHandler(float damage)
    {
        if (damage > 50)
        {
            PlayAnimationFromState("Down02");

            ChangeState(new StunnedState(this, 1f));

        }
        else
        {
            PlayAnimationFromState("hit_body");
            ChangeState(new StunnedState(this, 0.5f));
        }

    }

    public void IdleStateHandler()
    {
        PlayAnimationFromState("idle01");
    }
}