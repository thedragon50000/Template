using UnityEngine;
using System.Collections;
using R3;
using UnityEngine.UI;

namespace UniRxWorkBook.Operators
{
    public class Lesson_8_Repeat : MonoBehaviour
    {
        [SerializeField] private Text resultLabel;
        [SerializeField] private Button buttonLeft;
        [SerializeField] private Button buttonRight;

        private void Start()
        {
            var rightStream = buttonRight.OnClickAsObservable();
            var leftStream = buttonLeft.OnClickAsObservable();

            // _____を書き換え、LeftとRightを交互に1回ずつ押した時にOKが表示されるようにしよう
            // 
            // First()を外すだけではLeftとRightを連打した時の挙動が怪しいのでダメである
            // 適切なオペレータをFirstの後ろに入れよう
            
            var singleExecution = Observable.Zip(
                leftStream,
                rightStream,
                (_, _) => resultLabel.text += "OK\n" // 在 SubscribeToText 之前就執行 side effect
            ).Take(1);
            
            Observable.Concat(
                    singleExecution, // 第一次執行
                    singleExecution, // 第二次執行
                    singleExecution // 第三次執行
                )
                .Subscribe() // 只需要 Subscribe 來啟動整個流程
                .AddTo(this);
        }
    }
}