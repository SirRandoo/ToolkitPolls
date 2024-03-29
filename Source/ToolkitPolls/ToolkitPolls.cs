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

using JetBrains.Annotations;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Interfaces;
using SirRandoo.ToolkitPolls.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls
{
    [UsedImplicitly]
    public class ToolkitPolls : Mod
    {
        private static RimLogger _logger;
        public ToolkitPolls(ModContentPack content) : base(content)
        {
            GetSettings<PollSettings>();
        }

        [NotNull] public static IRimLogger Logger => _logger ??= new RimLogger("ToolkitPolls");

        [NotNull] public override string SettingsCategory() => Content.Name;

        public override void DoSettingsWindowContents(Rect canvas)
        {
            PollSettings.Draw(canvas);
        }
        public static bool CanSchedulePoll() => UnityData.IsInMainThread && !(Current.Game is null);

        public static bool SchedulePoll(IPoll poll)
        {
            if (!CanSchedulePoll())
            {
                return false;
            }

            var coordinator = Current.Game.GetComponent<Coordinator>();

            if (coordinator is null)
            {
                return false;
            }

            coordinator.Schedule(poll);

            return true;
        }

        public static bool SchedulePoll(PollSetupBuilder builder)
        {
            if (Current.Game == null)
            {
                return false;
            }

            var coordinator = Current.Game.GetComponent<Coordinator>();

            if (coordinator is null)
            {
                return false;
            }

            coordinator.Schedule(builder);

            return true;
        }
    }
}
