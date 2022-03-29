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
using JetBrains.Annotations;
using SirRandoo.ToolkitPolls.Interfaces;
using SirRandoo.ToolkitPolls.Models;
using UnityEngine;

namespace SirRandoo.ToolkitPolls
{
    public class PollBuilder
    {
        protected Poll Poll;

        public PollBuilder()
        {
            Poll = new Poll { Choices = new List<IChoice>() };
        }

        [NotNull]
        public PollBuilder WithCoverDrawer(Action<Rect> drawer)
        {
            Poll.CoverDrawer = drawer;

            return this;
        }

        [NotNull]
        public PollBuilder WithTitle(string title)
        {
            Poll.Title = title;

            return this;
        }

        [NotNull]
        public PollBuilder WithTitle(string title, [NotNull] string color)
        {
            Poll.TitleColor = color.StartsWith("#") ? color : $"#{color}";
            Poll.Title = title;

            return this;
        }

        [NotNull] public PollBuilder WithTitle(string title, Color color) => WithTitle(title, ColorUtility.ToHtmlStringRGB(color));

        [NotNull]
        public PollBuilder WithChoice(string label, Action onChosen)
        {
            Poll.Choices.Add(new Choice { Label = label, OnChosen = onChosen });

            return this;
        }

        [NotNull]
        public PollBuilder WithChoice(string label, Action onChosen, string tooltip)
        {
            Poll.Choices.Add(new Choice { Label = label, OnChosen = onChosen, Tooltip = tooltip });

            return this;
        }

        public Poll Build() => Poll;
    }
}
