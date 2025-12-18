using System;
using UnityEngine;
using System.Collections;
using R3;
using R3.Triggers;
using UnityEngine.UI;

namespace UniRxWorkBook.StreamSources
{
    public class Lesson_5_ReactiveProperty : MonoBehaviour
    {
        [SerializeField] private Text subscribeText; //RPをSubscribeした結果を表示する
        [SerializeField] private Text updateText; //Updat内でRPを監視して表示する
        [SerializeField] private Text toReactivePropertyText; //ストリームをRPに変換して表示する

        // ReactiveProperty(以下RP）は、普通の変数にSubjectの機能を付け加えたものである
        // 要するに、「Subscribeができる変数」である。挙動としてはBehaviorSubjectに似ている
        //
        // RPにはValueというプロパティが用意されており、このValueに値を代入することでOnNextを発行することができる
        // また、このValueはSubscribeなしで値にアクセス可能である（つまりSubscribeせずに普通の変数のようにReadができる）
        private ReactiveProperty<int> _reactiveProperty = new(0);

        private void Start()
        {
            //RPをSubscribeしてsubscribeTextに反映してみるパターン
            _reactiveProperty.Subscribe(value => subscribeText.text = value.ToString());

            //-------------------

            //RPのValueを毎フレームReadしてupdateTextに反映してみるパターン
            this.UpdateAsObservable().Subscribe(_ =>
            {
                //RPのValueプロパティでいつでも最新の値が取得できる
                updateText.text = _reactiveProperty.Value.ToString();
            });

            //-------------------

            //他のストリームをReactivePropertyに変換してみるパターン
            // Note: R3的 Observable.Timer已經全面改成Observable<Unit>，所以得改用Scan計數
            var interbalRP
                = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1)) //1秒毎にカウントアップ
                    .Scan(-1, (count, _) => count + 1)
                    .ToBindableReactiveProperty(); //RPに変換(初期値は０） 

            interbalRP.Subscribe(value => toReactivePropertyText.text = value.ToString())
                .AddTo(this); //GameObject破棄時にDisposeする

            //-------------------
            //RPの更新開始
            StartCoroutine(CountUpCoroutine());
        }

        //RPを１秒毎に更新する（カウントアップする）
        private IEnumerator CountUpCoroutine()
        {
            yield return new WaitForSeconds(1);
            while (true)
            {
                _reactiveProperty.Value++;
                yield return new WaitForSeconds(1);
            }
        }
    }
}