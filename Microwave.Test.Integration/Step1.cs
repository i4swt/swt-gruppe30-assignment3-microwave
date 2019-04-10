using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step1
    {
        private Timer _timer;
        private IDisplay _display;
        private IPowerTube _powerTube;
        private CookController _cookController;
        private IUserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _timer = new Timer();
            _display = Substitute.For<IDisplay>();
            _powerTube = Substitute.For<IPowerTube>();
            _userInterface = Substitute.For<IUserInterface>();
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
        }

        [TestCase(1100, 10, 1)]
        [TestCase(2100, 10, 2)]
        [TestCase(5100, 3, 3)]
        public void OnTimerEvent_TimeIsRemaining_DisplayIsUpdatedEachSecond(int sleepTimeMilliSeconds, int cookTimeSeconds, int numberOfEvents)
        {
            var power = 50;
            _cookController.StartCooking(power, cookTimeSeconds);
            Thread.Sleep(sleepTimeMilliSeconds);

            _display.Received(numberOfEvents).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void OnTimerEvent_TimerExpires_PowerTubeIsTurnedOff()
        {
            var power = 50;
            var cookTimeSeconds = 1;
            _cookController.StartCooking(power, cookTimeSeconds);
            Thread.Sleep(1100);

            _powerTube.Received(1).TurnOff();
        }
    }
}