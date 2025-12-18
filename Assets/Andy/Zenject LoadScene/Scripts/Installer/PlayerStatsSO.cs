using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Andy.Zenject_LoadScene.Scripts
{
    [CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Installers/PlayerStatsSO")]
    public class PlayerStatsSo : ScriptableObjectInstaller<PlayerStatsSo>
    {
        public PlayerStats playerStats;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Tests>().AsSingle();
        }
    }
}