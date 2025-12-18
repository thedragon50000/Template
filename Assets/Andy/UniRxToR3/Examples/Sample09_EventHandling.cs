#pragma warning disable 0067

using System;
using UnityEngine;
using R3;

namespace UniRx.Examples
{
    public class Sample09_EventHandling : MonoBehaviour
    {
        public class MyEventArgs : EventArgs
        {
            public int MyProperty { get; set; }
        }

        /// <summary>
        /// 標準的 C# 事件模式，帶有 sender 和 EventArgs 參數。
        /// </summary>
        public event EventHandler<MyEventArgs> FooBar;
        
        /// <summary>
        /// 簡化的 C# 事件模式，只帶有一個 int參數(當然也可以宣告其他屬性)
        /// </summary>
        public event Action<int> FooFoo;

        private readonly CompositeDisposable disposables = new();

        // Subject is Rx's native event expression and recommend way for use Rx as event.
        // Subject.OnNext as fire event,
        // expose IObserver is subscibable for external source, it's no need convert.
        // Note: Subject 是 Rx 的原生事件表達式，也是推薦使用 Rx 作為事件的途徑。
        //  Subject.OnNext 用於觸發事件，
        //  暴露 IObserver 介面供外部來源訂閱，無需轉換。
        private readonly Subject<(int, string)> onBarBar = new ();
        private Subject<(int, string)> OnBarBar => onBarBar;

        void Start()
        {
            // convert to IO<EventArgs>, many situation this is useful than FromEventPattern
            Observable.FromEvent<EventHandler<MyEventArgs>, MyEventArgs>(
                    h => (sender, e) => h(e), h => FooBar += h, h => FooBar -= h)
                .Subscribe()
                .AddTo(disposables);

            // You can convert Action like event.
            Observable.FromEvent<int>(
                    h => FooFoo += h, h => FooFoo -= h)
                .Subscribe()
                .AddTo(disposables);

            // Subject as like event.
            OnBarBar.Subscribe().AddTo(disposables);
            onBarBar.OnNext((1, "")); // fire event
        }

        void OnDestroy()
        {
            // manage subscription lifecycle
            disposables.Dispose();
        }
    }
}

#pragma warning restore 0067