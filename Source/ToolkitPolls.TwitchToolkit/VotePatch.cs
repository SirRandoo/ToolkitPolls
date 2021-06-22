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
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit;
using TwitchToolkit.Storytellers;
using TwitchToolkit.Votes;
using Verse;

namespace SirRandoo.ToolkitPolls.TwitchToolkit
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class VotePatch
    {
        private static readonly AccessTools.FieldRef<object, Dictionary<int, RaidStrategyDef>> AllRaidStrategies =
            AccessTools.FieldRefAccess<Dictionary<int, RaidStrategyDef>>(typeof(Vote_RaidStrategy), "allStrategies");

        private static readonly AccessTools.FieldRef<object, IncidentParms> RaidParams =
            AccessTools.FieldRefAccess<IncidentParms>(typeof(Vote_RaidStrategy), "parms");

        private static readonly AccessTools.FieldRef<object, IncidentWorker> RaidWorker =
            AccessTools.FieldRefAccess<IncidentWorker>(typeof(Vote_RaidStrategy), "worker");

        private static readonly AccessTools.FieldRef<object, string> StrategyTitle =
            AccessTools.FieldRefAccess<string>(typeof(Vote_RaidStrategy), "title");

        private static readonly AccessTools.FieldRef<object, StorytellerComp> RaidUristComp =
            AccessTools.FieldRefAccess<StorytellerComp>(typeof(Vote_RaidStrategy), "comp");

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(VoteHandler), "QueueVote");
        }

        public static bool Prefix(Vote vote)
        {
            switch (vote)
            {
                case Vote_ToryTalker toryTalker:
                    ProcessVote(toryTalker);
                    break;
                case Vote_HodlBot hodlBot:
                    ProcessVote(hodlBot);
                    break;
                case Vote_VotingIncident incident:
                    ProcessVote(incident);
                    break;
                case Vote_Mercurius mercurius:
                    ProcessVote(mercurius);
                    break;
                case Vote_Milasandra milasandra:
                    ProcessVote(milasandra);
                    break;
                case VoteIncidentDef def:
                    ProcessVote(def);
                    break;
                case Vote_RaidStrategy raidStrategy:
                    ProcessVote(raidStrategy);
                    break;
            }

            return false;
        }

        private static void ProcessVote(Vote_RaidStrategy raidStrategy)
        {
            var builder = new PollBuilder();

            foreach ((int key, RaidStrategyDef value) in AllRaidStrategies.Invoke(raidStrategy))
            {
                builder.WithChoice(
                    raidStrategy.VoteKeyLabel(key),
                    () =>
                    {
                        IncidentParms incidentParams = RaidParams.Invoke(raidStrategy);

                        if (incidentParams == null)
                        {
                            ConcludePoll();
                            return;
                        }

                        incidentParams.raidStrategy = value;
                        Ticker.FiringIncidents.Enqueue(
                            new FiringIncident(
                                RaidWorker.Invoke(raidStrategy)?.def,
                                RaidUristComp.Invoke(raidStrategy),
                                incidentParams
                            )
                        );
                        ConcludePoll();
                    }
                );
            }

            string title = StrategyTitle.Invoke(raidStrategy);
            if (!title.NullOrEmpty())
            {
                builder.WithTitle(title);
            }

            ToolkitPolls.SchedulePoll(builder.Build());
        }

        private static void ProcessVote([NotNull] Vote_VotingIncident incident)
        {
            var builder = new ToolkitPollBuilder(incident);

            foreach ((int key, VotingIncident value) in incident.incidents)
            {
                builder.WithChoice(
                    incident.VoteKeyLabel(key),
                    () =>
                    {
                        Ticker.IncidentHelpers.Enqueue(value.helper);
                        Current.Game.GetComponent<StoryTellerVoteTracker>()?.LogVoteIncident(value);
                        ConcludePoll();
                    }
                );
            }

            if (!incident.title.NullOrEmpty())
            {
                builder.WithTitle(incident.title);
            }

            ToolkitPolls.SchedulePoll(builder.Build());
        }

        private static void ProcessVote([NotNull] Vote_ToryTalker toryTalker)
        {
            var builder = new PollBuilder();

            foreach ((int key, VotingIncident value) in toryTalker.incidents)
            {
                builder.WithChoice(
                    toryTalker.VoteKeyLabel(key),
                    () =>
                    {
                        Ticker.IncidentHelpers.Enqueue(value.helper);
                        var component = Current.Game.GetComponent<StoryTellerVoteTracker>();
                        component?.LogVoteIncident(value);
                        component?.LogStorytellerCompVote(DefDatabase<StorytellerPack>.GetNamed("ToryTalker"));
                        ConcludePoll();
                    }
                );
            }

            if (!toryTalker.title.NullOrEmpty())
            {
                builder.WithTitle(toryTalker.title);
            }

            ToolkitPolls.SchedulePoll(builder.Build());
        }

        private static void ProcessVote([NotNull] Vote_HodlBot hodlBot)
        {
            var builder = new PollBuilder();

            foreach ((int key, VotingIncident value) in hodlBot.incidents)
            {
                builder.WithChoice(
                    hodlBot.VoteKeyLabel(key),
                    () =>
                    {
                        Ticker.IncidentHelpers.Enqueue(value.helper);
                        var component = Current.Game.GetComponent<StoryTellerVoteTracker>();
                        component?.LogVoteIncident(value);
                        component?.LogStorytellerCompVote(DefDatabase<StorytellerPack>.GetNamed("HodlBot"));
                        ConcludePoll();
                    }
                );
            }

            if (!hodlBot.title.NullOrEmpty())
            {
                builder.WithTitle(hodlBot.title);
            }

            ToolkitPolls.SchedulePoll(builder.Build());
        }

        private static void ProcessVote([NotNull] Vote_Milasandra milasandra)
        {
            var builder = new PollBuilder();

            foreach ((int key, IncidentDef value) in milasandra.incidents)
            {
                builder.WithChoice(
                    milasandra.VoteKeyLabel(key),
                    () =>
                    {
                        Current.Game.GetComponent<StoryTellerVoteTracker>()
                          ?.LogStorytellerCompVote(DefDatabase<StorytellerPack>.GetNamed("Milasandra"));
                        Ticker.FiringIncidents.Enqueue(new FiringIncident(value, milasandra.source, milasandra.parms));
                        Ticker.lastEvent = DateTime.Now;
                        Messages.Message(
                            new Message($"Chat votes for: {value.LabelCap}", MessageTypeDefOf.NeutralEvent)
                        );
                        ConcludePoll();
                    }
                );
            }

            if (!milasandra.title.NullOrEmpty())
            {
                builder.WithTitle(milasandra.title);
            }

            ToolkitPolls.SchedulePoll(builder.Build());
        }

        private static void ProcessVote([NotNull] Vote_Mercurius mercurius)
        {
            var builder = new PollBuilder();

            foreach ((int key, IncidentDef value) in mercurius.incidents)
            {
                builder.WithChoice(
                    mercurius.VoteKeyLabel(key),
                    () =>
                    {
                        Current.Game.GetComponent<StoryTellerVoteTracker>()
                          ?.LogStorytellerCompVote(DefDatabase<StorytellerPack>.GetNamed("Mercurius"));
                        Ticker.FiringIncidents.Enqueue(new FiringIncident(value, mercurius.source, mercurius.parms));
                        Ticker.lastEvent = DateTime.Now;
                        Messages.Message(
                            new Message($"Chat votes for: {value.LabelCap}", MessageTypeDefOf.NeutralEvent)
                        );
                        ConcludePoll();
                    }
                );
            }

            if (!mercurius.title.NullOrEmpty())
            {
                builder.WithTitle(mercurius.title);
            }

            ToolkitPolls.SchedulePoll(builder.Build());
        }

        private static void ProcessVote([NotNull] VoteIncidentDef def)
        {
            var builder = new PollBuilder();

            foreach ((int key, IncidentDef value) in def.incidents)
            {
                builder.WithChoice(
                    def.VoteKeyLabel(key),
                    () =>
                    {
                        Ticker.FiringIncidents.Enqueue(new FiringIncident(value, def.source, def.parms));
                        Ticker.lastEvent = DateTime.Now;
                        Messages.Message(
                            new Message($"Chat votes for: {value.LabelCap}", MessageTypeDefOf.NeutralEvent)
                        );
                        ConcludePoll();
                    }
                );
            }

            if (!def.title.NullOrEmpty())
            {
                builder.WithTitle(def.title);
            }

            ToolkitPolls.SchedulePoll(builder.Build());
        }

        private static void ConcludePoll()
        {
            VoteHandler.voteActive = false;
            VoteHandler.currentVote = null;
        }
    }
}
