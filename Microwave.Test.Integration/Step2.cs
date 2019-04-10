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
    public class Step2
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

        [Test]
        public void StartCooking_CookingIsStarted_TimerIsStarted()
        {
            var waitHandle = new ManualResetEvent(false);
            _timer.TimerTick += (sender, args) => waitHandle.Set();

            var power = 50;
            var cookTime = 1000;
            _cookController.StartCooking(power, cookTime);

            //Block max 1100ms. Assert a tick has happended
            Assert.That(waitHandle.WaitOne(cookTime + 100), Is.True);
        }

        [Test]
        public void Stop_CookingIsStopped_TimerIsStopped()
        {
            var waitHandle = new ManualResetEvent(false);
            _timer.TimerTick += (sender, args) => waitHandle.Set();

            var power = 50;
            var cookTime = 1000;
            _cookController.StartCooking(power, cookTime);
            _cookController.Stop(); //stop cooking --> stop timer

            //Block max 1100ms. Assert a tick has NOT happended
            Assert.That(waitHandle.WaitOne(cookTime + 100), Is.False);
        }
    }
}