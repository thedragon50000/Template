#if !(UNITY_METRO || UNITY_WP8)

#if UNITY_2018_3_OR_NEWER
#pragma warning disable CS0618
#endif

using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;

namespace UniRx.Examples
{
    /*Note: R3沒有這東西
        在 R3 中，處理 Unity Web 請求 (WWW/UnityWebRequest) 和其他複雜的非同步操作，最主流且官方推薦的做法是使用 UniTask 函式庫，並將其結果轉換為 IObservable。*/
    // sample script, attach your object.
    // public class Sample01_ObservableWWW : MonoBehaviour
    // {
    //     void Start()
    //     {
    //         // Basic: Download from google.
    //         {
    //             ObservableWWW.Get("http://google.co.jp/")
    //                 .Subscribe(
    //                     x => Debug.Log(x.Substring(0, 100)), // onSuccess
    //                     ex => Debug.LogException(ex)); // onError
    //         }
    //
    //         // Linear Pattern with LINQ Query Expressions
    //         // download after google, start bing download
    //         {
    //             var query = from google in ObservableWWW.Get("http://google.com/")
    //                         from bing in ObservableWWW.Get("http://bing.com/")
    //                         select new { google, bing };
    //
    //             var cancel = query.Subscribe(x => Debug.Log(x.google.Substring(0, 100) + ":" + x.bing.Substring(0, 100)));
    //
    //             // Call Dispose is cancel downloading.
    //             cancel.Dispose();
    //         }
    //
    //         // Observable.WhenAll is for parallel asynchronous operation
    //         // (It's like Observable.Zip but specialized for single async operations like Task.WhenAll of .NET 4)
    //         {
    //             var parallel = Observable.WhenAll(
    //                 ObservableWWW.Get("http://google.com/"),
    //                 ObservableWWW.Get("http://bing.com/"),
    //                 ObservableWWW.Get("http://unity3d.com/"));
    //
    //             parallel.Subscribe(xs =>
    //             {
    //                 Debug.Log(xs[0].Substring(0, 100)); // google
    //                 Debug.Log(xs[1].Substring(0, 100)); // bing
    //                 Debug.Log(xs[2].Substring(0, 100)); // unity
    //             });
    //         }
    //
    //         // with Progress
    //         {
    //             // notifier for progress
    //             var progressNotifier = new ScheduledNotifier<float>();
    //             progressNotifier.Subscribe(x => Debug.Log(x)); // write www.progress
    //
    //             // pass notifier to WWW.Get/Post
    //             ObservableWWW.Get("http://google.com/", progress: progressNotifier).Subscribe();
    //         }
    //
    //         // with Error
    //         {
    //             // If WWW has .error, ObservableWWW throws WWWErrorException to onError pipeline.
    //             // WWWErrorException has RawErrorMessage, HasResponse, StatusCode, ResponseHeaders
    //             ObservableWWW.Get("http://www.google.com/404")
    //                 .CatchIgnore((WWWErrorException ex) =>
    //                 {
    //                     Debug.Log(ex.RawErrorMessage);
    //                     if (ex.HasResponse)
    //                     {
    //                         Debug.Log(ex.StatusCode);
    //                     }
    //                     foreach (var item in ex.ResponseHeaders)
    //                     {
    //                         Debug.Log(item.Key + ":" + item.Value);
    //                     }
    //                 })
    //                 .Subscribe();
    //         }
    //     }
    // }
}

#endif

#if UNITY_2018_3_OR_NEWER
#pragma warning restore CS0618
#endif