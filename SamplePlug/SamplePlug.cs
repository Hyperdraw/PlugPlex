using PlugPlex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlug
{
    public class SamplePlug : Plug
    {

        public override Dictionary<string, Type> schemas {
            get {
                Dictionary<string, Type> _schemas = new Dictionary<string, Type>();
                _schemas.Add("Print", typeof(SamplePlugSchema));
                return _schemas;
            }
        }

        public override string name { get { return "SamplePlug"; } }

        public void Print(SamplePlugSchema args)
        {
            Console.WriteLine(args.msg);
        }
    }
}
