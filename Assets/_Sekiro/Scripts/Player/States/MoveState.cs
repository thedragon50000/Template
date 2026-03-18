using UnityEngine;

public class MoveState : IState
{

    public MoveState(PlayerMovement player) : base(player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }

    public override void Enter()
    {
        Debug.Log("移動動畫loop");
    }

    public override void Exit()
    {
        Debug.Log("exit moveState");
    }

    public override void HorizonInput(Vector2 moveInput)
    {
        _player.SetHorizontalMove(moveInput);
        if (moveInput.sqrMagnitude < 0.01f)
        {
            _player.ChangeState(new IdleState(_player));
        }
    }

    public override void VerticalInput(Vector3 velocity)
    {
        _player.ChangeState(new JumpState(_player));
        _player.SetVerticalVelocity(velocity.y);
    }

    public override void OnHitbyEnemy(float damage)
    {
        _player.TakeDamageHandler(damage);
    }

    public override void GuardInput()
    {
        _player.ChangeState(new ParryState(_player));
    }
}