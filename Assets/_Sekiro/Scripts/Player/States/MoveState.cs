using UnityEngine;

public class MoveState : IState
{
    private PlayerMovement _player;

    CharacterController _controller;

    public MoveState(PlayerMovement player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }

    public void Enter()
    {

    }

    public void Update()
    {
    }

    public void Exit() { }

    public void HorizonInput(Vector2 moveInput)
    {
        _player.SetHorizontalMove(moveInput);
    }

    public void VerticalInput(Vector3 velocity)
    {
        _player.ChangeState(new JumpState(_player));
        _player.SetVerticalVelocity(velocity.y);
    }
}