using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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
        // 初始狀態為 Idle
        ChangeState(new IdleState(this));
    }

    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    public void OnMove(InputAction.CallbackContext ctx) // 接收 Input System 訊號
    {
        _moveInput = ctx.ReadValue<Vector2>();
        Debug.Log($"axis: {_moveInput}");
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _velocity = Vector3.up * jumpForce;
            ChangeState(new JumpState(this, _velocity));
            Debug.Log("OnJump");
        }
    }

    void Update()
    {
        _currentState.Update();

        MoveUpdate();

    }

    private void MoveUpdate()
    {
        if (_moveInput == Vector2.zero) return;

        // 計算相對於攝影機的前後左右
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; // 忽略垂直方向
        right.y = 0;

        Vector3 moveDirection = (forward * _moveInput.y + right * _moveInput.x).normalized;

        // 移動角色
        _controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 角色轉向移動方向
        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.15f);
        }
    }
}