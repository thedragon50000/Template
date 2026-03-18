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
        perfectParry = true;
        Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
        {
            // 完美格檔時間結束
            perfectParry = false;
        }).AddTo(_disposables);
        Observable.Timer(TimeSpan.FromSeconds(0.7f)).Subscribe(_ =>
        {
            // 完美格檔時間結束
            _player.ChangeState(new IdleState(_player));
        }).AddTo(_disposables);
    }

    public override void OnHitbyEnemy(float damage)
    {
        _player.ParryHandler(perfectParry);
    }
}
