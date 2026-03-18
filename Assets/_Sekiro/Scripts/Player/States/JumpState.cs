using UnityEngine;
using R3;

public class JumpState : IState
{
    // private float _gravity = -9.81f; // 自定義重力
    private bool isHit;

    public JumpState(PlayerMovement player) : base(player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }

    public override void Enter()
    {
        // 狀態偵測流：監測著地
        Observable.EveryUpdate()
            .Where(_ => _controller.isGrounded)
            // .Skip(1)
            .Take(1) // 只取第一次滿足條件就結束流
            .Subscribe(_ =>
            {
                if (isHit == true)
                {
                    _player.ChangeState(new StunnedState(_player, 1));
                }
                _player.ChangeState(new IdleState(_player));
            })
            .AddTo(_disposables);
    }

    public override void HorizonInput(Vector2 moveInput)
    {
        // 只能微調
        _player.SetHorizontalMove(moveInput * 0.3f);
    }


    public override void OnHitbyEnemy(float damage)
    {
        isHit = true;
        // todo: which animation to play?
    }

    public override void GuardInput()
    {
        Debug.Log("空中防禦？");
    }
}
