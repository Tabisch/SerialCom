using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCom
{
    class ControllerActionCalls
    {
        short selection;
        short input;

        public ControllerActionCalls(short selection, short input)
        {
            this.selection = selection;
            this.input = input;
        }

        public short getSelection()
        {
            return selection;
        }

        public short getInput()
        {
            return input;
        }
    }
}
