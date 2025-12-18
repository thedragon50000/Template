// extension awaiter/methods can be 使用d by this namespace

using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Andy.UniTask.Samples
{
    public class UniTaskSample1 : MonoBehaviour
    {
        bool isActive = true;

        /// <summary>
        /// 一個 Coroutine
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private IEnumerator FooCoroutineEnumerator()
        {
            throw new NotImplementedException();
        }

        // Unity 專用的輕量級替代方案，不返回 Task<T>，而是返回 UniTask<T>(或 UniTask)
        // zero allocation and fast excution for zero overhead async/await integrate with Unity
        // 零分配與快速執行，實現零開銷的 async/await，並與 Unity 深度整合
        async UniTask<string> DemoAsync()
        {
            // 你可以 await Unity 的 AsyncObject
            var asset = await Resources.LoadAsync<TextAsset>("foo");
            var txt = (await UnityWebRequest.Get("https://...").SendWebRequest()).downloadHandler.text;
            await SceneManager.LoadSceneAsync("scene2");

            // .WithCancellation 讓你可以取消, GetCancellationTokenOnDestroy()可以得到 CancellationToken
            // CancellationToken 在 GameObject 生命週期結束時會觸發
            var asset2 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(this.GetCancellationTokenOnDestroy());
            // Unity 2022.2 之後的版本, 繼承 MonoBehaviour的腳本可以使用 `destroyCancellationToken`
            var asset22 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(destroyCancellationToken);

            // .ToUniTask accepts progress callback(and all options),
            // .ToUniTask 的功能是將 Unity 的 AsyncOperation 包裝成 UniTask，並開放 傳入 Progress 參數（或其他具名引數）
            // Progress.Create<T> 是 UniTask 為了追求 Zero Allocation 和 簡潔性 所提供的靜態方法，它內部高效地創建了一個實現 IProgress<T> 功能的物件，而避免了您手動寫一個類別。
            var asset3 = await Resources.LoadAsync<TextAsset>("baz")
                .ToUniTask(Progress.Create<float>(x => Debug.Log(x)));
            // 傳入具名引數
            var asset33 = await Resources.LoadAsync<TextAsset>("baz")
                .ToUniTask(timing: PlayerLoopTiming.FixedUpdate);

            // await frame-based operation like a coroutine
            await Cysharp.Threading.Tasks.UniTask.DelayFrame(100);

            // replacement of yield return new WaitForSeconds/WaitForSecondsRealtime
            await Cysharp.Threading.Tasks.UniTask.Delay(TimeSpan.FromSeconds(10), ignoreTimeScale: false);

            // yield any playerloop timing(PreUpdate, Update, LateUpdate, etc...)
            await Cysharp.Threading.Tasks.UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

            // replacement of yield return null
            await Cysharp.Threading.Tasks.UniTask.Yield();
            await Cysharp.Threading.Tasks.UniTask.NextFrame();

            // replacement of WaitForEndOfFrame
#if UNITY_2023_1_OR_NEWER
            await Cysharp.Threading.Tasks.UniTask.WaitForEndOfFrame();
#else
    // requires MonoBehaviour(CoroutineRunner))
    await UniTask.WaitForEndOfFrame(this); // this is MonoBehaviour
#endif

            // replacement of yield return new WaitForFixedUpdate(same as UniTask.Yield(PlayerLoopTiming.FixedUpdate))
            await Cysharp.Threading.Tasks.UniTask.WaitForFixedUpdate();

            // replacement of yield return WaitUntil
            await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => !isActive);

            // special helper of WaitUntil
            await Cysharp.Threading.Tasks.UniTask.WaitUntilValueChanged(this.gameObject, x => x);

            // 你可以 await IEnumerator coroutines
            await FooCoroutineEnumerator();

            // 你可以 await a standard task
            await Task.Run(() => 100);

            // Multithreading, run on ThreadPool under this code
            await Cysharp.Threading.Tasks.UniTask.SwitchToThreadPool();

            /* work on ThreadPool */

            // return to MainThread(same as `ObserveOnMainThread` in UniRx)
            await Cysharp.Threading.Tasks.UniTask.SwitchToMainThread();

            // get async webrequest
            async UniTask<string> GetTextAsync(UnityWebRequest req)
            {
                var op = await req.SendWebRequest();
                return op.downloadHandler.text;
            }

            var task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
            var task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
            var task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));

            // concurrent async-wait and get results easily by tuple syntax
            var (google, bing, yahoo) = await Cysharp.Threading.Tasks.UniTask.WhenAll(task1, task2, task3);

            // shorthand of WhenAll, tuple can await directly
            var (google2, bing2, yahoo2) = await (task1, task2, task3);

            // return async-value.(or 你可以 使用 `UniTask`(no result), `UniTaskVoid`(fire and forget)).
            return (asset as TextAsset)?.text ?? throw new InvalidOperationException("Asset not found");
        }
    }
}