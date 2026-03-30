using UnityEngine;
using Zenject;

public class SekiroInstaller : MonoInstaller
{
    public LockCameraPosition lockCameraPosition;
    public override void InstallBindings()
    {
        Container.BindInstance(lockCameraPosition);
    }
}