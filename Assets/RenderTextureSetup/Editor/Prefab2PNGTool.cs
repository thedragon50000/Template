using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SpriteDataExporter : EditorWindow
{
    public GameObject rootCharacter;
    public Vector2Int canvasSize = new Vector2Int(2048, 2048);
    public string folderName = "Numbered_Export";

    [MenuItem("Tools/立繪導出_自動編號版")]
    public static void ShowWindow() => GetWindow<SpriteDataExporter>("數據導出");

    private void OnGUI()
    {
        rootCharacter = (GameObject)EditorGUILayout.ObjectField("角色 Root", rootCharacter, typeof(GameObject), true);
        canvasSize = EditorGUILayout.Vector2IntField("畫布大小", canvasSize);
        folderName = EditorGUILayout.TextField("資料夾名稱", folderName);

        if (GUILayout.Button("執行導出 (自動編號 001, 002...)") && rootCharacter != null)
        {
            ExecuteNumberedExport();
        }
    }

    private void ExecuteNumberedExport()
    {
        // 1. 取得所有 SpriteRenderer (包含隱藏的)
        SpriteRenderer[] allSprites = rootCharacter.GetComponentsInChildren<SpriteRenderer>(true);
        
        string fullPath = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

        // 2. 計數器，用來生成編號
        int fileIndex = 1;

        foreach (var sr in allSprites)
        {
            if (sr.sprite == null) continue;

            // --- 自動處理 Atlas / Texture 讀寫權限 ---
            string assetPath = AssetDatabase.GetAssetPath(sr.sprite.texture);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            bool wasReadable = false;
            if (importer != null)
            {
                wasReadable = importer.isReadable;
                if (!wasReadable) {
                    importer.isReadable = true;
                    importer.SaveAndReimport();
                }
            }

            // 3. 建立畫布並初始化透明
            Texture2D finalTex = new Texture2D(canvasSize.x, canvasSize.y, TextureFormat.RGBA32, false);
            Color[] clearColors = new Color[canvasSize.x * canvasSize.y];
            finalTex.SetPixels(clearColors);

            // 4. 提取像素數據
            Sprite s = sr.sprite;
            Texture2D sourceTex = s.texture;
            Rect r = s.textureRect;
            Color[] pixels = sourceTex.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
            
            // 5. 計算對齊位置
            Vector2 relativePos = (Vector2)sr.transform.position - (Vector2)rootCharacter.transform.position;
            float ppu = s.pixelsPerUnit;
            int centerX = canvasSize.x / 2 + Mathf.RoundToInt(relativePos.x * ppu);
            int centerY = canvasSize.y / 2 + Mathf.RoundToInt(relativePos.y * ppu);
            int startX = centerX - Mathf.RoundToInt(s.pivot.x);
            int startY = centerY - Mathf.RoundToInt(s.pivot.y);

            // 6. 繪製並儲存
            if (startX >= 0 && startY >= 0 && startX + r.width <= canvasSize.x && startY + r.height <= canvasSize.y)
            {
                finalTex.SetPixels(startX, startY, (int)r.width, (int)r.height, pixels);
                finalTex.Apply();

                // --- 關鍵：格式化命名 (編號_物件名) ---
                // 使用 D3 確保編號是 001, 002 這種固定長度，排序才不會亂掉
                string fileName = $"{fileIndex:D3}_{sr.gameObject.name.Replace(" ", "_")}";
                File.WriteAllBytes(Path.Combine(fullPath, fileName + ".png"), finalTex.EncodeToPNG());
                
                fileIndex++; // 編號遞增
            }

            DestroyImmediate(finalTex);

            // 還原權限
            if (importer != null && !wasReadable) {
                importer.isReadable = false;
                importer.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"導出完成！共處理 {fileIndex - 1} 個部位，已加上編號。");
    }
}