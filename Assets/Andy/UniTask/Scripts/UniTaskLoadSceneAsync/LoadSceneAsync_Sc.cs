using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadSceneAsync_Sc : MonoBehaviour
{
    public string sceneName = "UniTask_ObservableCollection";

    public Button confirmButton;
    private AsyncOperation _sceneLoadingOperation;

    private void Awake()
    {
        confirmButton.onClick.AddListener(ActivateLoadedScene);
        confirmButton.interactable = false;
    }

    private void Start()
    {
        // 异步加载场景，但暂时不激活它
        WaitLoadScene().Forget(); // 使用 Forget 避免警告
    }

    private async UniTask WaitLoadScene()
    {
        await UniTask.WhenAll(LoadSceneAsync(sceneName));
        Debug.Log("開啟按鈕");
        confirmButton.interactable = true;
    }

    private async UniTask LoadSceneAsync(string sceneName)
    {
        Debug.Log("开始加载场景...");

        // 开始异步加载场景，并禁止自动激活
        _sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
        if (_sceneLoadingOperation != null)
        {
            _sceneLoadingOperation.allowSceneActivation = false;

            // 等待场景加载到 90% 完成（progress 达到 0.9）
            while (_sceneLoadingOperation.progress < 0.9f)
            {
                Debug.Log($"加载进度：{_sceneLoadingOperation.progress * 100}%");
                await UniTask.Yield(); // 等待下一帧
            }
        }

        Debug.Log("场景加载完成，但尚未激活。");
        // 此时场景已经加载完成，但还不会自动切换
    }

    // 手动切换场景
    private void ActivateLoadedScene()
    {
        if (_sceneLoadingOperation != null)
        {
            Debug.Log("手动激活场景...");
            _sceneLoadingOperation.allowSceneActivation = true; // 激活场景
        }
    }
}