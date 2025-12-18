// for uGUI(from 4.6)

#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx.Examples
{
    public class Sample12_ReactiveProperty : MonoBehaviour
    {
        // Open Sample12Scene. Set from canvas
        public Button myButton;
        public Toggle myToggle;
        public InputField myInput;
        public Text myText;
        public Slider mySlider;

        // You can monitor/modify in inspector by SpecializedReactiveProperty
        private readonly ReactiveProperty<int> intRxProp = new();

        private readonly Enemy enemy = new(1000);

        void Start()
        {
            // UnityEvent as Observable
            // (shortcut, MyButton.OnClickAsObservable())
            // myButton.onClick.AsObservable().Subscribe(_ => enemy.CurrentHp.Value -= 99);
            myButton.OnClickAsObservable().Subscribe(_ => enemy.CurrentHp.Value -= 99);

            // Toggle, Input etc as Observable(OnValueChangedAsObservable is helper for provide isOn value on subscribe)
            // SubscribeToInteractable is UniRx.UI Extension Method, same as .interactable = x)
            myToggle.OnValueChangedAsObservable().SubscribeToInteractable(myButton);

            // input shows delay after 1 second
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
            myInput.OnValueChangedAsObservable()
#else
            MyInput.OnValueChangeAsObservable()
#endif
                .Where(x => x != null)
                .Delay(TimeSpan.FromSeconds(1))
                .SubscribeToText(myText); // SubscribeToText is UniRx.UI Extension Method

            // converting for human visibility
            mySlider.OnValueChangedAsObservable()
                .SubscribeToText(myText, x => Math.Round(x, 2).ToString());

            // from RxProp, CurrentHp changing(Button Click) is observable
            enemy.CurrentHp.SubscribeToText(myText);
            enemy.IsDead.Where(isDead => isDead)
                .Subscribe(_ => { myToggle.interactable = myButton.interactable = false; });

            // initial text:)
            intRxProp.SubscribeToText(myText);
        }
    }

    // Reactive Notification Model
    public class Enemy
    {
        public ReactiveProperty<long> CurrentHp { get; private set; }

        public ReadOnlyReactiveProperty<bool> IsDead { get; private set; }

        public Enemy(int initialHp)
        {
            // Declarative Property
            CurrentHp = new ReactiveProperty<long>(initialHp);
            IsDead = CurrentHp.Select(x => x <= 0).ToBindableReactiveProperty();
        }
    }
}

#endif