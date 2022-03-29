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
using SirRandoo.ToolkitPolls.Models;
using TwitchToolkit.Votes;
using UnityEngine;
using Vote = TwitchToolkit.Votes.Vote;

namespace SirRandoo.ToolkitPolls.TwitchToolkit.Models
{
    public class ToolkitPoll : Poll
    {
        private readonly Vote _vote;
        private bool _signaled;

        public ToolkitPoll(Vote vote)
        {
            _vote = vote;
        }

        public override void DrawPoll(Rect region)
        {
            if (_signaled)
            {
                return;
            }

            VoteHandler.voteActive = _signaled;
            VoteHandler.currentVote = _vote;
            VoteHandler.voteStartedAt = DateTime.Now;
            _signaled = true;

            base.DrawPoll(region);
        }

        public override void Conclude()
        {
            base.Conclude();

            VoteHandler.voteActive = false;
        }
    }
}
