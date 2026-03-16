using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Assertions;

public abstract class baseCharacterAnimation : MonoBehaviour
{
    public AnimDataMap dataMap;
    protected Animator _animator;

    /// <summary>
    /// 不同的動作比較好接技
    /// </summary>
    protected bool bPlayNextMoveLock = false;

    /// <summary>
    /// 重複一樣的動作
    /// </summary>
    protected bool bReapeatMoveLock = false;

    // Key: StateHash, Value: 該動畫所有的觸發點清單

    protected Dictionary<int, List<ActionPoint>> _allActions = new();

    protected List<Tween> _activeTweens = new List<Tween>();

    public class ActionPoint
    {
        public float TriggerTime; // 0~1
        public Action Callback;
        public bool IsTriggered;
    }
    void Awake()
    {
        _animator = GetComponent<Animator>();
        SetupAnimationActions();
    }

    protected virtual void SetupAnimationActions()
    {
        // // 可在這裡加上重置
        // var stateNames = dataMap.states.Select(x => x.stateName);
        // foreach (var state in stateNames)
        // {
        //     InsertAction(state, 1.0f, () => OnAnimEnd(state));
        // }

    }

    // 1. 播放時主動重置
    public void Play(string stateName)
    {
        int hash = Animator.StringToHash(stateName);
        _animator.CrossFade(hash, 0.1f);

        // 重置該動畫所有事件的觸發狀態，防止它記住上一回的結果
        if (_allActions.TryGetValue(hash, out var points))
        {
            foreach (var p in points) p.IsTriggered = false;
        }
    }

    public void InsertAction(string stateName, float time, Action callback)
    {
        int hash = Animator.StringToHash(stateName);
        if (!_allActions.ContainsKey(hash)) _allActions[hash] = new List<ActionPoint>();

        _allActions[hash].Add(new ActionPoint
        {
            TriggerTime = time,
            Callback = callback
        });
    }

    protected void OnAnimEnd(string name)
    {
        Debug.Log($"{name} 播完囉，執行接續邏輯");

        bReapeatMoveLock = false;
        bPlayNextMoveLock = false;
        // 例如：if (name == "A") Play("B");
    }


    public void PlayAnimation(string state)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        var info = _animator.GetCurrentAnimatorStateInfo(0);
        // 相同動作
        if (info.shortNameHash == Animator.StringToHash(state))
        {
            if (bReapeatMoveLock) return;
            Debug.Log("同動作重複");
            _animator.Play(state, 0, 0);
        }
        else
        {
            if (bPlayNextMoveLock) return;
            Debug.Log("不同動作接技");
            // 播動畫
            _animator.CrossFade(state, 0.1f);
        }
        InitializeActionTimers(state);

        void InitializeActionTimers(string state)
        {
            // 1. 殺掉該角色身上所有舊的計時器，避免回調疊加
            foreach (var t in _activeTweens) t.Kill();
            _activeTweens.Clear();

            bPlayNextMoveLock = true;
            bReapeatMoveLock = true;

            var currentAnim = dataMap.states.Find(x => x.stateName == state);
            if (currentAnim == null) return;

            if (_allActions.TryGetValue(currentAnim.hash, out var points))
            {
                foreach (var p in points)
                {
                    float timing = p.TriggerTime * currentAnim.length;
                    var t = DOVirtual.DelayedCall(timing, () => p.Callback()).SetLink(gameObject);
                    _activeTweens.Add(t);
                }
            }
            else
            {
                // 防呆：如果這動作沒設任何事件，也要在播完後自動解鎖，不然角色會永久卡死
                var t = DOVirtual.DelayedCall(currentAnim.length, () =>
                {
                    bPlayNextMoveLock = false;
                    bReapeatMoveLock = false;
                }).SetLink(gameObject);
                _activeTweens.Add(t);
            }
        }
    }

}