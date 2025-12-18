using System;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using R3;
using UnityEngine.UI;

namespace UniRxWorkBook.StreamSources
{
    public class Lesson_4_AsyncSubject : MonoBehaviour
    {
        [SerializeField] private Button subscribeButton;
        [SerializeField] private Button onNextButton;
        [SerializeField] private Button onCompletedButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Text resultText;

        private ReactiveProperty<int> _onNextCount = new();

        // AsyncSubjectは非同期処理を模したSubjectである
        // OnNextが発行されるたびに最新の値をキャッシュし、OnCompletedの実行時に最新のOnNextを１つだけ通知する
        // （非同期で処理を回し、非同期処理完了を持って最後の結果を通知するイメージである）
        private TaskCompletionSource<int> _taskCompletionSource;
        private IDisposable _subscribe;

        private void Awake()
        {
            _onNextCount.Value = 7;
        }

        private void Start()
        {
            //BehaviorSubjectは初期値を設定できる
            _taskCompletionSource = new TaskCompletionSource<int>();

            /*
            Subscribe後にOnNextを繰り返しても値が発行されず、OnCompletedを実行した際に初めてOnNextが通知されるところ確認しよう
            また、その時のOnNextの値は最後の値１つだけになっていることも確認しよう
            */

            // Subscribeボタンが押されたらSubjectをSubscribeしてresultTextに表示する
            subscribeButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (_subscribe == null)
                {
                    _subscribe = _onNextCount.SubscribeToText(resultText);

                    // resultText._taskCompletionSource.TrySetResult(_onNextCount);
                    // _taskCompletionSource.Subscribe(
                    //     time => resultText.text += time.ToString() + "　", //OnNext
                    //     () => resultText.text += "OnCompleted　"); //OnCompleted
                }
            });

            // OnNextボタンが押されたら今が何度目のOnNextであるかを発行する
            onNextButton.OnClickAsObservable().Subscribe(_ =>
            {
                _onNextCount.Value++;
                // if (_taskCompletionSource != null)
                // {
                //     _taskCompletionSource.OnNext(++onNextCount);
                // }
            });

            // OnCompletedボタンが押されたらOnCompletedを発行する
            onCompletedButton.OnClickAsObservable().Subscribe(_ => { _taskCompletionSource?.TrySetResult(_onNextCount.Value); });

            // Resetボタンが押されたら全体を初期化する
            resetButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (_taskCompletionSource != null)
                {
                    _taskCompletionSource.TrySetCanceled();
                }

                _taskCompletionSource = new TaskCompletionSource<int>();
                _subscribe = null;
                _onNextCount.Dispose();
                resultText.text = "";
                _onNextCount.Value = 0;
            });
        }
    }
}