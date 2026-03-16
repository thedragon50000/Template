using UnityEngine;
using R3;

[ExecuteInEditMode]
public class animPlayer : baseCharacterAnimation
{
    [Header("測試快捷鍵")]
    [Tooltip("開啟後可以使用方向鍵測試動畫")]
    [SerializeField] private bool enableDebugKeys = true;

    [Header("目前的狀態")]
    // [ReadOnly] // todo: 這是自定義標籤，等一下我們來寫它
    [SerializeField] private string currentPlayingAnim = "None";

    [Header("預覽設置")]
    [Range(0, 1)]
    public float previewNormalizedTime;

    void Start()
    {
        Observable.EveryUpdate(destroyCancellationToken)
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    PlayAnimation("A");
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    PlayAnimation("B");
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PlayAnimation("C");
                }
            });
    }


    protected override void SetupAnimationActions()
    {
        // --- 動畫 A 的多點控制 ---
        InsertAction("A", 0.3f, () => Debug.Log("A 播到 30%"));
        InsertAction("A", 0.6f, () => Debug.Log("A 播到 60%"));
        InsertAction("A", 0.4f, () => { bPlayNextMoveLock = false; });
        InsertAction("A", 1.0f, () => OnAnimEnd("A")); // 1.0 就是結束點

        // --- 動畫 B 的控制 ---
        InsertAction("B", 0.5f, () => Debug.Log("B 的中場休息"));
        InsertAction("B", 0.4f, () => { bPlayNextMoveLock = false; });
        InsertAction("B", 1.0f, () => OnAnimEnd("B"));

        InsertAction("C", 0.3f, () => { bPlayNextMoveLock = false; });
        InsertAction("C", 1.0f, () => OnAnimEnd("C"));

    }
}