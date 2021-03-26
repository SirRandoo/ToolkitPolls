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
using SirRandoo.ToolkitPolls.Helpers;
using SirRandoo.ToolkitPolls.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls.Models
{
    public class Choice : IChoice
    {
        private static readonly Dictionary<int, float> WidthCache = new Dictionary<int, float>();
        private static bool _scaleChanged;
        private float _displayPercentage;
        private string _label;
        private float _labelWidth;
        private int _totalVotes;
        private string _totalVotesLabel;
        private float _totalVotesWidth;

        public string Label
        {
            get => _label;
            set
            {
                _label = value;

                Text.Font = PollSettings.GetTextScale();
                _labelWidth = Text.CalcSize(_label).x;
                Text.Font = GameFont.Small;
            }
        }

        public Action OnChosen { get; set; }

        public List<Vote> Votes { get; set; } = new List<Vote>();

        public void Draw(Rect canvas)
        {
            var labelRect = new Rect(canvas.x, canvas.y, _labelWidth, canvas.height);
            var voterRect = new Rect(
                canvas.x + canvas.width - _totalVotesWidth,
                canvas.y,
                _totalVotesWidth,
                canvas.height
            );
            GameFont scale = PollSettings.GetTextScale();

            SettingsHelper.DrawLabel(labelRect, _label, fontScale: scale);
            SettingsHelper.DrawLabel(voterRect, _totalVotesLabel, fontScale: scale);
        }

        public void DrawBar(Rect canvas, float percentage)
        {
            if (Mathf.Abs(percentage - _displayPercentage) >= 0.0001f)
            {
                _displayPercentage = Mathf.SmoothStep(_displayPercentage, percentage, 0.2f);
            }

            var region = new Rect(
                canvas.x,
                canvas.y + 2f,
                Mathf.FloorToInt(canvas.width * _displayPercentage),
                canvas.height - 4f
            );

            if (PollSettings.Colorless)
            {
                Widgets.DrawLightHighlight(region);
            }
            else
            {
                Texture2D.whiteTexture.DrawColored(region, new Color(0.2f, 0.8f, 0.85f, 0.4f));
            }
        }

        public void RegisterVote(Vote viewer)
        {
            Votes.RemoveAll(v => v.Viewer.EqualsIgnoreCase(viewer.Viewer));
            Votes.Add(viewer);
            UpdateVoteCache();
        }

        public void UnregisterVote(string viewer)
        {
            Votes.RemoveAll(v => v.Viewer.EqualsIgnoreCase(viewer));
            _totalVotes = Votes.Sum(v => v.GetTotalVotes());
            UpdateVoteCache();
        }

        private void UpdateVoteCache()
        {
            _totalVotes = Votes.Sum(v => v.GetTotalVotes());
            _totalVotesLabel = _totalVotes.ToString("N0");
            _totalVotesWidth = GetWidth(_totalVotes);
        }

        private static float GetWidth(int votes)
        {
            if (!_scaleChanged && WidthCache.TryGetValue(votes, out float cache))
            {
                return cache;
            }

            if (_scaleChanged)
            {
                WidthCache.Clear();
                _scaleChanged = false;
            }

            GameFont prevFont = Text.Font;
            Text.Font = PollSettings.GetTextScale();
            WidthCache[votes] = cache = Text.CalcSize(votes.ToString("N0")).x;
            Text.Font = prevFont;

            return cache;
        }

        internal static void NotifyScaleChanged()
        {
            _scaleChanged = true;
        }
    }
}
