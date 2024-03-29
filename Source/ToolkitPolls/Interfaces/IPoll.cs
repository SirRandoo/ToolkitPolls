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

using System;
using System.Collections.Generic;
using SirRandoo.ToolkitPolls.Models;
using UnityEngine;

namespace SirRandoo.ToolkitPolls.Interfaces
{
    public interface IPoll
    {
        enum PollState { Cover, Poll, Results }

        PollState State { get; set; }

        public Action<Rect> CoverDrawer { get; set; }
        public float CoverTimer { get; set; }
        public float ResultsTimer { get; set; }

        public float Timer { get; set; }
        public string Title { set; get; }
        public string TitleColor { get; set; }
        public List<IChoice> Choices { get; set; }
        public void ProcessVote(Vote vote);
        public void DrawPoll(Rect canvas);
        public void DrawCover(Rect canvas);
        public void DrawResults(Rect canvas);
        public void GetWinningChoice();
        public void Conclude();
    }
}
