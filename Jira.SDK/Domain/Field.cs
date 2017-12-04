﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jira.SDK.Domain
{
	public class Field
	{
		public String ID { get; set; }
		public String Name { get; set; }
		public Boolean Custom { get; set; }
        public FieldSchema Schema { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class FieldSchema
    {
        public String Type { get; set; }
        public String Custom { get; set; }
        public Int32 CustomID { get; set; }
    }
}
