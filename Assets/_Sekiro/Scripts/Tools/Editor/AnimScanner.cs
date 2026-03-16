#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using UnityEngine;

public class AnimScanner
{
    [MenuItem("我的工具/同步動畫資料庫")]
    public static void Sync()
    {
        var animator = Selection.activeGameObject?.GetComponent<Animator>();
        var controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null) return;

        var dataMap = AssetDatabase.LoadAssetAtPath<AnimDataMap>("Assets/_Sekiro/Resources/AnimationDataTemp/AnimDataMap.asset");
        // 如果沒檔案就建一個
        if (dataMap == null)
        {
            dataMap = ScriptableObject.CreateInstance<AnimDataMap>();
            AssetDatabase.CreateAsset(dataMap, "Assets/_Sekiro/Resources/AnimationDataTemp/AnimDataMap.asset");
        }
        dataMap.states.Clear();

        var states = controller.layers[0].stateMachine.states;
        foreach (var s in states)
        {
            var motion = s.state.motion as AnimationClip;
            if (motion == null) continue;

            // 檢查是否已經存在，有的話只更新長度，不覆蓋事件
            // var existing = dataMap.states.Find(x => x.stateName == s.state.name);
            // if (existing != null)
            // {
            //     existing.length = motion.length;
            //     existing.hash = Animator.StringToHash(s.state.name);
            // }
            // else
            {
                dataMap.states.Add(new AnimDataMap.StateData
                {
                    stateName = s.state.name,
                    hash = Animator.StringToHash(s.state.name),
                    length = motion.length,
                    // clip = motion
                });
            }
        }

        EditorUtility.SetDirty(dataMap);
        AssetDatabase.SaveAssets();
        Debug.Log("已更新並儲存動畫狀態資料");
    }
}
#endif