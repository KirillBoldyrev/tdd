﻿using System;
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
        private int CurrentFrameIndex { get; set; } = 1;
        private Dictionary<int, List<int>> ThrowBallResults { get; set; } = new Dictionary<int, List<int>>();

        public void Roll(int pins)
        {
            if (IsNewFrameStarted())
            {
                CurrentFrameIndex++; // current frame ended, go to next one
            }

            if (!ThrowBallResults.ContainsKey(CurrentFrameIndex))
            {
                ThrowBallResults.Add(CurrentFrameIndex, new List<int>());
            }

            ThrowBallResults[CurrentFrameIndex].Add(pins);
            TotalScore = TotalScore + pins;

            // check for spare bonus
            if (IsSpare())
            {
                var previousFrameResult = ThrowBallResults[CurrentFrameIndex - 1]
                    .Sum();

                if (previousFrameResult == 10)
                    TotalScore = TotalScore + pins;
            }

            // check for strike bonus
            if (IsSingleStrike())
            {
                TotalScore = TotalScore + pins;

                // check for multiple-strike-sequence bonus
                if (IsMultipleStrike())
                {
                    TotalScore = TotalScore + pins;
                }
            }
        }

        private bool IsNewFrameStarted()
        {
            return ThrowBallResults.ContainsKey(CurrentFrameIndex) &&
                            (ThrowBallResults[CurrentFrameIndex].Count == 2 ||
                             ThrowBallResults[CurrentFrameIndex].FirstOrDefault() == 10
                            );
        }

        private bool IsMultipleStrike()
        {
            return CurrentFrameIndex > 2 &&
                                ThrowBallResults[CurrentFrameIndex - 2].Count == 1 /* prev-previous frame was striked */ &&
                                ThrowBallResults[CurrentFrameIndex].Count == 1 /* take only first throw in current frame */;
        }

        private bool IsSingleStrike()
        {
            return CurrentFrameIndex > 1 &&
                            ThrowBallResults[CurrentFrameIndex - 1].Count == 1 /* previous frame was striked */;
        }

        private bool IsSpare()
        {
            return CurrentFrameIndex > 1 &&
                            ThrowBallResults[CurrentFrameIndex].Count == 1 /* first throw in current frame */  &&
                            ThrowBallResults[CurrentFrameIndex - 1].Count == 2 /* previous frame wasn't striked */;
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

            // #1 frame - spare 
            game.Roll(2);
            game.Roll(8);

            // #2 frame - simple roll
            game.Roll(3);
            game.Roll(2);

            game
                .GetScore()
                .Should().Be((2+8)+3+(3+2));
        }

        [Test]
        public void CalculateScore_SimpleRollsAndStrikes()
        {
            var game = new Game();

            // #1 frame - strike 
            game.Roll(10);

            // #2 frame - simple roll
            game.Roll(1);
            game.Roll(0);

            game
                .GetScore()
                .Should().Be(10 + (1 + 0) + (1 + 0));
        }

        [Test]
        public void CalculateScore_SimpleRollsAndDoubleStrike()
        {
            var game = new Game();

            // #1 frame - strike 
            game.Roll(10);

            // #2 frame - strike 
            game.Roll(10);

            // #3 frame - simple roll
            game.Roll(1);
            game.Roll(5);

            game
                .GetScore()
                .Should().Be(10 + (10 + 1) + 10 + (1 + 5) + (1 + 5));
        }
    }
}
