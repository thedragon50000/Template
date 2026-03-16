using UnityEngine;
using System;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    public Transform basePoint; // 劍柄
    public Transform tipPoint;  // 劍尖
    public float radius = 0.2f;
    public LayerMask enemyLayer;

    private List<Collider> _hitBuffer = new List<Collider>();

    public void CheckHit(Action<Collider> onHit)
    {
        // 核心：掃描兩點之間的區域
        Collider[] hits = Physics.OverlapCapsule(
            basePoint.position,
            tipPoint.position,
            radius,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            // 這裡可以加上額外的過濾，例如：不能砍到自己
            if (hit.transform.root == transform.root) continue;

            onHit?.Invoke(hit);
        }
    }
}