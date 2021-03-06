﻿// MIT License
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

using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitPolls.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls.Models
{
    public class Poll : IPoll
    {
        private float _allVotes;
        public float Timer { get; set; }

        public string Title { get; set; }

        public string TitleColor { get; set; }

        public List<IChoice> Choices { get; set; }

        public void ProcessVote(Vote vote)
        {
            if (vote.Choice <= 0 || vote.Choice > Choices.Count)
            {
                return;
            }

            IChoice choice = Choices[vote.Choice];

            foreach (IChoice c in Choices.Where(
                c => Enumerable.Any(c.Votes, v => v.Viewer.EqualsIgnoreCase(vote.Viewer))
            ))
            {
                c.UnregisterVote(vote.Viewer);
            }

            choice.RegisterVote(vote);
            _allVotes = Choices.Sum(c => c.Votes.Sum(v => v.GetTotalVotes()));
        }

        public void Draw(Rect canvas)
        {
            var listing = new Listing_Standard(PollSettings.GetTextScale());

            listing.Begin(canvas);

            foreach (IChoice choice in Choices)
            {
                Rect lineRect = listing.GetRect(Text.LineHeight);

                if (PollSettings.PollBars)
                {
                    choice.DrawBar(lineRect, choice.Votes.Count / _allVotes);
                }

                choice.Draw(lineRect);
            }

            listing.End();
            Text.Font = GameFont.Small;
        }

        public void Conclude()
        {
            int maxVotes = Choices.Max(c => c.Votes.Sum(v => v.GetTotalVotes()));
            IChoice winner = Choices.Where(c => c.Votes.Sum(v => v.GetTotalVotes()) == maxVotes).RandomElement();

            winner?.OnChosen();
        }
    }
}
