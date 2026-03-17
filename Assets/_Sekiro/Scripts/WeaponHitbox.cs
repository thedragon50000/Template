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
    public LineRenderer line;

    public void CheckHit(Action<Collider> onHit)
    {
        // 核心：掃描兩點之間的區域
        Collider[] hits = Physics.OverlapCapsule(
            basePoint.position,
            tipPoint.position,
            radius,
            enemyLayer
        );

        // line.SetPosition(0, basePoint.position);
        // line.SetPosition(1, tipPoint.position);

        foreach (var hit in hits)
        {
            Debug.Log(hit.name);
            // 這裡可以加上額外的過濾，例如：不能砍到自己
            if (hit.transform.root == transform.root) continue;

            onHit?.Invoke(hit);
        }
    }

    private void OnDrawGizmos()
    {
        if (basePoint == null || tipPoint == null) return;

        Gizmos.color = new Color(1, 0, 0, 0.4f); // 設為半透明紅，這樣重疊的地方才不會太刺眼

        int segmentCount = 5; // 你想要中間生出幾顆球
        for (int i = 0; i <= segmentCount; i++)
        {
            // 計算百分比 (0.0 到 1.0)
            float t = (float)i / segmentCount;
            // 找出這顆球應該在的位置
            Vector3 pos = Vector3.Lerp(basePoint.position, tipPoint.position, t);

            // 畫出實心球，半徑就用你 OverlapCapsule 的 radius
            Gizmos.DrawSphere(pos, radius);
        }

        // 額外畫一條線貫穿中心，增加指向感
        Gizmos.color = Color.red;
        Gizmos.DrawLine(basePoint.position, tipPoint.position);
    }
}