using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "DishornoredInstaller", menuName = "Installers/DishornoredInstaller")]
public class DishornoredInstaller : ScriptableObjectInstaller<DishornoredInstaller>
{
    public override void InstallBindings()
    {
    }
}