// -----------------------------------------------------------------------------
//  <copyright file="LevelLoadedEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Data
{
    using System;

    public class LevelLoadedEventArgs : EventArgs
    {
        public string MapName { get; set; }

        public LevelLoadedEventArgs(String mapName)
        {
            MapName = mapName;
        }
    }

    public class LevelLoadingEventArgs : LevelLoadedEventArgs
    {
        public LevelLoadingEventArgs(string mapName) : base(mapName)
        {
        }
    }
}
