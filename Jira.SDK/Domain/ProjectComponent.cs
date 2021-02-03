using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jira.SDK.Domain;
using Newtonsoft.Json;

namespace Jira.SDK
{
    [JsonObject(ItemRequired = Required.Always)]
    public class ProjectComponent
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        // Only way to stop RestSharp on deserializing Project is to make it internal
        internal Project Project { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
