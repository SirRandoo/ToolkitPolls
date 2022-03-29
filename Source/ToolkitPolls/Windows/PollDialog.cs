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
using System.Linq;
using CommonLib.Helpers;
using SirRandoo.ToolkitPolls.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls.Windows
{
    [StaticConstructorOnStartup]
    public class PollDialog : Window
    {
        internal const float Width = 310f;
        internal const float BaseHeight = 150f;
        private static readonly Gradient TimerGradient;

        private Coordinator _coordinator;

        static PollDialog()
        {
            TimerGradient = new Gradient();

            var colorKey = new GradientColorKey[2];
            colorKey[0].color = Color.green;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.red;
            colorKey[1].time = 1.0f;

            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;

            TimerGradient.SetKeys(colorKey, alphaKey);
        }

        public PollDialog()
        {
            doCloseX = true;
            draggable = true;
            closeOnCancel = false;
            closeOnAccept = false;
            focusWhenOpened = false;
            closeOnClickedOutside = false;
            absorbInputAroundWindow = false;
            preventCameraMotion = false;
        }

        public override Vector2 InitialSize => new Vector2(Width, BaseHeight);

        public override void PreOpen()
        {
            base.PreOpen();

            _coordinator = Current.Game?.GetComponent<Coordinator>();

            if (_coordinator is null)
            {
                ToolkitPolls.Logger.Warn("Polls can only be processed when a save is actively loaded.");
                Close();
            }

            optionalTitle = string.IsNullOrEmpty(_coordinator.CurrentPoll.TitleColor) || PollSettings.Colorless
                ? _coordinator.CurrentPoll?.Title
                : _coordinator.CurrentPoll?.Title.ColorTagged(_coordinator.CurrentPoll.TitleColor);

            optionalTitle = optionalTitle.Tagged("b");

            SetInitialSizeAndPosition();
        }

        public override void DoWindowContents(Rect canvas)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            var pollRect = new Rect(0f, 0f, canvas.width, canvas.height - Text.SmallFontHeight);
            var timerRect = new Rect(0f, canvas.height - Text.SmallFontHeight, canvas.width, Text.SmallFontHeight);

            GUI.BeginGroup(canvas);

            GUI.BeginGroup(pollRect);
            Rect innerRect = pollRect.AtZero();

            switch (_coordinator.CurrentPoll?.State)
            {
                case IPoll.PollState.Cover:
                    _coordinator.CurrentPoll?.DrawCover(innerRect);

                    break;
                case IPoll.PollState.Poll:
                    _coordinator.CurrentPoll?.DrawPoll(innerRect);

                    break;
                case IPoll.PollState.Results:
                    _coordinator.CurrentPoll?.DrawResults(innerRect);

                    break;
            }

            GUI.EndGroup();

            GUI.BeginGroup(timerRect);

            var progress = 0f;

            switch (_coordinator.CurrentPoll?.State)
            {
                case IPoll.PollState.Cover:
                    progress = (float)_coordinator.CurrentPoll?.CoverTimer / PollSettings.CoverDuration;

                    break;
                case IPoll.PollState.Poll:
                    progress = (float)_coordinator.CurrentPoll?.Timer / PollSettings.PollDuration;

                    break;
                case IPoll.PollState.Results:
                    progress = (float)_coordinator.CurrentPoll?.ResultsTimer / PollSettings.ResultsDuration;

                    break;
            }

            GUI.color = PollSettings.Colorless ? ColorLibrary.Teal : TimerGradient.Evaluate(1f - progress);
            Widgets.FillableBar(timerRect.AtZero(), progress, Texture2D.whiteTexture, null, true);
            GUI.color = Color.white;
            GUI.EndGroup();

            GUI.EndGroup();
        }

        protected override void SetInitialSizeAndPosition()
        {
            GameFont lastFont = Text.Font;
            Text.Font = PollSettings.GetTextScale();

            Vector2 initialSize = InitialSize;
            float desiredWidth = _coordinator?.CurrentPoll?.Choices?.Max(c => Text.CalcSize(c.Label).x) ?? 0f;
            float finalWidth = Mathf.Max(initialSize.x, desiredWidth);
            float finalHeight = initialSize.y + Text.LineHeight * (_coordinator?.CurrentPoll?.Choices?.Count ?? 0f);

            windowRect = new Rect(
                Mathf.Clamp(PollSettings.PollDialogX, 0f, UI.screenWidth - finalWidth),
                Mathf.Clamp(PollSettings.PollDialogY, 0f, UI.screenHeight - finalHeight),
                Mathf.Max(initialSize.x, desiredWidth),
                finalHeight
            );

            Text.Font = lastFont;
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            if (_coordinator.CurrentPoll?.State == IPoll.PollState.Results && _coordinator.CurrentPoll?.ResultsTimer <= 0)
            {
                Close();
            }
        }

        public override void PreClose()
        {
            base.PreClose();
            PersistWindowPosition();
        }

        public override void PostClose()
        {
            base.PostClose();

            if (!(_coordinator.CurrentPoll is null))
            {
                _coordinator.CurrentPoll.Timer -= PollSettings.PollDuration;
            }
        }

        private void PersistWindowPosition()
        {
            if (Math.Abs(windowRect.x - PollSettings.PollDialogX) < 0.1f && Math.Abs(windowRect.y - PollSettings.PollDialogY) < 0.1f)
            {
                return;
            }

            PollSettings.PollDialogX = windowRect.x;
            PollSettings.PollDialogY = windowRect.y;
            LoadedModManager.GetMod<ToolkitPolls>().WriteSettings();
        }
    }
}
