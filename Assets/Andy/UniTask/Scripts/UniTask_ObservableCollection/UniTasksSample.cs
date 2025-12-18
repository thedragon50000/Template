// ObservableCollections必要的命名空間

using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
//
//
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UniTasksSample : MonoBehaviour
{
    private ObservableCollection<GameObject> _gameObjects;
    public Button btn;
    public float f = 0;

    // ReSharper disable once Unity.IncorrectMethodSignature
    private async UniTaskVoid Awake()
    {
        btn.onClick.AddListener(async () =>
        {
            TempVoid().Forget();
            await TempVoid();
            print("C");
        });
        
    }

    // Note: UniTaskVoid 只管觸發不能await，適用於按鈕
    private async UniTask TempVoid()
    {
        print("A");
        await UniTask.Delay(500);
        print("B");
        await UniTask.DelayFrame(100);
    }
    
    async UniTaskVoid Start()
    {
        _gameObjects = new ObservableCollection<GameObject>();

        // 订阅 CollectionChanged 事件
        // Note: CySharp 改良過的 ObservableCollection可以結合 async 使用
        _gameObjects.CollectionChanged += async (sender, e) => { await OnChanged(e); };

        // Test();
        // Test2();
        await UniTask.Delay(100);
        Test3();
    }

    private void Test()
    {
        // 添加一些初始数据
        for (int i = 0; i < 3; i++)
        {
            var o = new GameObject
            {
                name = $"第{i}_Object"
            };
            _gameObjects.Add(o);
        }

        _gameObjects.RemoveAt(0);


        _gameObjects.Move(0, 1);
        int index = _gameObjects[0].transform.GetSiblingIndex();
        int index1 = _gameObjects[1].transform.GetSiblingIndex();
        print($"index: {index},{index1}");
        _gameObjects[0].transform.SetSiblingIndex(index1);
        _gameObjects[1].transform.SetSiblingIndex(index);

        _gameObjects.Clear();
    }

    private async void Test2()
    {
        // f = await GetScoreAsync();
        // f += await GetScoreAsync();
        // f += await GetScoreAsync();
        // f += await GetScoreAsync();
        print("數學算完了");
    }

    private async void Test3()
    {
        //Note: 呼叫並等待所有任務完成
        await UniTask.WhenAll(Task1(), Task2(), Math());
        Debug.Log("所有任务完成。");
    }

    private async UniTask Task1()
    {
        await UniTask.Delay(1000);
        Debug.Log("任务 1 完成。");
    }

    private async UniTask Task2()
    {
        await UniTask.Delay(1500);
        Debug.Log("任务 2 完成。");
    }

    private async UniTask OnChanged(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (GameObject newItem in e.NewItems)
                {
                    await UniTask.Delay(1000);
                    Debug.Log("等了一秒再印，Add會出現在Remove那些之後");
                    Debug.Log("Added: " + newItem);
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                await HandleRemoveAsync(e);
                break;

            case NotifyCollectionChangedAction.Move:
                foreach (GameObject newItem in e.NewItems)
                {
                    Debug.Log("Move: " + newItem);
                }

                foreach (GameObject oldItem in e.OldItems)
                {
                    Debug.Log("Move: " + oldItem);
                }

                break;
            case NotifyCollectionChangedAction.Replace:

                foreach (GameObject newItem in e.NewItems)
                {
                    Debug.Log("Replace: " + newItem);
                }

                foreach (GameObject oldItem in e.OldItems)
                {
                    Debug.Log("Replace: " + oldItem);
                }

                break;
            case NotifyCollectionChangedAction.Reset:
                Debug.Log("Reset");

                break;
        }
    }

    private async UniTask HandleRemoveAsync(NotifyCollectionChangedEventArgs e)
    {
        foreach (GameObject oldItem in e.OldItems)
        {
            await UniTask.Delay(100);
            Debug.Log("0.1");
            await UniTask.Delay(100);
            Debug.Log("0.2");
            await UniTask.Delay(100);
            Debug.Log("0.3");
            await UniTask.Delay(100);
            Debug.Log("0.4");
            await UniTask.Delay(100);
            Debug.Log("0.5");
            await UniTask.Delay(100);
            Debug.Log("0.6");
            Debug.Log("Removed: " + oldItem);
        }
    }

    private async UniTask<int> GetScoreAsync()
    {
        await UniTask.Delay(1000); // 模拟延迟
        print("math");
        return 100; // 返回分数
    }

    private async UniTask Math()
    {
        f += await GetScoreAsync();
        f += await GetScoreAsync();
        f += await GetScoreAsync();
        f += await GetScoreAsync();
    }
}