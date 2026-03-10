using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform cameraTransform; // 拖入 Main Camera
    private Vector2 moveInput;

    public void OnMove(InputAction.CallbackContext ctx) // 接收 Input System 訊號
    {
        moveInput = ctx.ReadValue<Vector2>();
        Debug.Log($"axis: {moveInput}");
    }

    void Update()
    {
        if (moveInput == Vector2.zero) return;

        // 計算相對於攝影機的前後左右
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; // 忽略垂直方向
        right.y = 0;

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // 移動角色
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 角色轉向移動方向
        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.15f);
        }
    }
}