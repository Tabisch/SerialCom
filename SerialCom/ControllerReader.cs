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
        Dictionary<GamepadButtonFlags, short> buttonMapping;
        //Dictionary<>
        
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

            short counter = 0;

            foreach(GamepadButtonFlags temp in Enum.GetValues(typeof(GamepadButtonFlags)))
            {
                prevButtonState.Add(temp, false);
            }

            buttonMapping = new Dictionary<GamepadButtonFlags, short>();

            buttonMapping.Add(GamepadButtonFlags.A, 1);
            buttonMapping.Add(GamepadButtonFlags.B, 2);
            buttonMapping.Add(GamepadButtonFlags.X, 3);
            buttonMapping.Add(GamepadButtonFlags.Y, 4);
            buttonMapping.Add(GamepadButtonFlags.LeftShoulder, 5);
            buttonMapping.Add(GamepadButtonFlags.RightShoulder, 6);
            buttonMapping.Add(GamepadButtonFlags.Back, 7);
            buttonMapping.Add(GamepadButtonFlags.Start, 8);
            buttonMapping.Add(GamepadButtonFlags.LeftThumb, 9);
            buttonMapping.Add(GamepadButtonFlags.RightThumb, 10);
            buttonMapping.Add(GamepadButtonFlags.DPadUp, 11);
            buttonMapping.Add(GamepadButtonFlags.DPadDown, 12); 
            buttonMapping.Add(GamepadButtonFlags.DPadLeft, 13);
            buttonMapping.Add(GamepadButtonFlags.DPadRight, 14);
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
            foreach(KeyValuePair<GamepadButtonFlags, short> entry in buttonMapping)
            {
                updateButton(entry.Value, entry.Key);
            }
        }

        void updateStick(short selection, short val)
        {
            Serial_Updater.sendSerial(selection, val);
        }

        void updateSticks()
        {
            updateStick(14, state.Gamepad.LeftThumbX);
            updateStick(14, state.Gamepad.LeftThumbY);
            updateStick(15, state.Gamepad.RightThumbX);
            updateStick(15, state.Gamepad.RightThumbY);
        }

        void updateTrigger(short selection, short val)
        {
            Serial_Updater.sendSerial(selection, val);
        }

        void updateTriggers()
        {
            updateTrigger(16,(short)state.Gamepad.LeftTrigger);
            updateTrigger(17, (short)state.Gamepad.RightTrigger);
        }
    }
}
