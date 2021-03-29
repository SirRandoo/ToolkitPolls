// MIT License
// 
// Copyright (c) 2021 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitPolls.Helpers;
using SirRandoo.ToolkitPolls.Interfaces;
using SirRandoo.ToolkitPolls.Models;
using SirRandoo.ToolkitPolls.Windows;
using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls
{
    [UsedImplicitly]
    public class Coordinator : TwitchInterfaceBase
    {
        private readonly ConcurrentQueue<IPoll> _pendingPolls = new ConcurrentQueue<IPoll>();
        private readonly ConcurrentQueue<Vote> _votes = new ConcurrentQueue<Vote>();
        private IPoll _currentPoll;
        private float _marker;
        public Coordinator(Game game) { }

        internal IPoll CurrentPoll
        {
            get => _currentPoll;
            private set
            {
                _currentPoll = value;

                if (_currentPoll != null && !Find.WindowStack.IsOpen(typeof(PollDialog)))
                {
                    Find.WindowStack.Add(new PollDialog());
                }
            }
        }

        public override void ParseMessage(ITwitchMessage twitchMessage)
        {
            if (CurrentPoll == null)
            {
                return;
            }

            string[] segments = twitchMessage.Message.Split(new[] {' '}, 1, StringSplitOptions.RemoveEmptyEntries);
            string maybeVote = segments.FirstOrDefault();

            if (maybeVote.NullOrEmpty() || !int.TryParse(maybeVote!.TrimStart('#'), out int vote))
            {
                return;
            }

            if (vote <= 0 || vote > CurrentPoll.Choices.Count)
            {
                return;
            }

            _votes.Enqueue(
                new Vote
                {
                    Choice = vote,
                    Viewer = twitchMessage.Username.ToLowerInvariant(),
                    UserTypes = twitchMessage.ChatMessage.GetUserType()
                }
            );
        }

        public override void GameComponentTick()
        {
            if (LegacyHelper.HasActivePoll() || _pendingPolls.IsEmpty || !_pendingPolls.TryDequeue(out IPoll poll))
            {
                return;
            }

            poll.CoverTimer = 10f;
            poll.ResultsTimer = 10f;
            poll.Timer = PollSettings.Duration;
            CurrentPoll = poll;
            _marker = Time.unscaledTime;

            if (PollSettings.ChoicesInChat)
            {
                SendChoicesToChat();
            }
        }

        public override void GameComponentOnGUI()
        {
            if (LegacyHelper.HasActivePoll())
            {
                var container = new List<IPoll>();

                while (!_pendingPolls.IsEmpty)
                {
                    if (_pendingPolls.TryDequeue(out IPoll poll))
                    {
                        container.Add(poll);
                    }
                }

                container.Insert(0, CurrentPoll);
                CurrentPoll = null;
                Find.WindowStack.TryRemove(typeof(PollDialog), false);

                foreach (IPoll poll in container)
                {
                    _pendingPolls.Enqueue(poll);
                }
            }

            if (CurrentPoll != null)
            {
                ProcessCurrentPoll();
            }
        }

        private void ProcessCurrentPoll()
        {
            switch (CurrentPoll.State)
            {
                case IPoll.PollState.Cover:
                    CurrentPoll.CoverTimer -= Time.unscaledTime - _marker;
                    _marker = Time.unscaledTime;
                    ProcessVotes();

                    if (CurrentPoll.CoverDrawer == null || CurrentPoll.CoverTimer <= 0)
                    {
                        CurrentPoll.State = IPoll.PollState.Poll;
                    }

                    break;
                case IPoll.PollState.Poll:
                    CurrentPoll.Timer -= Time.unscaledTime - _marker;
                    _marker = Time.unscaledTime;
                    ProcessVotes();

                    if (CurrentPoll.Timer <= 0)
                    {
                        CurrentPoll.State = IPoll.PollState.Results;
                        CurrentPoll.GetWinningChoice();
                    }

                    break;
                case IPoll.PollState.Results:
                    CurrentPoll.ResultsTimer -= Time.unscaledTime - _marker;
                    _marker = Time.unscaledTime;

                    if (CurrentPoll.ResultsTimer <= 0)
                    {
                        CurrentPoll.Conclude();
                        CurrentPoll = null;
                        _votes.Clear();
                        Find.WindowStack.TryRemove(typeof(PollDialog), false);
                    }

                    break;
            }
        }

        private void ProcessVotes()
        {
            while (!_votes.IsEmpty)
            {
                if (!_votes.TryDequeue(out Vote vote))
                {
                    break;
                }

                ProcessVote(vote);
            }
        }

        private void ProcessVote(Vote vote)
        {
            CurrentPoll.ProcessVote(vote);
        }

        public void Schedule(IPoll poll)
        {
            _pendingPolls.Enqueue(poll);
        }

        private void SendChoicesToChat()
        {
            if (!TwitchWrapper.Client?.IsConnected ?? true)
            {
                return;
            }

            TwitchWrapper.SendChatMessage(CurrentPoll.Title);

            for (var index = 0; index < CurrentPoll.Choices.Count; index++)
            {
                IChoice choice = CurrentPoll.Choices[index];
                TwitchWrapper.SendChatMessage($"[{index + 1}] {choice.Label}");
            }
        }
    }
}
