using System;
using System.Collections.Generic;
using System.Linq;
using BowlingGame.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BowlingGame
{
    public class Game
    {
        private int TotalScore { get; set; } = 0;
        private int CurrentThrowBallIndex { get; set; } = 0;
        private Dictionary<int, int> ThrowBallResults { get; set; } = new Dictionary<int, int>();

        public void Roll(int pins)
        {
            CurrentThrowBallIndex++;
            ThrowBallResults.Add(CurrentThrowBallIndex, pins);
            TotalScore = TotalScore + pins;

            // check for spare bonus
            if (CurrentThrowBallIndex >= 3 && CurrentThrowBallIndex % 2 == 1)
            {
                var previousFrameResult = ThrowBallResults
                    .Where(kv => kv.Key < CurrentThrowBallIndex)
                    .OrderByDescending(kv => kv.Key)
                    .Take(2)
                    .Select(kv => kv.Value)
                    .Sum();

                if (previousFrameResult == 10)
                    TotalScore = TotalScore + pins;
            }
        }

        public int GetScore()
        {
            return TotalScore;
        }
    }

    [TestFixture]
    public class Game_should : ReportingTest<Game_should>
    {
        [Test]
        public void HaveZeroScore_BeforeAnyRolls()
        {
            new Game()
                .GetScore()
                .Should().Be(0);
        }

        [Test]
        public void CalculateScore_OnlySimpleRolls()
        {
            var game = new Game();
        
            game.Roll(4);
            game.Roll(4);
        
            game
                .GetScore()
                .Should().Be(8);
        }

        [Test]
        public void CalculateScore_SimpleRollsAndSpares()
        {
            var game = new Game();

            // #1 frame - simple roll
            game.Roll(4);
            game.Roll(4);

            // #2 frame - spare 
            game.Roll(2);
            game.Roll(8);

            // #3 frame - simple roll
            game.Roll(3);
            game.Roll(2);

            game
                .GetScore()
                .Should().Be(8+10+3+5);
        }
    }
}
