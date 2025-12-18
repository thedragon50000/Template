using UnityEngine;
using Zenject;

namespace Andy.Zenject_LoadScene.Scripts
{
    public class Tests : IInitializable
    {
        public void Initialize()
        {
            Debug.Log("Tests Created");
        }

        public void Sonzai()
        {
            Debug.Log("test 存在");
        }
    }
}