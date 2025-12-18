using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R3;
using R3.Triggers;
using UnityEngine;

namespace UniRx.Examples
{
    public class Sample08_DetectDoubleClick : MonoBehaviour
    {
        void Start()
        {
            // Global event handling is very useful.
            // UniRx can handle there events.
            // Observable.EveryUpdate/EveryFixedUpdate/EveryEndOfFrame
            // Observable.EveryApplicationFocus/EveryApplicationPause
            // Observable.OnceApplicationQuit

            // This DoubleCLick Sample is from
            // The introduction to Reactive Programming you've been missing
            // https://gist.github.com/staltz/868e7e9bc2a7b8c1f754

            Observable<Unit> clickStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0));

            clickStream.Chunk(clickStream.ThrottleLast(TimeSpan.FromSeconds(5)))
                .Select(e => e.Length) // 將事件的array長度 轉換成 int 傳入下一個操作符
                .Where(count => count >= 2) // 當 select傳入的值>=2的時候才會執行subscribe裡面的內容
                .Subscribe(count => { Debug.Log("DoubleClick Detected! Count:" + count); });
        }
    }
}