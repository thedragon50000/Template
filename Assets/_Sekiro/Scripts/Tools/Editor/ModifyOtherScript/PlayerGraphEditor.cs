using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;

[CustomEditor(typeof(PlayerGraph))]
public class PlayerGraphEditor : Editor
{
    private float previewTime = 0f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerGraph script = (PlayerGraph)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("🎬 非運行模式預覽", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        previewTime = EditorGUILayout.Slider("動畫進度", previewTime, 0f, script.clip1.length);

        if (EditorGUI.EndChangeCheck())
        {
            script.ManualUpdate(previewTime);
            // 💡 提示：在 Editor 模式手動改模型位置，需要強制重繪 Scene 視窗才看得到變化
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("關閉預覽並銷毀 Graph"))
        {
            previewTime = 0;
            script.ManualUpdate(previewTime);
            SceneView.RepaintAll();

            // 記得清理，不然 Editor 會殘留 Graph
            script.OnDestroy();
        }
    }
    // // 讓 Editor 每幀刷新，這樣進度條才會動
    // public override bool RequiresConstantRepaint() => true;
}