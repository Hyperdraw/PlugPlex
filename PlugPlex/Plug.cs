using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PlugPlex
{
    public abstract class Plug : WebSocketBehavior
    {
        [HandleProcessCorruptedStateExceptions]
        protected override void OnMessage(MessageEventArgs e)
        {
            //string nameLengthString = e.Data.Substring(0, 2);
            int nameLength;

            if(!int.TryParse(e.Data.Substring(0, 2), out nameLength))
            {
                Send(JsonConvert.SerializeObject(new PlugError("Data packet is corrupt!")));
                return;
            }

            string name = e.Data.Substring(2, nameLength);
            string argJson = e.Data.Substring(nameLength + 2);

            Type schema = null;

            try
            {
                schema = schemas[name];
            }
            catch (KeyNotFoundException)
            {
                Send(JsonConvert.SerializeObject(new PlugError("Function not found!")));
                return;
            }

            object args = null;
            try
            {
                args = JsonConvert.DeserializeObject(argJson, schema);
            }catch(JsonReaderException)
            {
                Send(JsonConvert.SerializeObject(new PlugError("JSON data is malformed!")));
                return;
            }

            Type type = this.GetType();

            object ret = null;

            try
            {
                MethodInfo method = type.GetMethod(name);

                ret = method.Invoke(this, new object[] { args });
            }
            catch (Exception ex)
            {
                Send(JsonConvert.SerializeObject(new PlugError("An unexpected error occurred: " + ex.Message)));
            }

            Send(JsonConvert.SerializeObject(ret));
        }

        public abstract Dictionary<string, Type> schemas {
            get;
        }

        public abstract string name {
            get;
        }
    }
}
