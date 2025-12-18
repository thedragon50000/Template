using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MessagePipe;
using R3;
using TMPro;
using Zenject;
using Game.Protocol.Bingo;

namespace Andy.MessagePipe.Scripts._0
{
    public class PublishView : MonoBehaviour
    {
        [SerializeField] string inputMessage;
        [SerializeField] Button button;
        [SerializeField] TMP_Text text;

        // 發送者
        IPublisher<BingoBoard> _startPublisher;

        [Inject]
        public void Construct(IPublisher<BingoBoard> startPublisher)
        {
            _startPublisher = startPublisher;
        }

        void Awake()
        {
            List<int> board = new();
            for (int i = 0; i < 25; i++)
            {
                board.Add(i);
            }

            button.OnClickAsObservable()
                // 按下按鈕 發送一個string為 inputMessage 的SendName_Signal類給訂閱者
                .Subscribe(_ =>
                {
                    BingoBoard message = new();
                    message.PlayerId = "k";
                    message.BoardNumbers.AddRange(board);
                    // message.IsMarked
                    _startPublisher.Publish(message);
                })
                .AddTo(this);
        }

        public void ShowMessage(string message)
        {
            text.DOText(message, 2.5f);

            // text.text = message;
        }
    }
}