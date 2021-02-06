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

using System.Diagnostics;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitPolls.Helpers
{
    public static class LogHelper
    {
        private const string ErrorColor = "#ff768e";
        private const string WarnColor = "#ff6b00";

        public static void Message(string message)
        {
            Log.Message($"ToolkitPolls :: {message}");
        }

        public static void Message(string message, Color color)
        {
            Message(message.ColorTagged(color));
        }

        public static void Message(string level, string message)
        {
            Log.Message($"{level.ToUpperInvariant()} ToolkitPolls :: {message}");
        }

        public static void Message(string level, string message, Color color)
        {
            Log.Message($"{level.ToUpperInvariant()} ToolkitPolls :: {message}".ColorTagged(color));
        }

        public static void Message(string level, string message, string colorHex)
        {
            Log.Message($"{level.ToUpperInvariant()} ToolkitPolls :: {message}".ColorTagged(colorHex));
        }

        public static void Info(string message)
        {
            Message("INFO", message);
        }

        public static void Warn(string message)
        {
            Message("WARN", message, WarnColor);
        }

        public static void Error(string message)
        {
            Message("ERROR", message, ErrorColor);
        }

        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            Message("DEBUG", message, ColorLibrary.LightPink);
        }
    }
}
