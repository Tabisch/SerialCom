using System;
using System.Collections.Generic;
using System.Timers;
using SharpDX.XInput;

namespace SerialCom
{
    class ControllerReader
    {
        bool updateStickX = false;
        bool updateStickY = false;

        short stickUpdateMargin = 500;
        short deadZone = 5500;

        Timer timer;
        Controller controller;
        State state;

        Dictionary<GamepadButtonFlags, bool> prevButtonState;
        Dictionary<GamepadButtonFlags, short> buttonMapping;

        Dictionary<string, short> prevPosition;

        Dictionary<string, short> stickValStore;

        public ControllerReader()
        {
            controller = new Controller(UserIndex.Four);

            Setup();

            controller.GetState(out state);

            timer = new Timer(50);

            timer.Elapsed += Update;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        void Setup()
        {
            prevButtonState = new Dictionary<GamepadButtonFlags, bool>();

            foreach (GamepadButtonFlags temp in Enum.GetValues(typeof(GamepadButtonFlags)))
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

            prevPosition = new Dictionary<string, short>();

            prevPosition.Add("leftX", 0);
            prevPosition.Add("leftY", 0);
            prevPosition.Add("rightX", 0);
            prevPosition.Add("rightY", 0);
            prevPosition.Add("RightTrigger", 0);
            prevPosition.Add("LeftTrigger", 0);

            stickValStore = new Dictionary<string, short>();
            stickValStore.Add("leftX", 0);
            stickValStore.Add("leftY", 0);
            stickValStore.Add("rightX", 0);
            stickValStore.Add("rightY", 0);
        }

        void Update(Object source, ElapsedEventArgs e)
        {
            if (Serial_Updater.getDesynced())
            {
                return;
            }

            controller.GetState(out state);

            updateButtons();
            updateSticks();
            updateTriggers();
        }

        void updateButton(short selection, GamepadButtonFlags gBF)
        {
            if (state.Gamepad.Buttons.HasFlag(gBF) != prevButtonState[gBF])
            {
                Serial_Updater.sendSerial(selection, Convert.ToInt16(state.Gamepad.Buttons.HasFlag(gBF)), gBF.ToString());
                prevButtonState[gBF] = !prevButtonState[gBF];
            }
        }

        void updateButtons()
        {
            foreach (KeyValuePair<GamepadButtonFlags, short> entry in buttonMapping)
            {
                updateButton(entry.Value, entry.Key);
            }
        }

        void checkIfUpdateNeeded(short selection, short val, string name)
        {
            int maxUpper, maxLower;

            if (short.MaxValue - stickUpdateMargin < prevPosition[name])
            {
                maxUpper = short.MaxValue;
            }
            else
            {
                maxUpper = prevPosition[name] + stickUpdateMargin;
            }

            if (short.MinValue + stickUpdateMargin > prevPosition[name])
            {
                maxLower = short.MaxValue;
            }
            else
            {
                maxLower = prevPosition[name] - stickUpdateMargin;
            }

            if (val >= -deadZone && val <= deadZone)
            {
                if (prevPosition[name] == 0)
                {
                    return;
                }

                stickValStore[name] = 0;

                if (selection == 17)
                {
                    updateStickY = true;
                }
                else
                {
                    updateStickX = true;
                }

                return;
            }

            if (!(val >= maxLower && val <= maxUpper))
            {
                if(selection == 17)
                {
                    updateStickY = true;
                }
                else
                {
                    updateStickX = true;
                }

                stickValStore[name] = val;
            }
        }

        void updateStick(short selection, short val, string name)
        {
            Serial_Updater.sendSerial(selection, val, name);
            prevPosition[name] = stickValStore[name];
        }

        void updateSticks()
        {
            checkIfUpdateNeeded(17, state.Gamepad.LeftThumbX, "leftX");
            checkIfUpdateNeeded(17, state.Gamepad.LeftThumbY, "leftY");
            checkIfUpdateNeeded(18, state.Gamepad.RightThumbX, "rightX");
            checkIfUpdateNeeded(18, state.Gamepad.RightThumbY, "rightY");

            if(updateStickY)
            {
                updateStick(17, state.Gamepad.LeftThumbX, "leftX");
                updateStick(17, state.Gamepad.LeftThumbY, "leftY");

                updateStickY = false;
            }

            if(updateStickX)
            {
                updateStick(18, state.Gamepad.RightThumbX, "rightX");
                updateStick(18, state.Gamepad.RightThumbY, "rightY");

                updateStickX = false;
            }
        }

        void updateTrigger(short selection, short val, string name)
        {
            if (val != prevPosition[name])
            {
                Serial_Updater.sendSerial(selection, val, name);
                prevPosition[name] = val;
            }

            
        }

        void updateTriggers()
        {
            updateTrigger(15,(short)state.Gamepad.LeftTrigger, "LeftTrigger");
            updateTrigger(16, (short)state.Gamepad.RightTrigger, "RightTrigger");
        }
    }
}
