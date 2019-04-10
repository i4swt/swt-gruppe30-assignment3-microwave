using System;
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
    public class Step10_12
    {
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;
        private ILight _light;
        private ITimer _timer;
        private IOutput _output;
        private Display _display;
        private PowerTube _powerTube;
        private CookController _cookController;
        private UserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();

            _output = Substitute.For<IOutput>();
            _light = new Light(_output);
            _timer = new Timer(); //Stubbed to be able to raise events when needed

            _display = new Display(_output);
            _powerTube = new PowerTube(_output);

            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light,
                _cookController);
            _cookController.UI = _userInterface;
        }


        [Test]
        public void Open_WhenDoorOpens_OutputLightTurnsOn()
        {
            _door.Open();
            _output.Received().OutputLine("Light is turned on");
        }

        [Test]
        public void Close_WhenDoorCloses_OutputLightTurnedOff()
        {

            //Set state to validate that door close works
            _door.Open();
            _output.ClearReceivedCalls();

            //Act and assert
            _door.Close();
            _output.Received().OutputLine("Light is turned off");
        }
    }
}