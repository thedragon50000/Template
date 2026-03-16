using UnityEngine;
// using UnityEditor; // 如果你上面沒 using，這行可以不要

[System.Serializable]
public class BodyPartData
{
    public string partName = "部位";
    public Sprite partSprite;
    public RuntimeAnimatorController partAnimator;
    public Vector2 initialLocalPosition;
    public int id; // 💡 新增：用來在 SceneView 辨識哪個部位被拖動

    // 💡 方便初始化 ID
    public BodyPartData(int _id, string _name, Vector2 _pos)
    {
        id = _id;
        partName = _name;
        initialLocalPosition = _pos;
    }
}