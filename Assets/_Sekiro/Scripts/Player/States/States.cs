using UnityEngine;
using System;
using R3;

public abstract class IState
{
    protected PlayerMovement _player;
    protected CharacterController _controller;

    // 用來管理所有訂閱，確保隨時能取消
    protected CompositeDisposable _disposables = new CompositeDisposable();

    public IState(PlayerMovement player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }
    public virtual void Enter()
    {

    }
    public virtual void Update() { }
    public virtual void Exit()
    {
        _disposables.Dispose();
    }
    public virtual void HorizonInput(Vector2 moveInput)
    {
        // 預設為不動，因為大部分都不會動
        moveInput = Vector2.zero;
        _player.SetHorizontalMove(moveInput);
    }
    public virtual void VerticalInput(Vector3 velocity)
    {
        // 預設為不動，因為大部分都不能跳
        _player.SetVerticalVelocity(0);
    }
    public virtual void OnHitbyEnemy(float damage) { }
    public virtual void GuardInput() { }
}

public class IdleState : IState
{

    public IdleState(PlayerMovement player) : base(player)
    {
    }

    public override void Enter()
    {
        _player.IdleStateHandler();
    }

    public override void HorizonInput(Vector2 moveInput)
    {
        _player.ChangeState(new MoveState(_player));
        _player.SetHorizontalMove(moveInput);
    }

    public override void VerticalInput(Vector3 velocity)
    {
        if (_controller.isGrounded)
        {
            _player.ChangeState(new JumpState(_player));
            _player.SetVerticalVelocity(velocity.y);
        }
    }

    public override void OnHitbyEnemy(float damage)
    {
        _player.TakeDamageHandler(damage);
        Debug.Log("站著拉完了");
    }

    public override void GuardInput()
    {
        Debug.Log("idle進防守");
        _player.ChangeState(new ParryState(_player));
    }
}
