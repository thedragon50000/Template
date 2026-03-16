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

    protected List<Tween> _activeTweens = new List<Tween>();

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void AttackStateHandler()
    {
    }

    public void PlayAnimationFromState(string targetStateName)
    {
        AnimDataMap.StateData targetState = dataMap.states.Find(x => x.stateName == targetStateName);
        if (targetState == null)
        {
            Debug.Log($"{gameObject.name}: 無此動畫state");
            return;
        }

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        // 相同動作
        if (info.shortNameHash == Animator.StringToHash(targetStateName))
        {
            if (bReapeatMoveLock) return;
            Debug.Log("同動作重複");
            _animator.Play(targetState.hash, 0, 0);
        }
        else
        {
            // todo: 銜接、過渡 的效果，先留著
            if (bPlayNextMoveLock) return;
            Debug.Log("不同動作接技");
            // 播動畫
            _animator.CrossFade(targetState.hash, 0.1f);
        }
    }
}