using UnityEngine;

// 這只是個標記，裡面不需要寫任何邏輯

public class ReadOnlyAttribute : PropertyAttribute { }
// Note: 之後標籤可以寫 [ReadOnly] 就好，允許自動省略Attribute
public class PercentAttribute : PropertyAttribute { }