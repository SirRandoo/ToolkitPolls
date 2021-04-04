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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitPolls.Helpers;
using SirRandoo.ToolkitPolls.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls.Models
{
    public class Poll : IPoll
    {
        private float _allVotes;
        private IChoice _winner;

        public float ResultsTimer { get; set; }

        public float Timer { get; set; }

        public string Title { get; set; }

        public string TitleColor { get; set; }

        public List<IChoice> Choices { get; set; }

        public IPoll.PollState State { get; set; }

        public Action<Rect> CoverDrawer { get; set; }

        public float CoverTimer { get; set; }

        public void ProcessVote([NotNull] Vote vote)
        {
            if (vote.Choice <= 0 || vote.Choice > Choices.Count || State == IPoll.PollState.Results)
            {
                return;
            }

            IChoice choice = Choices[vote.Choice - 1];

            foreach (IChoice c in Choices.Where(
                c => Enumerable.Any(c.Votes, v => v.Viewer.EqualsIgnoreCase(vote.Viewer))
            ))
            {
                c.UnregisterVote(vote.Viewer);
            }

            choice.RegisterVote(vote);
            _allVotes = Choices.Sum(c => c.Votes.Sum(v => v.GetTotalVotes()));
        }

        public void DrawPoll(Rect canvas)
        {
            GameFont font = PollSettings.GetTextScale();
            var listing = new Listing_Standard(font);

            listing.Begin(canvas);

            for (var index = 0; index < Choices.Count; index++)
            {
                IChoice choice = Choices[index];
                Rect lineRect = listing.GetRect(Text.LineHeight);
                var numRect = new Rect(lineRect.x, lineRect.y, font == GameFont.Medium ? 30f : 25f, lineRect.height);
                var choiceRect = new Rect(
                    numRect.x + numRect.width + 2f,
                    lineRect.y,
                    lineRect.width - numRect.width - 2f,
                    lineRect.height
                );

                if (PollSettings.PollBars)
                {
                    choice.DrawBar(lineRect, choice.Votes.Sum(v => v.GetTotalVotes()) / _allVotes);
                }

                SettingsHelper.DrawLabel(numRect, $"<b>#{index + 1f}</b>", TextAnchor.MiddleCenter, font);
                choice.Draw(choiceRect);
            }

            listing.End();
            Text.Font = GameFont.Small;
        }

        public void DrawCover(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            CoverDrawer?.Invoke(canvas.AtZero());
            GUI.EndGroup();
        }

        public void DrawResults(Rect canvas)
        {
            GameFont font = PollSettings.GetTextScale();
            var listing = new Listing_Standard(font);

            listing.Begin(canvas);

            for (var index = 0; index < Choices.Count; index++)
            {
                IChoice choice = Choices[index];
                Rect lineRect = listing.GetRect(Text.LineHeight);
                var numRect = new Rect(lineRect.x, lineRect.y, font == GameFont.Medium ? 30f : 25f, lineRect.height);
                var choiceRect = new Rect(
                    numRect.x + numRect.width + 2f,
                    lineRect.y,
                    lineRect.width - numRect.width - 2f,
                    lineRect.height
                );

                if (PollSettings.PollBars && choice == _winner)
                {
                    Widgets.DrawHighlightSelected(lineRect);
                }

                SettingsHelper.DrawLabel(numRect, $"#{index + 1f}", TextAnchor.MiddleCenter, font);
                choice.Draw(choiceRect);
            }

            listing.End();
            Text.Font = GameFont.Small;
        }

        public void GetWinningChoice()
        {
            int maxVotes = Choices.Where(i => i.OnChosen != null).Max(c => c.Votes.Sum(v => v.GetTotalVotes()));
            _winner = Choices.Where(i => i.OnChosen != null)
               .Where(c => c.Votes.Sum(v => v.GetTotalVotes()) == maxVotes)
               .RandomElement();
        }

        public void Conclude()
        {
            int maxVotes = Choices.Max(c => c.Votes.Sum(v => v.GetTotalVotes()));
            IChoice winner = Choices.Where(c => c.Votes.Sum(v => v.GetTotalVotes()) == maxVotes).RandomElement();

            winner?.OnChosen?.Invoke();
        }
    }
}
