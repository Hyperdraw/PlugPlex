using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlugPlex
{
    class PlugError
    {
        public bool success = false;
        public String msg;

        public PlugError(string msg)
        {
            this.msg = msg;
        }
    }
}
