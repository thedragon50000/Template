using UnityEngine;
using UnityEditor;

/*
在第一階段，我們是用 importer.textureType = ... 這種方式修改屬性。這叫「直接存取」。
但如果有些屬性在 API 裡沒開放（隱藏屬性）
或者想寫一個能同時處理數百個物件且支援 Undo 的專業工具
就必須使用 SerializedObject。
*/

/*
// Note: 什麼是 SerializedObject？
Unity 的所有物件（GameObject, Component, Asset）本質上都是一堆數據。

SerializedObject：就是把這些數據「抽出來」放在一個包裝盒裡。

SerializedProperty：就是這個盒子裡的具體某個數據（例如圖片的寬度、一個數值、一個字串）。

為什麼要這麼麻煩？

自動支援 Undo：透過序列化修改，Unity 會自動記錄舊值。

多選編輯：你可以把一堆物件塞進同一個 SerializedObject，改一次就全部改好。

安全：它會確保你的修改符合 Unity 的數據結構，不會把檔案改壞。
*/
public class SerializedObjectTool
{
    [MenuItem("我的工具/Advanced Texture Setup can Undo")]
    static void SetupTexturesCanUndo()
    {
        Object[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);

        // 💡 步驟 A: 開啟一個 Undo 群組，讓這次所有修改變成「一次 Ctrl+Z」
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Batch Change Mesh Type");
        int undoGroup = Undo.GetCurrentGroup();

        foreach (var tex in textures)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                // 💡 步驟 B: 紀錄該物件，並指定屬於剛才那個群組
                Undo.RecordObject(importer, "Change Sprite Mesh Type");

                SerializedObject so = new SerializedObject(importer);
                SerializedProperty meshTypeProp = so.FindProperty("m_SpriteMeshType");

                if (meshTypeProp != null)
                {
                    meshTypeProp.intValue = 0;
                    so.ApplyModifiedProperties();
                }

                // 💡 提示：SaveAndReimport 是硬碟操作，無法被 Undo 撤回檔案層級。
                // 但因為 .meta 被 Undo 改回去了，Unity 會偵測到變化並自動再次觸發 Reimport。
                importer.SaveAndReimport();
            }
        }

        // 💡 步驟 C: 結束並合併群組
        Undo.CollapseUndoOperations(undoGroup);
    }
    
    [MenuItem("我的工具/Advanced Texture Setup")]
    static void SetupTextures()
    {
        // 1. 抓取資源 (使用DeepAssets抓取資料夾內部檔案)
        Object[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        Debug.Log($"length: {textures.Length}");

        // 2. 針對每個物件進行序列化修改
        foreach (var tex in textures)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            // Note: .meta 存放的是屬性、導入設定 (AssetImporter) 以及身分證字號 (GUID)。
            // Note: 其中繼承AssetImporter 的 TextureImporter ，負責圖片的導入
            // Note: Path 指向的是圖片，但 Unity 會「自動導航」到對應的 .meta。

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                // --- 序列化開始 ---
                // 建立包裝盒
                SerializedObject so = new SerializedObject(importer);

                // 尋找欄位（可以在 Unity 原始碼或 Debug 模式下看欄位名稱）
                // 這裡我們修改 "Sprite Mesh Type"
                SerializedProperty meshTypeProp = so.FindProperty("m_SpriteMeshType");

                if (meshTypeProp != null)
                {
                    // 修改值 (0 是 Full Rect, 1 是 Tight)
                    meshTypeProp.intValue = 0;
                    Debug.Log($"meshTypeProp's value is {meshTypeProp.intValue}");

                    // 寫回物件
                    so.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError("meshTypeProp is null.");
                }

                // 只有修改 Importer 屬性後才需要重新匯入
                importer.SaveAndReimport();
            }
            else
            {
                Debug.LogError("importer is null.");
            }

            Debug.Log("序列化修改完成！");
        }
    }

    [MenuItem("我的工具/Check Asset Properties")]
    static void Check()
    {
        var target = Selection.assetGUIDs;
        if (target.Length == 0)
        {
            return;
        }
        if (target.Length > 1)
        {
            Debug.LogWarning("只能選一個資產查詢");
            return;
        }
        string path = AssetDatabase.GUIDToAssetPath(target[0]);

        AssetImporter importer = AssetImporter.GetAtPath(path);
        SerializedObject so = new SerializedObject(importer);
        SerializedProperty it = so.GetIterator();

        // 「只要還有下一個房間，就繼續走」
        while (it.Next(true))
        {
            if (it.name.StartsWith("m_"))
            {
                // 現在 it 指向的就是當前的那個欄位
                Debug.Log("發現欄位名稱：" + it.name);
            }
        }
    }
}