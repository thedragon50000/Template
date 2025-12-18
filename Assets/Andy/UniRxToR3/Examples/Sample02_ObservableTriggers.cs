using UnityEngine;
using R3.Triggers; // Triggers Namepsace
// using System;
using R3;

namespace UniRx.Examples
{
    public class Sample02_ObservableTriggers : MonoBehaviour
    {
        void Start()
        {
            // Get the plain object
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.UpdateAsObservable().ThrottleLastFrame(30).Subscribe(x => Debug.Log("cube"), done => Debug.Log("destroy")).AddTo(cube);
            
            // // Add ObservableXxxTrigger for handle MonoBehaviour's event as Observable
            // cube.AddComponent<ObservableUpdateTrigger>()
            //     .UpdateAsObservable()
            //     // .SampleFrame(30)
            //     .ThrottleLastFrame(30)
            //     .Subscribe(x => Debug.Log("cube"), done => Debug.Log("destroy"));

            // destroy after 3 second:)
            GameObject.Destroy(cube, 3f);
        }
    }
}