using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jira.SDK.Domain
{
	public class IssueLinkType
	{
		public enum IssueLinkTypeEnum
		{
			Cloners,
			Relates,
			Blocks,
			Duplicate,
		}

		public Int32 ID { get; set; }
		public String Name { get; set; }

		public IssueLinkTypeEnum ToEnum()
		{
            IssueLinkTypeEnum value;
            if (Enum.TryParse<IssueLinkTypeEnum>(Name, out value))
            {
                return value;
            }
            else
            {
                return default(IssueLinkTypeEnum);
            }
		}


        public override string ToString()
        {
            return Name;
        }
    }
}
