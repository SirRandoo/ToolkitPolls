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
using SirRandoo.ToolkitPolls.Interfaces;
using SirRandoo.ToolkitPolls.Models;
using UnityEngine;

namespace SirRandoo.ToolkitPolls
{
    public class PollSetupBuilder
    {
        internal class ChoiceBuilder
        {
            internal string Label { get; set; }
            internal string Tooltip { get; set; }
            internal Action OnChosen { get; set; }

            [NotNull] internal IChoice Build() => new Choice {Label = Label, Tooltip = Tooltip, OnChosen = OnChosen};
        }
        
        private readonly List<ChoiceBuilder> _choices = new List<ChoiceBuilder>();
        private string _title;
        private string _titleColor;
        private Action<Rect> _coverDelegate;

        [NotNull]
        public static PollSetupBuilder Create()
        {
            return new PollSetupBuilder();
        }

        [NotNull]
        public PollSetupBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        [NotNull]
        public PollSetupBuilder WithTitle(string title, string color)
        {
            _title = title;
            _titleColor = color;
            return this;
        }

        [NotNull]
        public PollSetupBuilder WithCoverDelegate(Action<Rect> drawer)
        {
            _coverDelegate = drawer;
            return this;
        }

        [NotNull]
        public PollSetupBuilder WithChoice(string label, Action onChosen)
        {
            _choices.Add(new ChoiceBuilder {Label = label, OnChosen = onChosen});
            return this;
        }

        [NotNull]
        public PollSetupBuilder WithChoice(string label, Action onChosen, string tooltip)
        {
            _choices.Add(new ChoiceBuilder {Label = label, OnChosen = onChosen, Tooltip = tooltip});
            return this;
        }

        [NotNull]
        public IPoll Build()
        {
            return new Poll
            {
                Title = _title,
                TitleColor = _titleColor,
                CoverDrawer = _coverDelegate,
                Choices = _choices.Select(i => i.Build()).ToList()
            };
        }
    }
}
