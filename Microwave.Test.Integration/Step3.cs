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

        [TestCase(1)] //boundary value
        [TestCase(350)]
        [TestCase(700)] //boundary value
        public void StartCooking_PowerValueIsValid_PowerTubeIsTurnedOn(int power)
        {
            var cookTime = 10000;
            _cookController.StartCooking(power, cookTime);

            _output.Received(1).OutputLine($"PowerTube works with {power} W");
        }
    }
}