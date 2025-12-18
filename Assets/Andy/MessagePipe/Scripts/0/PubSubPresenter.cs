using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;
using Zenject;

namespace Andy.MessagePipe.Scripts._0
{
    public class PubSubPresenter : IInitializable, IDisposable
    {
        readonly HelloWorldService _service;
        readonly PublishView _view;
        readonly CompositeDisposable _disposable = new();

        [Inject]
        public PubSubPresenter(HelloWorldService service, PublishView view)
        {
            _service = service;
            _view = view;
        }

        public void Initialize()
        {
            // 容器內有一個HelloWorldService
            _service.UserNameProperty
            // 訂閱在Hierarchy上的物件的void
                .Subscribe(name => _view.ShowMessage(name))
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}