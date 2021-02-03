using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jira.SDK
{
    public class Attachement
    {
        public int ID { get; set; }
        public User Author { get; set; }
        public String Filename { get; set; }
        public DateTime Created { get; set; }
        public int Size { get; set; }
        public String MimeType { get; set; }
        public String Content { get; set; }

        [JsonIgnore]
        public byte[] Data { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Attachement()
        {
            Data = null;
        }
    }

    public class Attachements : List<Attachement>
    {
    }
}
