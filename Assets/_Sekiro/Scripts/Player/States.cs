using UnityEngine;
using R3;
using System;

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class JumpState : IState
{
    private PlayerMovement _player;

    CharacterController _controller;


    private Vector3 _velocity; // 這裡存的是垂直速度
    private float _gravity = -9.81f; // 自定義重力

    // 用來管理所有訂閱，確保隨時能取消
    private CompositeDisposable _disposables = new CompositeDisposable();

    public JumpState(PlayerMovement player, Vector3 force)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
        _velocity = force;
    }

    public void Enter()
    { /* 設定跳躍初始速度 */
        Observable.EveryUpdate(UnityFrameProvider.PreLateUpdate, _player.destroyCancellationToken)
        .Select(_ => _velocity.magnitude).DistinctUntilChanged()
        .Subscribe(_ =>
        {
            JumpUpdate();
        }).AddTo(_disposables);

        // 狀態偵測流：監測著地
        Observable.EveryUpdate()
            .Where(_ => _controller.isGrounded && _velocity.y < 0)
            // .Skip(1)
            .Take(1) // 只取第一次滿足條件就結束流
            .Subscribe(_ => _player.ChangeState(new IdleState(_player)))
            .AddTo(_disposables);

    }

    public void Update()
    {
    }

    private void JumpUpdate()
    {
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 給一個小小的負值，確保它緊貼地面
            return;
        }

        // 加上重力加速度
        _velocity.y += _gravity * Time.deltaTime;

        // 執行移動（記得 Time.deltaTime 要用兩次，這是物理運動學的公式）
        _controller.Move(_velocity * Time.deltaTime);

    }
    public void Exit()
    {
        // 最重要的一步：離開狀態時，強制切斷所有流，防止舊狀態還在干擾
        _disposables.Clear();
    }
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
}