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
    public class Step9
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
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
            _cookController.UI = _userInterface;
        }

        [Test]
        public void Timer_StartCookingDesiredAmountOfTime()
        {
            _userInterface.OnPowerPressed(null, null); 
            _userInterface.OnTimePressed(null, null); 
            _userInterface.OnStartCancelPressed(null, null);
            //Sleep 10 seconds. 
            Thread.Sleep(10000);
            //Expected cook time 1 minut. 
            for (int x = 59; x > 50; x--)
            {
                _output.Received(1).OutputLine($"Display shows: 00:{x}");
            }

        }

        [Test]
        public void Timer_StartCookingDesiredAmountOfTime_NotExpiredBeforeTimerIsDone()
        {
            _userInterface.OnPowerPressed(null, null);
            _userInterface.OnTimePressed(null, null);
            _userInterface.OnStartCancelPressed(null, null);
            //Sleep 10 seconds. 
            _output.ClearReceivedCalls();
            Thread.Sleep(3000);
            //Expected cook time 1 minut. 
            _output.DidNotReceive().OutputLine($"PowerTube turned off");
        }

        [Test]
        public void Timer_StartCookingDesiredAmountOfTime_TimerExpiredAfter61Seconds()
        {
            _userInterface.OnPowerPressed(null, null);
            _userInterface.OnTimePressed(null, null);
            _userInterface.OnStartCancelPressed(null, null);
            //Sleep 10 seconds.
            _output.ClearReceivedCalls();
            Thread.Sleep(61000);
            //Expected cook time 1 minut. 
            _output.Received(1).OutputLine($"PowerTube turned off");
        }
    }
}