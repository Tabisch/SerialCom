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
        string info;

        public ControllerActionCalls(short selection, short input, string info)
        {
            this.selection = selection;
            this.input = input;
            this.info = info;
        }

        public short getSelection()
        {
            return selection;
        }

        public short getInput()
        {
            return input;
        }

        public string getInfo()
        {
            return info;
        }
    }
}
