using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Use physics raycast hit from mouse click to set agent destination
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ClickToMove : MonoBehaviour
    {
        NavMeshAgent m_Agent;
        RaycastHit m_HitInfo;

        void Awake()
        {
            m_Agent = GetComponent<NavMeshAgent>();
        }
        //
        // void Update()
        // {
        //     if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        //     {
        //         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //         if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
        //             m_Agent.destination = m_HitInfo.point;
        //     }
        // }

        public void OnMove(InputAction.CallbackContext ctx) // 接收 Input System 訊號
        {
            if (ctx.performed)
            {
                // 取得當前滑鼠物件
                var mouse = Mouse.current;
                if (mouse == null) return;

                // 讀取滑鼠座標
                Vector2 mousePosition = mouse.position.ReadValue();
// 轉換為射線
                Ray ray = Camera.main!.ScreenPointToRay(mousePosition);

                if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
                    m_Agent.destination = m_HitInfo.point;
            }
        }
    }
}