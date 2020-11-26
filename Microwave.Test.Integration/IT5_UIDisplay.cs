using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave;
using NSubstitute;
using System;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT5_UIDisplay
    {
        private IUserInterface _uut; //T
        private IButton _powerButton; //X
        private IButton _timeButton; //X
        private IButton _startCancelButton; //X
        private IDoor _door; //X
        private ILight _light; //X
        private ICookController _cookController; //x
        private IDisplay _display; //X

        private IOutput _fakeOutput; //S
        private IPowerTube _fakePowerTube; //S
        private ITimer _fakeTimer; //S

        [SetUp]
        public void Setup()
        {
            _fakeOutput = Substitute.For<IOutput>();
            _fakePowerTube = Substitute.For<IPowerTube>();
            _fakeTimer = Substitute.For<ITimer>();

            _display = new Display(_fakeOutput);
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);
            _cookController = new CookController(_fakeTimer, _display, _fakePowerTube);

            _uut = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookController);

            //Finishes the double association with interfaces
            _cookController.UI = _uut;
        }

        [TestCase(1)]
        [TestCase(7)]
        [TestCase(14)]
        public void Display_ShowPower_50_250_700_IsCorrect(int amountOfPresses)
        {
            for(int i = 0; i < amountOfPresses; i++)
            {
                _powerButton.Press();
            }

            _fakeOutput.Received(1).OutputLine($"Display shows: {amountOfPresses * 50} W");

            //Output receives call every time button is pressed, 
            //this ensures it doesn't show power higher than amount of times pressed
            _fakeOutput.DidNotReceive().OutputLine($"Display shows: {(1 + amountOfPresses) * 50} W");
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void Display_ShowTime_1_5_10_IsCorrect(int amountOfPresses)
        {
            _powerButton.Press();
            for (int i = 0; i < amountOfPresses; i++)
            {
                _timeButton.Press();
            }

            _fakeOutput.Received(1).OutputLine($"Display shows: {amountOfPresses:D2}:{0:D2}");

            //Output receives call every time button is pressed, 
            //this ensures it doesn't show time higher than amount of times pressed
            _fakeOutput.DidNotReceive().OutputLine($"Display shows: {(1+amountOfPresses):D2}:{0:D2}");
        }

        [Test]
        public void Display_Clear_IsCorrect()
        {
            _powerButton.Press();
            _startCancelButton.Press();

            _fakeOutput.Received(1).OutputLine($"Display cleared");
        }
    }
}
