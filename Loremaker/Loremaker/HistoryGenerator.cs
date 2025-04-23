using System;
using System.Collections.Generic;
using System.Linq;
using Archigen;
using Loremaker.Data;
using Loremaker.Names;
using Stateless;
using Syllabore;

namespace Loremaker
{
    public enum WarState
    {
        Inactive,
        Start,
        Battle,
        End
    }

    public enum Action
    {
        Next
    }

    public class HistoryGenerator : IGenerator<Codex>
    {
        private readonly Random _random;
        private readonly DefaultNameGenerator _nameGenerator;
        private readonly FactionGenerator _factionGenerator;
        private readonly PersonGenerator _personGenerator;
        private Codex _inProgressCodex;
        private int _currentYear;

        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public HistoryGenerator(int startYear, int endYear)
        {
            _random = new Random();
            _nameGenerator = new DefaultNameGenerator();
            _factionGenerator = new FactionGenerator();
            _personGenerator = new PersonGenerator();

            _inProgressCodex = null; // show it's purposely null
            _currentYear = -1;

            this.StartYear = startYear;
            this.EndYear = endYear;
        }

        private void GenerateStartEvent()
        {
            // Generate the "Start of War" event
            var warEvent = new Event(_currentYear++, "Start of War");
            warEvent.EventType = "start-of-war";
            warEvent.EventClassification = "major";

            // Generate two factions and register in codex so they get IDs
            var attackingFaction = _factionGenerator.Next();
            var defendingFaction = _factionGenerator.Next();
            _inProgressCodex.Register(attackingFaction);
            _inProgressCodex.Register(defendingFaction);

            // Generate two leaders and register in codex too
            var attackingLeader = _personGenerator.Next();
            attackingLeader.Properties["physical-appearance"] = "feminine";
            attackingLeader.Properties["personality-hints"] = "clever, strategic, passionate";
            attackingLeader.Properties["thematic-hints"] = "steel, apples, the sky";
            attackingLeader.Properties["color-hints"] = Things.GetDefaultColorGenerator().Next();
            _inProgressCodex.Register(attackingLeader);

            var defendingLeader = _personGenerator.Next();
            defendingLeader.Properties["physical-appearance"] = "masculine";
            defendingLeader.Properties["personality-hints"] = "talkative, thoughtful, passionate";
            defendingLeader.Properties["thematic-hints"] = "grass, sailing, cabins";
            defendingLeader.Properties["color-hints"] = Things.GetDefaultColorGenerator().Next();
            _inProgressCodex.Register(defendingLeader);

            // Assign factions to the people and event
            attackingLeader.FactionId = attackingFaction.Id.ToString();
            defendingLeader.FactionId = defendingFaction.Id.ToString();
            warEvent["attacking-faction-id"] = attackingFaction.Id.ToString();
            warEvent["defending-faction-id"] = defendingFaction.Id.ToString();

            // Rename the event to reflect the faction names
            warEvent.Name = $"The {attackingFaction.Name}-{defendingFaction.Name} War";
            warEvent.Description = $"Led by {attackingLeader.GivenName} {attackingLeader.FamilyName}, {attackingFaction.DecoratedName} began to wage war on the {defendingFaction.DecoratedName}.";

            // Register the event in the Codex
            _inProgressCodex.Register(warEvent);
        }

        private void GenerateTransitionEvent()
        {
            // Generate a battle event
            var battleName = "Battle of " + this._nameGenerator.Next();
            var battleEvent = new Event(_currentYear++, battleName);
            battleEvent.EventType = "battle";
            battleEvent.EventClassification = "minor";

            _inProgressCodex.Register(battleEvent);
        }

        private void GenerateEndEvent()
        {
            // Generate the "End of War" event
            var endOfWarEvent = new Event(_currentYear++, "End of War");
            endOfWarEvent.EventType = "end-of-war";
            endOfWarEvent.EventClassification = "major";

            _inProgressCodex.Register(endOfWarEvent);
        }

        public Codex Next()
        {
            _inProgressCodex = new Codex();
            _currentYear = this.StartYear + this._random.Next(this.EndYear - this.StartYear);

            var battleCount = 0;
            var maxBattleCount = _random.Next(3, 5);
            var simulator = new StateMachine<WarState, Action>(WarState.Inactive);

            simulator.Configure(WarState.Inactive)
                .Permit(Action.Next, WarState.Start);

            simulator.Configure(WarState.Start)
                .OnEntry(GenerateStartEvent)
                .Permit(Action.Next, WarState.Battle);

            simulator.Configure(WarState.Battle)
                .OnEntry(GenerateTransitionEvent)
                .PermitDynamic(Action.Next, () => battleCount++ >= maxBattleCount ? WarState.End : WarState.Battle);

            simulator.Configure(WarState.End)
                .OnEntry(GenerateEndEvent);

            while (simulator.CanFire(Action.Next))
            {
                simulator.Fire(Action.Next);
            }

            // TODO distribute years

            var result = _inProgressCodex;
            _inProgressCodex = null;
            _currentYear = -1;
            return result;
        }
    }

}
