﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jira.SDK.Domain
{
    public class ProjectRole
    {
        public Jira Jira { get; set; }

        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Self { get; set; }
        public List<ProjectRoleActor> Actors { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProjectRoleActor
    {
        public Int32 Id { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
