using System;
using System.Collections.Generic;
using GameDesire.Rest;
using Zenject;

namespace DIFramework
{
    public class RestInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var client = new RestClient("http://localhost:8080", "healthcheck/check", new TimeSpan(0,0,0,3), new Dictionary<string, string>());
            Container.Bind<RestClient>().ToSelf().FromInstance(client).AsSingle().NonLazy();
        }
    }
}