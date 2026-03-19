using UnityEngine;
using R3;
using System;

public class ParryState : IState
{
    private bool perfectParry;

    public ParryState(PlayerMovement player) : base(player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }

    public override void Enter()
    {
        _player.DefendStateHandler();
        perfectParry = false;

        // 先等 0.2s 執行 A，再過 0.4s 執行 B
        Observable.Timer(TimeSpan.FromSeconds(0.2f))
            .Do(_ => perfectParry = true)
            .Delay(TimeSpan.FromSeconds(0.2f))
            .Do(_ => perfectParry = false)
            .Delay(TimeSpan.FromSeconds(0.2f))
            .Subscribe(_ =>
            {
                _player.ChangeState(new IdleState(_player));
            })
            .AddTo(_disposables);

        // Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
        // {
        //     // 完美格檔時間結束
        //     perfectParry = true;
        // }).AddTo(_disposables);
        // Observable.Timer(TimeSpan.FromSeconds(0.4f)).Subscribe(_ =>
        // {
        //     // 完美格檔時間結束
        //     _player.ChangeState(new IdleState(_player));
        // }).AddTo(_disposables);
    }

    public override void OnHitbyEnemy(float damage)
    {
        _player.ParryHandler(perfectParry);
    }

    public override void HorizonInput(Vector2 moveInput)
    {
        var direction = moveInput * 0.01f;
        _player.SetHorizontalMove(direction);
    }
}
