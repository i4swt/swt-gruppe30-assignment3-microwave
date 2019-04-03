using System;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step3
    {
        private Timer _timer;
        private IOutput _output;
        private IDisplay _display;
        private PowerTube _powerTube;
        private CookController _cookController;
        private IUserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _timer = new Timer();
            _display = Substitute.For<IDisplay>();
            _powerTube = new PowerTube(_output);
            _userInterface = Substitute.For<IUserInterface>();
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void StartCookingWithValidPowerValue_TurnsOnPowerTube_WritesOutPut(int power)
        {
            var cookTime = 10000;
            _cookController.StartCooking(power, cookTime);

            _output.Received(1).OutputLine($"PowerTube works with {power} %");
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(101)]
        public void StartCookingWithInvalidPowerValue_ThrowsArgumentOutOfRangeException(int power)
        {
            var cookTime = 10000;

            Assert.That(() => _cookController.StartCooking(power, cookTime),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void StartCookingWhileCookingInProgress_ThrowsApplicationException()
        {
            var power = 50;
            var cookTime = 10000;
            _cookController.StartCooking(power, cookTime);

            //Start cooking a second time
            Assert.That(() => _cookController.StartCooking(power, cookTime),
                Throws.TypeOf<ApplicationException>());
        }
    }
}