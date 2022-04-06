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
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitPolls.Models;
using SirRandoo.ToolkitPolls.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls
{
    public class PollSettings : ModSettings
    {
        public static bool Colorless;
        public static int MaxChoices = 4;
        public static int CoverDuration = 5;
        public static int PollDuration = 300;
        public static int ResultsDuration = 5;
        public static bool PollBars = true;
        public static bool TieredVotes = true;
        public static bool LargeText;
        public static bool ChoicesInChat;

        public static int SubscriberWeight = 1;
        public static int VipWeight = 1;
        public static int FounderWeight = 1;
        public static int ModeratorWeight = 1;

        internal static float PollDialogX = Mathf.Floor(UI.screenWidth - PollDialog.Width);
        internal static float PollDialogY = Mathf.Floor(UI.screenHeight / 3f);

        private static string _maxChoicesBuffer;
        private static string _coverDurationBuffer;
        private static string _pollDurationBuffer;
        private static string _resultsDurationBuffer;
        private static string _subscriberWeightBuffer;
        private static string _vipWeightBuffer;
        private static string _founderWeightBuffer;
        private static string _moderatorWeightBuffer;
        private static Vector2 _scrollPosition = Vector2.zero;

        public static void Draw(Rect canvas)
        {
            var listing = new Listing_Standard(GameFont.Small);
            var viewPort = new Rect(0f, 0f, canvas.width - 16f, Text.SmallFontHeight * 35f);

            GUI.BeginGroup(canvas);
            Widgets.BeginScrollView(canvas.AtZero(), ref _scrollPosition, viewPort);
            listing.Begin(viewPort);

            listing.GroupHeader("ToolkitPolls.SettingGroups.General".TranslateSimple(), false);
            (Rect choicesLabel, Rect choicesField) = listing.Split();
            UiHelper.Label(choicesLabel, "ToolkitPolls.Settings.MaxChoices.Label".TranslateSimple());
            Widgets.TextFieldNumeric(choicesField, ref MaxChoices, ref _maxChoicesBuffer, 2f);
            listing.DrawDescription("ToolkitPolls.Settings.MaxChoices.Description".TranslateSimple());

            (Rect coverLabel, Rect coverField) = listing.Split();
            UiHelper.Label(coverLabel, "ToolkitPolls.Settings.CoverDuration.Label".TranslateSimple());
            Widgets.TextFieldNumeric(coverField, ref CoverDuration, ref _coverDurationBuffer, 1);
            listing.DrawDescription("ToolkitPolls.Settings.CoverDuration.Description".TranslateSimple());

            (Rect durationLabel, Rect durationField) = listing.Split();
            UiHelper.Label(durationLabel, "ToolkitPolls.Settings.PollDuration.Label".TranslateSimple());
            Widgets.TextFieldNumeric(durationField, ref PollDuration, ref _pollDurationBuffer, 1);
            listing.DrawDescription("ToolkitPolls.Settings.PollDuration.Description".TranslateSimple());

            (Rect resultsLabel, Rect resultsField) = listing.Split();
            UiHelper.Label(resultsLabel, "ToolkitPolls.Settings.ResultsDuration.Label".TranslateSimple());
            Widgets.TextFieldNumeric(resultsField, ref ResultsDuration, ref _resultsDurationBuffer, 1);
            listing.DrawDescription("ToolkitPolls.Settings.ResultsDuration.Description".TranslateSimple());

            listing.CheckboxLabeled("ToolkitPolls.Settings.Colorless.Label".TranslateSimple(), ref Colorless);
            listing.DrawDescription("ToolkitPolls.Settings.Colorless.Description".TranslateSimple());

            listing.CheckboxLabeled("ToolkitPolls.Settings.PollBars.Label".TranslateSimple(), ref PollBars);
            listing.DrawDescription("ToolkitPolls.Settings.PollBars.Description".TranslateSimple());

            bool proxy = LargeText;
            listing.CheckboxLabeled("ToolkitPolls.Settings.LargeText.Label".TranslateSimple(), ref proxy);
            listing.DrawDescription("ToolkitPolls.Settings.LargeText.Description".TranslateSimple());

            if (proxy != LargeText)
            {
                LargeText = proxy;
                Choice.NotifyScaleChanged();
            }

            listing.CheckboxLabeled("ToolkitPolls.Settings.ChoicesInChat.Label".TranslateSimple(), ref ChoicesInChat);
            listing.DrawDescription("ToolkitPolls.Settings.ChoicesInChat.Description".TranslateSimple());


            listing.GroupHeader("ToolkitPolls.SettingGroups.VoteWeights.Label".TranslateSimple());
            listing.DrawDescription("ToolkitPolls.SettingGroups.VoteWeights.Description".TranslateSimple());
            listing.Gap(8f);

            (Rect subscriberLabel, Rect subscriberField) = listing.Split();
            UiHelper.Label(subscriberLabel, "ToolkitPolls.Settings.SubscriberWeight".TranslateSimple());
            Widgets.TextFieldNumeric(subscriberField, ref SubscriberWeight, ref _subscriberWeightBuffer, 1f);

            (Rect vipLabel, Rect vipField) = listing.Split();
            UiHelper.Label(vipLabel, "ToolkitPolls.Settings.VipWeight".TranslateSimple());
            Widgets.TextFieldNumeric(vipField, ref VipWeight, ref _vipWeightBuffer, 1f);

            (Rect founderLabel, Rect founderField) = listing.Split();
            UiHelper.Label(founderLabel, "ToolkitPolls.Settings.FounderWeight".TranslateSimple());
            Widgets.TextFieldNumeric(founderField, ref FounderWeight, ref _founderWeightBuffer, 1f);

            (Rect moderatorLabel, Rect moderatorField) = listing.Split();
            UiHelper.Label(moderatorLabel, "ToolkitPolls.Settings.ModeratorWeight".TranslateSimple());
            Widgets.TextFieldNumeric(moderatorField, ref ModeratorWeight, ref _moderatorWeightBuffer, 1f);

            listing.CheckboxLabeled("ToolkitPolls.Settings.TieredVotes.Label".TranslateSimple(), ref TieredVotes);
            listing.DrawDescription("ToolkitPolls.Settings.TieredVotes.Description".TranslateSimple());

            listing.End();
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Colorless, "colorless");
            Scribe_Values.Look(ref MaxChoices, "choices", 4);
            Scribe_Values.Look(ref CoverDuration, "coverDuration", 5);
            Scribe_Values.Look(ref PollDuration, "duration", 300);
            Scribe_Values.Look(ref ResultsDuration, "resultsDuration", 5);
            Scribe_Values.Look(ref PollBars, "bars", true);
            Scribe_Values.Look(ref TieredVotes, "tieredVotes", true);
            Scribe_Values.Look(ref LargeText, "largeText");
            Scribe_Values.Look(ref ChoicesInChat, "choicesInChat");

            Scribe_Values.Look(ref SubscriberWeight, "subscriberWeight", 1);
            Scribe_Values.Look(ref VipWeight, "vipWeight", 1);
            Scribe_Values.Look(ref FounderWeight, "founderWeight", 1);
            Scribe_Values.Look(ref ModeratorWeight, "moderatorWeight", 1);

            Scribe_Values.Look(ref PollDialogX, "xPosition", Mathf.Floor(UI.screenWidth - PollDialog.Width));
            Scribe_Values.Look(ref PollDialogY, "yPosition", Mathf.Floor(UI.screenHeight / 3f));
        }

        internal static GameFont GetTextScale() => LargeText ? GameFont.Medium : GameFont.Small;
    }
}
