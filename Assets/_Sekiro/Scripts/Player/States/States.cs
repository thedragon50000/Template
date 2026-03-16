using UnityEngine;
using System;

public interface IState
{
    void Enter();
    void Update();
    void Exit();

    void HorizonInput(Vector2 moveInput);
    void VerticalInput(Vector3 velocity);

}

public class IdleState : IState
{
    private PlayerMovement _player;

    CharacterController _controller;


    public IdleState(PlayerMovement player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }

    public void Enter() { }

    public void Update()
    {
    }

    public void Exit() { }

    public void HorizonInput(Vector2 moveInput)
    {
        _player.ChangeState(new MoveState(_player));
        _player.SetHorizontalMove(moveInput);
    }

    public void VerticalInput(Vector3 velocity)
    {
        if (_controller.isGrounded)
        {
            _player.ChangeState(new JumpState(_player));
            _player.SetVerticalVelocity(velocity.y);
        }
    }
}
