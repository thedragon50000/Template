using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Samples.R3Sample
{
    public class AlternativeAsyncSubject : MonoBehaviour
    {
        [SerializeField] private bool bCancel;
        [SerializeField] private bool bException;

        private void Start()
        {
            // UniTaskCompletionSourceをAsyncSubjectの代わりに使ってみる
            var utcs = new UniTaskCompletionSource<int>();
            UniTaskCompletionSource<GameObject> kk = new UniTaskCompletionSource<GameObject>();

            if (bCancel)
            {
                // OnError(OperationCanceledException)とだいたい同じ
                // Note: Cancel只會印一次，通知取消
                utcs.TrySetCanceled();
            }
            else if (bException)
            {
                // OnError(new Exception())と同じ
                // Note: Exception不會取消訂閱，而是讓所有訂閱者收到錯誤消息
                utcs.TrySetException(new Exception());
            }

            GameObject o = new GameObject();
            o.name = "123";
            kk.TrySetResult(o);
            // OnNext(100) + OnCompleted()と同じ
            utcs.TrySetResult(100);

            utcs.TrySetResult(200); //無效的，因為上一行已經完成


            // -----------------------------------------------

            // ラムダ式で待ち受け。ただこれだとキャンセルができない
            utcs.Task.ContinueWith(result => Debug.Log(result));


            // async/awaitで待ち受け
            // AttachExternalCancellationでキャンセルを外付け
            UniTask.Void(async () =>
            {
                var result = await utcs.Task.AttachExternalCancellation(destroyCancellationToken);
                Debug.Log(result);
            });

            // UniTask -> ValueTask -> R3.Observable
            utcs.Task
                .AsValueTask()
                .ToObservable()
                .Subscribe(x => Debug.Log(x))
                .AddTo(this);
            
            kk.Task.AsValueTask()
                .ToObservable()
                .Subscribe(_ => print(_.name));
        }

        void OnError()
        {
        }
    }
}