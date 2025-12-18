using Andy.Zenject_LoadScene1.Scripts;
using UnityEngine;
using Zenject;

public class LastManager : MonoBehaviour
{
    [Inject] private LastSceneSettings settings;

    public GameObject plane;

    void Start()
    {
        if (settings != null)
        {
            Debug.Log("讀取了上一個場景傳來的參數");
            plane.SetActive(settings.PlaneAlive);
        }
    }
}