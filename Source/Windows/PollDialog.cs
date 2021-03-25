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
using SirRandoo.ToolkitPolls.Helpers;
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

        public override Vector2 InitialSize => new Vector2(Width, BaseHeight);

        public override void PreOpen()
        {
            base.PreOpen();

            _coordinator = Current.Game?.GetComponent<Coordinator>();

            if (_coordinator == null)
            {
                LogHelper.Warn("Polls can only be processed a save is actively loaded.");
                Close();
            }

            optionalTitle = _coordinator.CurrentPoll.TitleColor.NullOrEmpty() || PollSettings.Colorless
                ? _coordinator.CurrentPoll?.Title
                : _coordinator.CurrentPoll?.Title.ColorTagged(_coordinator.CurrentPoll.TitleColor);

            SetInitialSizeAndPosition();
        }

        public override void DoWindowContents(Rect canvas)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            var pollRect = new Rect(0f, 0f, canvas.width, canvas.height - Text.SmallFontHeight);
            var timerRect = new Rect(0f, canvas.y - Text.SmallFontHeight, canvas.width, Text.SmallFontHeight);

            GUI.BeginGroup(canvas);
            _coordinator.CurrentPoll?.Draw(pollRect);


            float progress = (_coordinator.CurrentPoll?.Timer ?? 0f) / PollSettings.Duration;
            GUI.color = TimerGradient.Evaluate(1f - progress);
            Widgets.FillableBar(timerRect, progress, Texture2D.whiteTexture, null, true);
            GUI.color = Color.white;

            GUI.EndGroup();
        }

        protected override void SetInitialSizeAndPosition()
        {
            GameFont lastFont = Text.Font;
            Text.Font = PollSettings.GetTextScale();
            
            Vector2 initialSize = InitialSize;
            windowRect = new Rect(
                Mathf.Clamp(PollSettings.PollDialogX, 0f, UI.screenWidth - initialSize.x),
                Mathf.Clamp(PollSettings.PollDialogY, 0f, UI.screenHeight - initialSize.y),
                initialSize.x + (_coordinator?.CurrentPoll?.Choices?.Max(c => Text.CalcSize(c.Label).x) ?? 0f),
                initialSize.y + Text.LineHeight * (_coordinator?.CurrentPoll?.Choices?.Count ?? 0f)
            );

            Text.Font = lastFont;
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            if (_coordinator.CurrentPoll?.Timer <= 0)
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

            if (_coordinator.CurrentPoll != null)
            {
                _coordinator.CurrentPoll.Timer -= PollSettings.Duration;
            }
        }

        private void PersistWindowPosition()
        {
            if (Math.Abs(windowRect.x - PollSettings.PollDialogX) < 0.1f
                && Math.Abs(windowRect.y - PollSettings.PollDialogY) < 0.1f)
            {
                return;
            }

            PollSettings.PollDialogX = windowRect.x;
            PollSettings.PollDialogY = windowRect.y;
            LoadedModManager.GetMod<ToolkitPolls>().WriteSettings();
        }
    }
}
