using UnityEngine;
using R3;
using System;

public class ParryState : IState
{
    private PlayerMovement _player;

    CharacterController _controller;


    // 用來管理所有訂閱，確保隨時能取消
    private CompositeDisposable _disposables = new CompositeDisposable();
    private bool perfectParry;

    public ParryState(PlayerMovement player)
    {
        _player = player;
        _controller = player.GetComponent<CharacterController>();
    }

    public void Enter()
    {
        perfectParry = true;
        Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
        {
            // 完美格檔時間結束
            perfectParry = false;
        });
        Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
        {
            // 完美格檔時間結束
            _player.ChangeState(new IdleState(_player));
        });
    }

    public void OnHitbyEnemy(float damage)
    {
        if (perfectParry)
        {
            // todo: 完美格檔處理
            Debug.Log("完美格檔，不拉");
        }
        else
        {
            Debug.Log("有點拉");
        }
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
        // 至少要能轉向
        _player.SetHorizontalMove(moveInput * 0.01f);
    }

    public void VerticalInput(Vector3 velocity)
    {
    }

    public void GuardInput()
    {
    }
}
