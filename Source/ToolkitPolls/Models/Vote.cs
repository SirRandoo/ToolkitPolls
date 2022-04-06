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

namespace SirRandoo.ToolkitPolls.Models
{
    [Flags] public enum UserTypes : short { None = 0, Subscriber = 1, Vip = 2, Founder = 4, Moderator = 8 }

    public class Vote
    {
        public int Choice { get; set; }
        public string Viewer { get; set; }
        public UserTypes UserTypes { get; set; }

        public int GetTotalVotes() => PollSettings.TieredVotes ? GetTotalVotesTiered() : GetTotalVotesAdditive();

        private int GetTotalVotesAdditive()
        {
            if (UserTypes == UserTypes.None)
            {
                return 1;
            }

            var count = 0;

            if (UserTypes.HasFlag(UserTypes.Founder))
            {
                count += PollSettings.FounderWeight;
            }

            if (UserTypes.HasFlag(UserTypes.Vip))
            {
                count += PollSettings.VipWeight;
            }

            if (UserTypes.HasFlag(UserTypes.Subscriber))
            {
                count += PollSettings.SubscriberWeight;
            }

            if (UserTypes.HasFlag(UserTypes.Moderator))
            {
                count += PollSettings.ModeratorWeight;
            }

            return count;
        }

        private int GetTotalVotesTiered()
        {
            if (UserTypes.HasFlag(UserTypes.Moderator))
            {
                return PollSettings.ModeratorWeight;
            }

            if (UserTypes.HasFlag(UserTypes.Vip))
            {
                return PollSettings.VipWeight;
            }

            if (UserTypes.HasFlag(UserTypes.Founder))
            {
                return PollSettings.FounderWeight;
            }

            return UserTypes.HasFlag(UserTypes.Subscriber) ? PollSettings.SubscriberWeight : 1;
        }
    }
}
