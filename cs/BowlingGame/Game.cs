using System;
using BowlingGame.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BowlingGame
{
    public class Game
    {
        private int TotalScore { get; set; } = 0;
        
        public void Roll(int pins)
        {
            TotalScore = TotalScore + pins;
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
