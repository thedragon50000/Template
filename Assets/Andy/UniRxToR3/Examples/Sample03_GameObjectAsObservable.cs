#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

using R3;
using UnityEngine;
using R3.Triggers; // for enable gameObject.EventAsObservbale()

namespace UniRx.Examples
{
    public class Sample03_GameObjectAsObservable : MonoBehaviour
    {
        void Start()
        {
            // All events can subscribe by ***AsObservable if enables UniRx.Triggers
            this.OnMouseDownAsObservable()
                .SelectMany(_ => gameObject.UpdateAsObservable())
                .TakeUntil(gameObject.OnMouseUpAsObservable())
                .Select(_ => Input.mousePosition)
                .Subscribe(x => Debug.Log(x), done=> Debug.Log("!!!" + "complete"));
        }
    }
}

#endif