using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Samples.R3Sample
{
    public class AddToSample : MonoBehaviour
    {
        [SerializeField] private GameObject childObject;
        [SerializeField] private bool bDestroyChild;
        CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            // childObjectに紐づいたOnCollisionEnterをObservableとして取得
            childObject.transform.parent
                .OnCollisionEnterAsObservable()
                .Subscribe(collision => { Debug.Log("OnCollisionEnter: " + collision.gameObject.name); })
                // Observableの寿命をこのchildObjectに紐付ける
                .AddTo(childObject);

            // 如果碰到之前刪了，因為AddTo的關係，訂閱就沒了，就不會印出log
            DestroySelf(bDestroyChild);
        }

        void DestroySelf(bool b)
        {
            if (b)
            {
                Destroy(childObject);
            }
        }
    }
}