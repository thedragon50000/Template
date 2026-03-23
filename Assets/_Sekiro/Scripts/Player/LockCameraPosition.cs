using UnityEngine;
using R3;
using DG.Tweening;
public class LockCameraPosition : MonoBehaviour
{
    public Transform player;
    public Transform target;

    public bool isInvert;

    void Awake()
    {

    }
    void Start()
    {
        Observable.EveryUpdate(UnityFrameProvider.PreLateUpdate, destroyCancellationToken).Subscribe(_ =>
        {
            transform.position = GetRotatedCPoint(target.position, player.position);
            transform.LookAt(target.position);
        });
    }
    public Vector3 GetRotatedCPoint(Vector3 A, Vector3 B)
    {
        // 1. 取得向量 AB
        Vector3 AB = B - A;

        // 2. 旋轉 AB 向量 15 度
        float y = isInvert ? -15 : 3;
        Vector3 rotatedVector = Quaternion.Euler(0, y, 0) * AB;

        // 3. 縮放長度為原來的 cos(15°) 倍
        float scale = Mathf.Cos(15f * Mathf.Deg2Rad) * 1.5f;
        // Vector3 C = A + (rotatedVector * scale) + Vector3.up * 2;
        float distance = Vector3.Distance(A, B);
        float positionY = Mathf.Lerp(0, 2, distance / 2) + 1;
        Vector3 C = A + rotatedVector * 2 + Vector3.up * positionY;

        return C;
    }
}
