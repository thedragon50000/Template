using UnityEngine;
using UnityEditor;

// 告訴 Unity：這個繪製器是專門給 [ReadOnly] 用的
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 💡 核心魔法：把 GUI 的互動功能暫時關閉
        GUI.enabled = false;

        // 依照原本的方式繪製該欄位
        EditorGUI.PropertyField(position, property, label);

        // 記得把它開回來，不然之後的所有 Inspector 欄位都會變灰色
        GUI.enabled = true;
    }

}
