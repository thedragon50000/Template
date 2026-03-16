using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using DG.Tweening;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Animations;
using System.Linq;

public class StateAnimator : MonoBehaviour

{
    private Animator _animator;
    // AnimatorStateInfo stateInfo;


    // 將動畫state的哈希值存儲為靜態常量
    private static readonly int IdleStateHash = Animator.StringToHash("Attack01");
    private static readonly int RunStateHash = Animator.StringToHash("Attack02");
    private static readonly int JumpStateHash = Animator.StringToHash("C");
    private static readonly int Idle = Animator.StringToHash("New State");

    private readonly Dictionary<int, string> _stateAnim = new();
    private readonly Dictionary<int, AnimData> _animLibrary = new();
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        var a = _animator.runtimeAnimatorController as AnimatorController;
        var temp = a.layers[0].stateMachine.states;
        AnimatorState[] states = temp.Select(s => s.state).ToArray();

        foreach (var item in states)
        {
            Debug.Log($"{item.name}");
            Debug.Log($"{item.motion}");
            Debug.Log($"{item.nameHash}");
        }


        var clips = _animator.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
        {
            var data = new AnimData(clip.name, clip.length);
        }
        _stateAnim.Add(IdleStateHash, "A");
        _stateAnim.Add(RunStateHash, "B");
        _stateAnim.Add(JumpStateHash, "C");
        _stateAnim.Add(Idle, "New State");
    }

    void Start()
    {

        Observable.EveryUpdate()
            .Select(_ => _animator.GetCurrentAnimatorStateInfo(0))
            // 只有當播放進度跨越 1.0，且目前不是處於過渡狀態時
            .Select(info => info.normalizedTime >= 1.0f && !_animator.IsInTransition(0))

            // 狀態改變（從 false 變 true）才發送信號
            .DistinctUntilChanged()
            .Where(isFinished => isFinished)

            .Subscribe(_ =>
            {
                Debug.Log("動畫播放完畢！");
                // 這裡可以接續播 Idle
                _animator.CrossFade(Idle, 0.2f);
            });

        Observable.EveryUpdate(destroyCancellationToken)
             .Subscribe(_ =>
             {
                 if (Input.GetKeyDown(KeyCode.DownArrow)) PlayAnimation(IdleStateHash);
                 if (Input.GetKeyDown(KeyCode.RightArrow)) PlayAnimation(RunStateHash);
                 if (Input.GetKeyDown(KeyCode.LeftArrow)) PlayAnimation(JumpStateHash);
             });
    }

    void PlayAnimation(int hash)
    {
        _animator.CrossFade(hash, 0.1f);
        OnStateExit(hash);
    }

    // callBack 當動畫狀態退出時調用
    private void OnStateExit(int hash)
    {
        var info = _animator.GetCurrentAnimatorStateInfo(0);
        _stateAnim.TryGetValue(hash, out var state);
        var motion = state.Length; // 拿到裡面裝的 Clip
        var countdown = info.length;
        DOVirtual.DelayedCall(countdown, () => OnAnimationComplete(state));
    }


    // 自定義回調處理邏輯
    private void OnAnimationComplete(string animationName)
    {
        // 在這裡執行動畫結束後的任務
        Debug.Log($"Animation {animationName} completed!");
        switch (animationName)
        {
            default:
                break;
        }

        // _animator.CrossFade(Idle, 0.2f);
    }

    public class AnimData
    {
        public string Name;
        public int Hash;
        public float Length;

        public AnimData(string name, float length)
        {
            print($"name: {name}");
            Name = name;
            Hash = Animator.StringToHash(name);
            Length = length;
        }
    }
}