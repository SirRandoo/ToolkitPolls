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

using SirRandoo.ToolkitPolls.Models;
using TwitchLib.Client.Models;

namespace SirRandoo.ToolkitPolls.Helpers
{
    public static class MessageHelper
    {
        public static UserType GetUserType(this ChatMessage message)
        {
            var container = UserType.None;

            foreach ((string key, string _) in message.Badges)
            {
                switch (key)
                {
                    case "admin":
                    case "broadcaster":
                    case "moderator":
                    case "global_mod":
                    case "staff":
                        container |= UserType.Moderator;
                        break;
                    case "subscriber":
                        container |= UserType.Subscriber;
                        break;
                    case "founder":
                        container |= UserType.Founder;
                        break;
                    case "vip":
                        container |= UserType.Vip;
                        break;
                }
            }

            return container;
        }
    }
}
