﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System.Diagnostics;
using System.Threading.Tasks;
using osu.Framework.Platform;
using osu.Game.Database;

namespace osu.Game.IPC
{
    public class ScoreIPCChannel : IpcChannel<ScoreImportMessage>
    {
        private ScoreDatabase scores;

        public ScoreIPCChannel(IIpcHost host, ScoreDatabase scores = null)
            : base(host)
        {
            this.scores = scores;
            MessageReceived += (msg) =>
            {
                Debug.Assert(scores != null);
                ImportAsync(msg.Path);
            };
        }

        public async Task ImportAsync(string path)
        {
            if (scores == null)
            {
                //we want to contact a remote osu! to handle the import.
                await SendMessageAsync(new ScoreImportMessage { Path = path });
                return;
            }

            scores.ReadReplayFile(path);
        }
    }

    public class ScoreImportMessage
    {
        public string Path;
    }
}
