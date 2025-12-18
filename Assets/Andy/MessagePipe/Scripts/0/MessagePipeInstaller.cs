using Game.Protocol.Bingo;
using Zenject;
using MessagePipe;

namespace Andy.MessagePipe.Scripts._0
{
    public class MessagePipeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            /*
            使用 BindMessageBroker() 的好处
            解耦：对象之间不需要直接引用，可以通过 MessageBroker 传递消息，减少耦合。
            方便管理：可以集中处理消息的发布和订阅，提高代码的可维护性。
            灵活配置：可以为不同类型的消息绑定多个 MessageBroker，适应不同的需求。
            BindMessageBroker() 是一种将消息发布/订阅模式与依赖注入结合的方法，能够简化事件驱动的应用程序开发。通过在 Zenject 容器中绑定 MessageBroker，开发者可以轻松管理消息传递和对象间的通信。
             */

            // Signal
            // Note: 將 SendName_Signal 「訊息結構」註冊到 Zenject 容器中，讓所有需要發送或接收這個訊號的腳本都能透過 Zenject 進行依賴注入 (DI)
            var option = Container.BindMessagePipe();
            Container.BindMessageBroker<BingoBoard>(option);

            // Service
            // 在這裡是 純C#，直接去看內容寫啥就好
            Container.BindInterfacesAndSelfTo<HelloWorldService>().AsSingle();

            // View
            // Warning: 在Hierarchy尋找腳本，FromComponentInHierarchy() 確實會涉及遍歷（查找）的過程，但這通常只在初始化階段發生，所以對於遊戲的運行效能影響不大。
            Container.Bind<PublishView>().FromComponentInHierarchy().AsCached();

            // Presenter
            Container.BindInterfacesAndSelfTo<PubSubPresenter>().AsSingle();
        }
    }
}