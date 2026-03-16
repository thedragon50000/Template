using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ShanyangCreator : EditorWindow
{
    /// <summary>
    /// 即時座標 for UI滾輪
    /// </summary>
    private Vector2 scrollPos;

    /// <summary>
    /// 正在編輯positions
    /// </summary>
    private bool isEditingPositions = false;

    private GameObject previewParentGameObject = null;

    /// <summary>
    /// 身體各部位 各自的資料，分成幾部位就需要幾份資料
    /// </summary>
    public List<BodyPartData> bodyParts = new List<BodyPartData>();
    private string parentObjectName = "New Character";

    [MenuItem("我的工具/商鞅產生器")]
    public static void ShowWindow() => GetWindow<ShanyangCreator>("分屍模擬器");

    private void OnEnable()
    {
        // 🚨 必須在這裡訂閱，否則 Scene 視窗永遠看不到紅點
        SceneView.duringSceneGui += OnSceneGUI;

        if (bodyParts.Count == 0)
        {
            bodyParts.Add(new BodyPartData(0, "頭部", new Vector2(0, 1.5f)));
            bodyParts.Add(new BodyPartData(1, "軀幹", new Vector2(0, 0)));
            bodyParts.Add(new BodyPartData(2, "左臂", new Vector2(-1.5f, 0.5f)));
            bodyParts.Add(new BodyPartData(3, "右臂", new Vector2(1.5f, 0.5f)));
            bodyParts.Add(new BodyPartData(4, "下半身", new Vector2(0, -1.5f)));
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("商鞅五馬分屍生成器", EditorStyles.boldLabel);

        // todo: 讀到這裡而已，接著繼續看

        // 整體的合成預覽區域
        DrawCombinedPreview();

        EditorGUILayout.Space(10);

        // 編輯模式開關
        GUI.backgroundColor = isEditingPositions ? Color.red : Color.white;
        if (GUILayout.Button(isEditingPositions ? "結束編輯模式" : "進入 Scene 編輯模式 (出現紅點)", GUILayout.Height(30)))
        {
            isEditingPositions = !isEditingPositions;
            GeneratePreviewObjects();
            SceneView.RepaintAll();
        }
        GUI.backgroundColor = Color.white;

        parentObjectName = EditorGUILayout.TextField("父物件名稱", parentObjectName);

        EditorGUILayout.Space(10);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (GUILayout.Button("↑", GUILayout.Width(20)) && i > 0)
            {
                var temp = bodyParts[i];
                bodyParts[i] = bodyParts[i - 1];
                bodyParts[i - 1] = temp;
            }

            BodyPartData part = bodyParts[i];
            EditorGUILayout.BeginVertical("helpBox");

            using (new EditorGUILayout.HorizontalScope())
            {
                part.partName = EditorGUILayout.TextField(part.partName, GUILayout.Width(100));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", GUILayout.Width(20))) { bodyParts.RemoveAt(i); ClearPreview(); break; }
            }

            part.partSprite = (Sprite)EditorGUILayout.ObjectField("圖片", part.partSprite, typeof(Sprite), false);

            // 💡 修正 2：現在可以手動調整 X, Y 了
            part.initialLocalPosition = EditorGUILayout.Vector2Field("相對位置 (可手動改)", part.initialLocalPosition);

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ 增加部位")) { bodyParts.Add(new BodyPartData(bodyParts.Count, "新部位", Vector2.zero)); }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("生成到場景", GUILayout.Height(40))) { CreateShanyang(); }
    }

    // 💡 繪製合成預覽圖
    private void DrawCombinedPreview()
    {
        Rect rect = GUILayoutUtility.GetRect(200, 200);
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 1f)); // 背景稍微深一點

        if (bodyParts.Count == 0) return;

        // --- 1. 計算所有圖片組成的「最大邊界」 ---
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        bool hasAnySprite = false;
        foreach (var part in bodyParts)
        {
            if (part.partSprite == null) continue;
            hasAnySprite = true;

            // 考慮位置 + 圖片大小 (Sprite 的 size 是以 Unit 為單位，通常 1 Unit = 100 像素)
            float sw = part.partSprite.rect.width / 100f;
            float sh = part.partSprite.rect.height / 100f;

            minX = Mathf.Min(minX, part.initialLocalPosition.x - sw / 2);
            maxX = Mathf.Max(maxX, part.initialLocalPosition.x + sw / 2);
            minY = Mathf.Min(minY, part.initialLocalPosition.y - sh / 2);
            maxY = Mathf.Max(maxY, part.initialLocalPosition.y + sh / 2);
        }

        if (!hasAnySprite) return;

        // --- 2. 根據邊界計算縮放比 (Scale) ---
        float contentWidth = maxX - minX;
        float contentHeight = maxY - minY;

        // 加上一點邊距 (Margin)，不要貼死邊框
        contentWidth *= 1.2f;
        contentHeight *= 1.2f;

        // 算出適合視窗寬度與高度的比例，並取其小者（確保寬高都能裝進去）
        float scaleX = rect.width / contentWidth;
        float scaleY = rect.height / contentHeight;
        float autoScale = Mathf.Min(scaleX, scaleY);

        // 計算內容的中心偏移，讓整群圖居中
        Vector2 contentCenter = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);

        // --- 3. 開始繪製 ---
        foreach (var part in bodyParts)
        {
            if (part.partSprite == null) continue;

            // 計算相對於預覽框中心的位置
            // 公式：(部位位置 - 內容中心) * 自動縮放比
            Vector2 relativePos = (part.initialLocalPosition - contentCenter);
            Vector2 drawPos = rect.center + new Vector2(relativePos.x * autoScale, -relativePos.y * autoScale);

            float w = part.partSprite.rect.width * (autoScale / 100f);
            float h = part.partSprite.rect.height * (autoScale / 100f);

            Rect spriteRect = new Rect(drawPos.x - w / 2, drawPos.y - h / 2, w, h);

            // 繪製 Sprite (處理 Sprite Atlas 的情況)
            GUI.DrawTextureWithTexCoords(spriteRect, part.partSprite.texture,
                new Rect(part.partSprite.rect.x / part.partSprite.texture.width,
                         part.partSprite.rect.y / part.partSprite.texture.height,
                         part.partSprite.rect.width / part.partSprite.texture.width,
                         part.partSprite.rect.height / part.partSprite.texture.height));
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isEditingPositions) return;

        // 如果預覽物件不見了（例如切換場景），重新生成
        if (previewParentGameObject == null) GeneratePreviewObjects();

        foreach (var part in bodyParts)
        {
            // 💡 修正 3：確保 Handle 座標跟隨預覽父物件
            Vector3 worldPos = previewParentGameObject.transform.position + (Vector3)part.initialLocalPosition;
            float size = HandleUtility.GetHandleSize(worldPos) * 0.15f;

            Handles.color = Color.red;
            EditorGUI.BeginChangeCheck();

            // 畫出紅點並允許拖拽
            Vector3 newPos = Handles.FreeMoveHandle(worldPos, size, Vector3.zero, Handles.DotHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this, "Move Part");
                part.initialLocalPosition = (Vector2)(newPos - previewParentGameObject.transform.position);
                UpdatePreviewPositions(); // 即時移動圖片
                Repaint(); // 更新視窗內的數字
            }

            Handles.Label(worldPos + Vector3.up * size, part.partName);
        }
    }

    private void UpdatePreviewPositions()
    {
        if (previewParentGameObject == null) return;
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (i < previewParentGameObject.transform.childCount)
            {
                previewParentGameObject.transform.GetChild(i).localPosition = bodyParts[i].initialLocalPosition;
            }
        }
    }

    private void GeneratePreviewObjects()
    {
        ClearPreview();
        if (!isEditingPositions) return;

        previewParentGameObject = new GameObject("SHANYANG_PREVIEW");
        previewParentGameObject.hideFlags = HideFlags.HideAndDontSave;

        // 放在 Scene 攝影機看得到的地方
        if (SceneView.lastActiveSceneView != null)
            previewParentGameObject.transform.position = SceneView.lastActiveSceneView.pivot;

        foreach (var part in bodyParts)
        {
            GameObject go = new GameObject(part.partName);
            go.transform.SetParent(previewParentGameObject.transform);
            go.transform.localPosition = part.initialLocalPosition;
            go.hideFlags = HideFlags.HideAndDontSave;
            if (part.partSprite != null)
            {
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = part.partSprite;
                sr.color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    private void ClearPreview()
    {
        if (previewParentGameObject != null) DestroyImmediate(previewParentGameObject);
    }

    private void CreateShanyang()
    {
        GameObject root = new GameObject(parentObjectName);
        Undo.RegisterCreatedObjectUndo(root, "Create Shanyang");
        foreach (var part in bodyParts)
        {
            GameObject go = new GameObject(part.partName);
            go.transform.SetParent(root.transform);
            go.transform.localPosition = part.initialLocalPosition;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = part.partSprite;
            if (part.partAnimator != null) go.AddComponent<Animator>().runtimeAnimatorController = part.partAnimator;
        }
        Selection.activeObject = root;
        isEditingPositions = false;
        ClearPreview();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        ClearPreview();
    }
}
