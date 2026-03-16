using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PercentAttribute))]
public class PercentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 先檢查：被我包住的東西到底是不是數字？
        if (property.propertyType == SerializedPropertyType.Float)
        {
            property.floatValue = EditorGUI.Slider(position, label, property.floatValue, 0, 100);
        }
        else if (property.propertyType == SerializedPropertyType.Integer)
        {
            // 轉成 int 處理
            property.intValue = EditorGUI.IntSlider(position, label, property.intValue, 0, 100);
        }
        else
        {
            // 如果不是數字（例如是個字串），就畫一個紅色的警告標記，提醒自己用錯了
            EditorGUI.HelpBox(position, $"錯誤：[Percent] 不能用在 {property.type} 類型上！", MessageType.Error);
        }
    }
}