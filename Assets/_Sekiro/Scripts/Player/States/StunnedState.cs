using System;
using R3;
using UnityEngine;

public class StunnedState : IState
{
    private float _stunnedTime;
    public StunnedState(PlayerMovement player, float stunnedTime) : base(player)
    {
        _player = player;
        _stunnedTime = stunnedTime;
    }

    public override void Enter()
    {
        Observable.Timer(TimeSpan.FromSeconds(_stunnedTime)).Subscribe(_ =>
        {
            _player.ChangeState(new IdleState(_player));
        }).AddTo(_disposables);
    }

    public override void OnHitbyEnemy(float damage)
    {
        _player.TakeDamageHandler(damage);
    }
}
