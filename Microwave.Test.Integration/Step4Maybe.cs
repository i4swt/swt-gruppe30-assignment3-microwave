﻿using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step4Maybe
    {
        private Timer _timer;
        private IOutput _output;
        private Display _display;
        private PowerTube _powerTube;
        private CookController _cookController;
        private IUserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _timer = new Timer();
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            _userInterface = Substitute.For<IUserInterface>();
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
        }

        [TestCase(1100, 10000, 1, "Display shows: 00:09")] //buffer because our delay isn't precise
        [TestCase(2100, 10000, 1, "Display shows: 00:08")]
        public void OnTimerTick_RemainingTimeIsLogged(int sleepTime, int cookTime, int numberOfEvents, string expectedOutput)
        {
            _cookController.StartCooking(50, cookTime);
            Thread.Sleep(sleepTime);

            _output.Received(numberOfEvents).OutputLine(expectedOutput);
        }

        [Test]
        public void OnTimerExpired_RemainingTimeIsLogged()
        {
            var startTime = 2000;

            _cookController.StartCooking(50, startTime);
            Thread.Sleep(startTime + 1100);

            _output.Received(1).OutputLine("PowerTube turned off");
        }
    }
}