using System;
using System.Globalization;
using UnityEngine;
using R3;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Zenject;

namespace Andy.Zenject_LoadScene.Scripts
{
    public class HpDamage : MonoBehaviour
    {
        [Inject] private PlayerStats _playerStats;
        [Inject] private Tests tests;
        private float temp;

        private readonly ReactiveProperty<float> _currentHp = new(0);
        public TMP_Text hpText;
        public int intDamageOrHeal;
        public Button btn;

        [Inject]
        private void Init()
        {
            Debug.Log($"_playerStats.HpValue {_playerStats.HpValue}");
            _currentHp.Value = _playerStats.HpValue;
            hpText.text = _currentHp.Value.ToString(CultureInfo.CurrentCulture);
        }

        private void Start()
        {
            tests.Sonzai();
            _currentHp.Subscribe(_ =>
            {
                DOTween.To(() => temp, x => temp = x, _currentHp.Value, 2f).SetEase(Ease.InSine).OnUpdate(() =>
                {
                    hpText.text = temp.ToString();
                });
            });

            // 造成傷害或治癒
            btn.onClick.AddListener(delegate
            {
                Debug.Log("onclick");
                _currentHp.Value += intDamageOrHeal;
            });
        }
    }
}