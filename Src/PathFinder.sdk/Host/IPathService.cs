using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Actor.Host;

namespace PathFinder.sdk.Host
{
    public interface IPathService
    {
        string Name { get; }
        void RegisterActor(IActorHost actorHost);
    }
}
