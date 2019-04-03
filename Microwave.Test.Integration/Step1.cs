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

        [TestCase(1100, 10000, 1)]
        [TestCase(2100, 10000, 2)]
        public void StartCooking_DisplayUpdatedOnTimerTick(int sleepTime, int cookTime, int numberOfEvents)
        {
            var power = 50;
            _cookController.StartCooking(power, cookTime);
            Thread.Sleep(sleepTime);

            _display.Received(numberOfEvents).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void CookingIsDone_PowerTubeTurnedOff()
        {
            var power = 50;
            var cookTime = 1000;
            _cookController.StartCooking(power, cookTime);
            Thread.Sleep(cookTime + 100);

            _powerTube.Received(1).TurnOff();
        }
    }
}