using UnityEngine;
using UnityEditor; // 必須引用此命名空間
using System.IO;

public class BatchRenameTool
{
    // 在選單列建立一個按鈕，路徑為 我的工具 > Rename Selected Assets
    [MenuItem("我的工具/Rename Selected Assets")]
    static void RenameAssets()
    {
        // 1. 獲取目前在 Project 視窗選中的所有資源的 GUID
        string[] selectedGuids = Selection.assetGUIDs;

        if (selectedGuids.Length == 0)
        {
            Debug.LogWarning("請先在 Project 視窗選取資源！");
            return;
        }

        int counter = 1;

        // 開始批量處理
        foreach (string guid in selectedGuids)
        {
            // 2. 將 GUID 轉換為檔案路徑
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 3. 取得該檔案的原始名稱
            string oldName = Path.GetFileNameWithoutExtension(assetPath);
            string newName = $"Asset_{counter:D3}"; // 格式化為 Asset_001, Asset_002...

            // 4. 執行重新命名
            // 注意：AssetDatabase.RenameAsset 需要路徑，且不包含副檔名
            string error = AssetDatabase.RenameAsset(assetPath, newName);

            if (string.IsNullOrEmpty(error))
            {
                Debug.Log($"已將 {oldName} 重新命名為 {newName}");
            }
            else
            {
                Debug.LogError($"重新命名失敗: {error}");
            }

            counter++;
        }

        // 5. 強制存檔並重新整理資料庫，讓修改立即顯示在 Editor
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("我的工具/Rename Selected Assets 2")]

    static void Test()
    {
        var selectAssets = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        if (selectAssets.Length == 0)
        {
            Debug.LogWarning("請先在 Project 視窗選取資源！");
            return;
        }

        // 1. 抓取 (記得加 Assets | Deep)
        int counter = 1;

        // 開始批量處理
        foreach (var guid in selectAssets)
        {
            // 2. 將 GUID 轉換為檔案路徑
            string assetPath = AssetDatabase.GetAssetPath(guid);

            // 3. 取得該檔案的原始名稱

            string oldName = Path.GetFileNameWithoutExtension(assetPath);
            string newName = $"Asset_{counter:D3}"; // 格式化為 Asset_001, Asset_002...

            // 4. 執行重新命名
            // 注意：AssetDatabase.RenameAsset 需要路徑，且不包含副檔名
            string error = AssetDatabase.RenameAsset(assetPath, newName);

            if (string.IsNullOrEmpty(error))
            {
                Debug.Log($"已將 {oldName} 重新命名為 {newName}");
            }
            else
            {
                Debug.LogError($"重新命名失敗: {error}");
            }

            counter++;
        }

        // 5. 強制存檔並重新整理資料庫，讓修改立即顯示在 Editor
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}