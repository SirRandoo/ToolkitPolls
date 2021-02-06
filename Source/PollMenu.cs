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

using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitPolls.Helpers;
using SirRandoo.ToolkitPolls.Interfaces;
using ToolkitCore.Interfaces;
using Verse;

namespace SirRandoo.ToolkitPolls
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class PollAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> Options;

        static PollAddonMenu()
        {
            Options = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "ToolkitPolls.AddonMenu.Settings".TranslateSimple(),
                    () => SettingsHelper.OpenSettingsMenuFor(LoadedModManager.GetMod<ToolkitPolls>())
                ),
                new FloatMenuOption(
                    "ToolkitPolls.AddonMenu.ClosePoll".TranslateSimple(),
                    () =>
                    {
                        IPoll currentPoll = Current.Game?.GetComponent<Coordinator>()?.CurrentPoll;

                        if (currentPoll != null)
                        {
                            currentPoll.Timer = 0;
                        }
                    }
                )
            };
        }

        public List<FloatMenuOption> MenuOptions()
        {
            return Options;
        }
    }
}
