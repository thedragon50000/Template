using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.R3Sample
{
    public class SubscribeAwaitSample1 : MonoBehaviour
    {
        [SerializeField] private Button _goButton;

        [SerializeField] private bool bAwait;

        private void Start()
        {
            _goButton.OnClickAsObservable()
                .SubscribeAwait(async (_, ct) =>
                    {
                        var time = Time.time;
                        while (Time.time - time < 1f)
                        {
                            transform.position += Vector3.forward * Time.deltaTime;
                            await UniTask.Yield(ct);
                        }
                    },
                    AwaitOperation.Sequential,  //
                    // configureAwaitをTrueにすること
                    configureAwait: bAwait)
                .AddTo(this);
        }
    }
}