using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Andy.Zenject_LoadScene.Scripts
{
    public class SendHp2NextScene : MonoBehaviour
    {
        [FormerlySerializedAs("InputField")] public TMP_InputField inputField;
        public Button btn;
        public PlayerStats tempStat;
        [Inject] public ZenjectSceneLoader SceneLoader;

        void Start()
        {
            btn.onClick.AddListener(delegate
            {
                float.TryParse(inputField.text, out var i);
                // Note: [Serializable] 過了所以已經實例化了 
                tempStat.HpValue = i;
                // Note: container.BindInstance(tempStat) 的作用是讀取下一個場景的初始化階段時完成綁定
                //  也就是說我們要 在A場景 決定 B場景的綁定，在B場景的Installer就綁定反而會導致重複綁定
                SceneLoader.LoadScene(2, LoadSceneMode.Single, container => container.BindInstance(tempStat));

                //  Warning: 之前會成功只是因為 FromInstance 優先級太高會蓋掉其他tempStat的綁定，誤打誤撞的
                // SceneLoader.LoadScene(2, LoadSceneMode.Single,
                //     container => container.Bind<PlayerStats>().FromInstance(tempStat));
            });
        }
    }
}