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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Steamworks;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls.Helpers
{
    public enum SortOrder { Ascending, Descending }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SettingsHelper
    {
        private static readonly FieldInfo SelectedModField = AccessTools.Field(typeof(Dialog_ModSettings), "selMod");

        private static readonly GameFont[] GameFonts = Enum.GetNames(typeof(GameFont))
           .Select(f => (GameFont) Enum.Parse(typeof(GameFont), f))
           .OrderByDescending(f => (int) f)
           .ToArray();

        public static bool DrawFieldButton(Rect canvas, string label)
        {
            var region = new Rect(canvas.x + canvas.width - canvas.height, canvas.y, canvas.height, canvas.height);
            Widgets.ButtonText(region, label, false);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

            if (!clicked)
            {
                return false;
            }

            GUIUtility.keyboardControl = 0;
            return true;
        }

        public static bool DrawFieldButton(Rect canvas, Texture2D icon)
        {
            var region = new Rect(canvas.x + canvas.width - canvas.height, canvas.y, canvas.height, canvas.height);
            Widgets.ButtonImage(region, icon);

            bool clicked = Mouse.IsOver(region) && Event.current.type == EventType.Used && Input.GetMouseButtonDown(0);

            if (!clicked)
            {
                return false;
            }

            GUIUtility.keyboardControl = 0;
            return true;
        }

        public static bool DrawClearButton(Rect canvas)
        {
            return DrawFieldButton(canvas, "×");
        }

        public static void DrawSortIndicator(Rect canvas, SortOrder order)
        {
            var region = new Rect(canvas.x + canvas.width - 12f, canvas.y + 4f, canvas.height - 8f, canvas.height - 8f);

            switch (order)
            {
                case SortOrder.Ascending:
                    GUI.DrawTexture(region, Textures.SortAscend);
                    return;
                case SortOrder.Descending:
                    GUI.DrawTexture(region, Textures.SortDescend);
                    return;
                default:
                    GUI.DrawTexture(region, Textures.QuestionMark);
                    return;
            }
        }

        public static bool DrawDoneButton(Rect canvas)
        {
            return DrawFieldButton(canvas, "✔");
        }

        public static bool WasLeftClicked(this Rect region)
        {
            return WasMouseButtonClicked(region, 0);
        }

        public static bool WasRightClicked(this Rect region)
        {
            return WasMouseButtonClicked(region, 1);
        }

        public static bool WasMouseButtonClicked(this Rect region, int mouseButton)
        {
            if (!Mouse.IsOver(region))
            {
                return false;
            }

            Event current = Event.current;
            bool was = current.button == mouseButton;

            switch (current.type)
            {
                case EventType.Used when was:
                case EventType.MouseDown when was:
                    current.Use();
                    return true;
                default:
                    return false;
            }
        }

        public static Rect ShiftLeft(this Rect region, float padding = 5f)
        {
            return new Rect(region.x - region.width - padding, region.y, region.width, region.height);
        }

        public static Rect ShiftRight(this Rect region, float padding = 5f)
        {
            return new Rect(region.x + region.width + padding, region.y, region.width, region.height);
        }

        public static bool IsRegionVisible(this Rect region, Rect scrollView, Vector2 scrollPos)
        {
            return (region.y >= scrollPos.y || region.y + region.height - 1f >= scrollPos.y)
                   && region.y <= scrollPos.y + scrollView.height;
        }

        public static void DrawColored(this Texture2D t, Rect region, Color color)
        {
            Color old = GUI.color;

            GUI.color = color;
            GUI.DrawTexture(region, t);
            GUI.color = old;
        }

        public static void DrawLabel(
            Rect region,
            string text,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont fontScale = GameFont.Small,
            bool vertical = false
        )
        {
            Text.Anchor = anchor;
            Text.Font = fontScale;

            if (vertical)
            {
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            Widgets.Label(region, text);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        public static bool DrawTabButton(
            Rect region,
            string text,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont fontScale = GameFont.Small,
            bool vertical = false,
            bool selected = false
        )
        {
            Text.Anchor = anchor;
            Text.Font = fontScale;

            if (vertical)
            {
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            GUI.color = selected ? new Color(0.46f, 0.49f, 0.5f) : new Color(0.21f, 0.23f, 0.24f);
            Widgets.DrawHighlight(region);
            GUI.color = Color.white;

            if (!selected && Mouse.IsOver(region))
            {
                Widgets.DrawLightHighlight(region);
            }

            Widgets.Label(region, text);
            bool pressed = Widgets.ButtonInvisible(region);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            return pressed;
        }

        public static void DrawColoredLabel(
            Rect region,
            string text,
            Color color,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont fontScale = GameFont.Small,
            bool vertical = false
        )
        {
            GUI.color = color;
            DrawLabel(region, text, anchor, fontScale, vertical);
            GUI.color = Color.white;
        }

        public static void DrawFittedLabel(
            Rect region,
            string text,
            TextAnchor anchor = TextAnchor.MiddleLeft,
            GameFont maxScale = GameFont.Small,
            bool vertical = false
        )
        {
            Text.Anchor = anchor;

            if (vertical)
            {
                region.y += region.width;
                GUIUtility.RotateAroundPivot(-90f, region.position);
            }

            var maxFontScale = (int) maxScale;
            foreach (GameFont f in GameFonts)
            {
                if ((int) f > maxFontScale)
                {
                    continue;
                }

                Text.Font = f;

                if (Text.CalcSize(text).x <= region.width)
                {
                    break;
                }
            }

            Widgets.Label(region, text);

            if (vertical)
            {
                GUI.matrix = Matrix4x4.identity;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        public static Tuple<Rect, Rect> ToForm(this Rect region, float factor = 0.8f)
        {
            var left = new Rect(region.x, region.y, region.width * factor - 2f, region.height);

            return new Tuple<Rect, Rect>(
                left.Rounded(),
                new Rect(left.x + left.width + 2f, left.y, region.width - left.width - 2f, left.height).Rounded()
            );
        }

        public static Tuple<Rect, Rect> GetForm(this Listing listing, float factor = 0.8f)
        {
            return listing.GetRect(Text.LineHeight).ToForm(factor);
        }

        public static string Tagged(this string s, string tag)
        {
            return $"<{tag}>{s}</{tag}>";
        }

        public static string ColorTagged(this string s, string hex)
        {
            if (!hex.StartsWith("#"))
            {
                hex = $"#{hex}";
            }

            return $@"<color=""{hex}"">{s}</color>";
        }

        public static string ColorTagged(this string s, Color color)
        {
            return ColorTagged(s, ColorUtility.ToHtmlStringRGB(color));
        }

        public static void TipRegion(this Rect region, string tooltip)
        {
            TooltipHandler.TipRegion(region, tooltip);
            Widgets.DrawHighlightIfMouseover(region);
        }

        public static void OpenSettingsMenuFor(Mod modInstance)
        {
            var settings = new Dialog_ModSettings();
            SelectedModField.SetValue(settings, modInstance);

            Find.WindowStack.Add(settings);
        }

        public static Rect TrimLeft(this Rect region, float amount)
        {
            return new Rect(region.x + amount, region.y, region.width - amount, region.height);
        }

        public static Rect WithWidth(this Rect region, float width)
        {
            return new Rect(region.x, region.y, width, region.height);
        }

        public static void DrawExperimentalNotice(this Listing listing)
        {
            listing.DrawDescription("ToolkitPolls.Experimental".TranslateSimple(), new Color(1f, 0.53f, 0.76f));
        }

        public static void DrawDescription(this Listing listing, string description, Color color)
        {
            GameFont fontCache = Text.Font;
            GUI.color = color;
            Text.Font = GameFont.Tiny;
            float height = Text.CalcHeight(description, listing.ColumnWidth * 0.7f);
            DrawLabel(
                listing.GetRect(height).TrimLeft(10f).WithWidth(listing.ColumnWidth * 0.7f).Rounded(),
                description,
                TextAnchor.UpperLeft,
                GameFont.Tiny
            );
            GUI.color = Color.white;
            Text.Font = fontCache;

            listing.Gap(6f);
        }

        public static void DrawDescription(this Listing listing, string description)
        {
            DrawDescription(listing, description, new Color(0.72f, 0.72f, 0.72f));
        }

        public static void DrawGroupHeader(this Listing listing, string heading, bool gapPrefix = true)
        {
            if (gapPrefix)
            {
                listing.Gap(Mathf.CeilToInt(Text.LineHeight * 1.25f));
            }

            DrawLabel(listing.GetRect(Text.LineHeight), heading, TextAnchor.LowerLeft, GameFont.Tiny);
            listing.GapLine(6f);
        }

        public static void DrawModGroupHeader(this Listing listing, string modName, ulong modId, bool gapPrefix = true)
        {
            if (gapPrefix)
            {
                listing.Gap(Mathf.CeilToInt(Text.LineHeight * 1.25f));
            }

            Rect lineRect = listing.GetRect(Text.LineHeight);
            DrawLabel(lineRect, modName, TextAnchor.LowerLeft, GameFont.Tiny);

            string modRequirementString = "ToolkitPolls.ModRequirement".Translate(modName);
            GUI.color = new Color(1f, 0.53f, 0.76f);

            Text.Font = GameFont.Tiny;
            float width = Text.CalcSize(modRequirementString).x;
            var modRequirementRect = new Rect(lineRect.x + lineRect.width - width, lineRect.y, width, Text.LineHeight);
            Text.Font = GameFont.Small;

            DrawLabel(lineRect, modRequirementString, TextAnchor.LowerRight, GameFont.Tiny);
            GUI.color = Color.white;

            Widgets.DrawHighlightIfMouseover(modRequirementRect);
            if (Widgets.ButtonInvisible(modRequirementRect))
            {
                SteamUtility.OpenWorkshopPage(new PublishedFileId_t(modId));
            }

            listing.GapLine(6f);
        }

        public static bool DrawCheckbox(Rect canvas, ref bool state)
        {
            bool proxy = state;
            Widgets.Checkbox(canvas.position, ref proxy, Mathf.Min(canvas.width, canvas.height), paintable: true);

            bool changed = proxy != state;
            state = proxy;
            return changed;
        }
    }
}
