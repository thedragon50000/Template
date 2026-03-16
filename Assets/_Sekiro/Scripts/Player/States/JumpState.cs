using UnityEngine;
using R3;

public class JumpState : IState
{
    private PlayerMovement _player;

    CharacterController _controller;


    private Vector3 _velocity; // 這裡存的是垂直速度
    // private float _gravity = -9.81f; // 自定義重力

    // 用來管理所有訂閱，確保隨時能取消
    private CompositeDisposable _disposables = new CompositeDisposable();

    public JumpState(PlayerMovement player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }

    public void Enter()
    {
        // 狀態偵測流：監測著地
        Observable.EveryUpdate()
            .Where(_ => _controller.isGrounded)
            // .Skip(1)
            .Take(1) // 只取第一次滿足條件就結束流
            .Subscribe(_ => _player.ChangeState(new IdleState(_player)))
            .AddTo(_disposables);
    }

    public void Update()
    {
    }
    public void Exit()
    {
        // 最重要的一步：離開狀態時，強制切斷所有流，防止舊狀態還在干擾
        _disposables.Clear();
    }

    public void HorizonInput(Vector2 moveInput)
    {
        // 只能微調
        _player.SetHorizontalMove(moveInput * 0.3f);
    }

    public void VerticalInput(Vector3 velocity)
    {
    }
}
