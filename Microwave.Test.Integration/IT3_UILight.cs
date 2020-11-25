using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT3_UILight
    {
        private IUserInterface _uut; //T
        private IButton _powerButton; //X
        private IButton _timeButton; //X
        private IButton _startCancelButton; //X
        private IDoor _door; //X
        private ILight _light; //X

        private ICookController _fakeCookController; //S
        private IDisplay _fakeDisplay; //S
        private IOutput _fakeOutput; //S

        [SetUp]
        public void Setup()
        {
            _fakeCookController = Substitute.For<ICookController>();
            _fakeDisplay = Substitute.For<IDisplay>();
            _fakeOutput = Substitute.For<IOutput>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);

            _uut = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _fakeDisplay,
                _light,
                _fakeCookController);
        }

        #region Light
        [Test]
        public void Light_DoorOpen_TurnOn_IsCorrect()
        {
            _door.Open();

            _fakeOutput.Received(1).OutputLine("Light is turned on");
        }

        [Test]
        public void Light_DoorClose_TurnOff_IsCorrect()
        {
            _door.Open();

            _door.Close();

            _fakeOutput.Received(1).OutputLine("Light is turned off");
        }

        [Test]
        public void Light_Cooking_TurnOn_TurnOff_IsCorrect()
        {
            //Power button is pressed, changing state to SETPOWER
            _powerButton.Press();
            //Time button is pressed, changing state to SETTIME
            _timeButton.Press();
            //Start/Cancel butoon is pressed, changing state to COOKING
            _startCancelButton.Press();
            //Start/Cancel butoon is pressed again to stop, changing state to READY
            _startCancelButton.Press();
            //Door is opened, changing state to DOOROPEN
            _door.Open();
            //Door is closed
            _door.Close();

            //Assert light is turned on/off the correct amount of times
            _fakeOutput.Received(2).OutputLine("Light is turned on");
            _fakeOutput.Received(2).OutputLine("Light is turned off");
        }
        #endregion
    }
}
