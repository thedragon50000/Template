using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using R3.Triggers;

namespace Andy.Zenject_LoadScene1.Scripts
{
    public class PreManager : MonoBehaviour
    {
        public GameObject plane;

        [Inject] ZenjectSceneLoader sceneLoader;

        private void Start()
        {
            this.UpdateAsObservable().Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("毀了飛機！");

                    plane.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    bool b = plane.activeSelf;
                    LastSceneSettings settings = new LastSceneSettings(b);
                    sceneLoader.LoadScene("lastScene", LoadSceneMode.Single, container => container.BindInstance(settings));
                }
            });
        }
    }
}