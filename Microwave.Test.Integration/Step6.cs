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
    public class Step6
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
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();
            _light = Substitute.For<ILight>();
            _output = Substitute.For<IOutput>();
            _timer = Substitute.For<ITimer>(); //Stubbed to be able to raise events when needed

            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
            _cookController.UI = _userInterface;
        }

        [Test]
        public void CookingStarted_OnPowerPressedActivatedOneTime_PowerLevelIsSetTo50W()
        {
            //UI state == ready
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (50)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME
            _userInterface.OnStartCancelPressed(null, null); //UI state == COOKING

            var power = 50;

            _output.Received(1).OutputLine($"PowerTube works with {power} W");
        }

        [Test]
        public void CookingStarted_OnPowerPressedActivatedThreeTimes_PowerLevelIsSetTo150W()
        {
            //UI state == ready
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (50)
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (100)
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (150)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME
            _userInterface.OnStartCancelPressed(null, null); //UI state == COOKING

            var power = 150;

            _output.Received(1).OutputLine($"PowerTube works with {power} W");
        }


        [Test]
        public void CookingStarted_OmTimePressedActivatedOneTime_DisplayShowsOneMinut()
        {
            //UI state == ready
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (50)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME (1 minute)
            _userInterface.OnStartCancelPressed(null, null); //UI state == COOKING

            _output.Received(1).OutputLine($"Display shows: 01:00");
        }

        [Test]
        public void CookingStarted_OnTimePressedActivatedTwoTimes_DisplayShowsTwoMinutes()
        {
            //UI state == ready
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (50)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME (1 minute)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME (2 minutes)
            _userInterface.OnStartCancelPressed(null, null); //UI state == COOKING

            _output.Received(1).OutputLine($"Display shows: 02:00");
        }

        [Test]
        public void CookingStarted_OnStartCancelPressedActivatedWhileCookingIsStarted_PowerTubeIsTurnedOff()
        {
            StartCookingAndClearOutput();
            _userInterface.OnStartCancelPressed(null, null);

            _output.Received(1).OutputLine("PowerTube turned off");
            
        }

        [Test]
        public void CookingStarted_OnStartCancelPressedActivatedWhileCookingIsStarted_DisplayIsCleared()
        {
            StartCookingAndClearOutput();
            _userInterface.OnStartCancelPressed(null, null);

            _output.Received(1).OutputLine("Display cleared");
        }

        //This test might have to be placed at another point. 
        [Test]
        public void CookingStarted_TimeSetToTwoMinutes_TimerStartedWith60Seconds()
        {
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (50)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME (1 minute)
            _userInterface.OnStartCancelPressed(null, null); //UI state == COOKING

            _timer.Received().Start(60);
        }

        [Test]
        public void CookingStarted_TimeSetToTwoMinutes_TimerStartedWith120Seconds()
        {
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (50)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME (1 minute)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME (2 minutes)
            _userInterface.OnStartCancelPressed(null, null); //UI state == COOKING

            _timer.Received().Start(120);
        }

        /// <summary>
        /// Used to start a cooking. This results in _output receiving a number of calls.
        /// Because we assert on how many times the _output object has received calls, we clear this counter.
        /// </summary>
        private void StartCookingAndClearOutput()
        {
            _userInterface.OnPowerPressed(null, null); //UI state == SETPOWER, sets power to the first powerlevel (50)
            _userInterface.OnTimePressed(null, null); //UI state == SETTIME
            _userInterface.OnStartCancelPressed(null, null); //UI state == COOKING
            _output.ClearReceivedCalls();
        }
    }
}