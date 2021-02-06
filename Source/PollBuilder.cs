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
using SirRandoo.ToolkitPolls.Interfaces;
using SirRandoo.ToolkitPolls.Models;
using UnityEngine;

namespace SirRandoo.ToolkitPolls
{
    public class PollBuilder
    {
        private readonly Poll _poll;

        public PollBuilder()
        {
            _poll = new Poll {Choices = new List<IChoice>()};
        }

        public PollBuilder WithTitle(string title)
        {
            _poll.Title = title;
            return this;
        }

        public PollBuilder WithTitle(string title, string color)
        {
            _poll.TitleColor = color.StartsWith("#") ? color : $"#{color}";
            _poll.Title = title;
            return this;
        }

        public PollBuilder WithTitle(string title, Color color)
        {
            return WithTitle(title, ColorUtility.ToHtmlStringRGB(color));
        }

        public PollBuilder WithChoice(string label, Action onChosen)
        {
            _poll.Choices.Add(new Choice{Label = label, OnChosen = onChosen});
            return this;
        }

        public Poll Build() => _poll;
    }
}
