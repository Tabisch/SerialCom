using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using SharpDX.XInput;

namespace SerialCom
{
    class ControllerReader
    {

        Timer timer;
        Controller controller;
        State state;

        Dictionary<GamepadButtonFlags, bool> prevButtonState;
        
        public ControllerReader()
        {
            controller = new Controller(UserIndex.One);

            Setup();

            controller.GetState(out state);

            timer = new Timer(10);

            timer.Elapsed += Update;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        void Setup()
        {
            prevButtonState = new Dictionary<GamepadButtonFlags, bool>();

            prevButtonState.Add(GamepadButtonFlags.A, false);
            prevButtonState.Add(GamepadButtonFlags.B, false);
            prevButtonState.Add(GamepadButtonFlags.X, false);
            prevButtonState.Add(GamepadButtonFlags.Y, false);
            prevButtonState.Add(GamepadButtonFlags.DPadUp, false);
            prevButtonState.Add(GamepadButtonFlags.DPadDown, false);
            prevButtonState.Add(GamepadButtonFlags.DPadLeft, false);
            prevButtonState.Add(GamepadButtonFlags.DPadRight, false);
            prevButtonState.Add(GamepadButtonFlags.Start, false);
            prevButtonState.Add(GamepadButtonFlags.Back, false);
            prevButtonState.Add(GamepadButtonFlags.LeftShoulder, false);
            prevButtonState.Add(GamepadButtonFlags.LeftThumb, false);
            prevButtonState.Add(GamepadButtonFlags.RightShoulder, false);
            prevButtonState.Add(GamepadButtonFlags.RightThumb, false);
        }

        void Update(Object source, ElapsedEventArgs e)
        {
            controller.GetState(out state);

            updateButtons();
            //updateSticks();
            //updateTriggers();
        }

        void updateButton(short selection, GamepadButtonFlags gBF)
        {
            if (state.Gamepad.Buttons.HasFlag(gBF) != prevButtonState[gBF])
            {
                Serial_Updater.sendSerial(selection,Convert.ToInt16(state.Gamepad.Buttons.HasFlag(gBF)));
                prevButtonState[gBF] = !prevButtonState[gBF];
            }
        }

        void updateButtons()
        {
            updateButton(0,GamepadButtonFlags.A);
            updateButton(1, GamepadButtonFlags.B);
            updateButton(2, GamepadButtonFlags.X);
            updateButton(3, GamepadButtonFlags.Y);
            updateButton(4, GamepadButtonFlags.DPadUp);
            updateButton(5, GamepadButtonFlags.DPadDown);
            updateButton(6, GamepadButtonFlags.DPadLeft);
            updateButton(7, GamepadButtonFlags.DPadRight);
            updateButton(8, GamepadButtonFlags.Start);
            updateButton(9, GamepadButtonFlags.Back);
            updateButton(10, GamepadButtonFlags.LeftShoulder);
            updateButton(11, GamepadButtonFlags.LeftThumb);
            updateButton(12, GamepadButtonFlags.RightShoulder);
            updateButton(13, GamepadButtonFlags.RightThumb);

        }

        void updateStick(short selection, short val)
        {
            Serial_Updater.sendSerial(selection, val);
        }

        void updateSticks()
        {
            updateStick(14, state.Gamepad.LeftThumbX);
            updateStick(15, state.Gamepad.LeftThumbY);
            updateStick(16, state.Gamepad.RightThumbX);
            updateStick(17, state.Gamepad.RightThumbY);
        }

        void updateTrigger(short selection, short val)
        {
            Serial_Updater.sendSerial(selection, val);
        }

        void updateTriggers()
        {
            updateTrigger(18,(short)state.Gamepad.LeftTrigger);
            updateTrigger(19, (short)state.Gamepad.RightTrigger);
        }
    }
}
