using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jira.SDK.Domain
{
    public class IssueFields
    {
        public String Summary { get; set; }
        public String Description { get; set; }
        public CommentSearchResult Comment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime ResolutionDate { get; set; }
        public IssueType IssueType { get; set; }
        public User Reporter { get; set; }
        public User Assignee { get; set; }
        public List<ProjectVersion> FixVersions { get; set; }
        public List<ProjectVersion> AffectsVersions { get; set; }
        public List<Component> Components { get; set; }
        public Project Project { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public Resolution Resolution { get; set; }
        public ParentIssue Parent { get; set; }
        public List<Subtask> Subtasks { get; set; }
        public TimeTracking TimeTracking { get; set; }
        public WorklogSearchResult Worklog { get; set; }
        public List<IssueLink> IssueLinks { get; set; }
        public List<String> Labels { get; set; }
        public List<Attachement> Attachements { get; set; }
        public Dictionary<String, CustomField> CustomFields { get; set; }

        public IssueFields() { }

        public IssueFields(JObject fieldsObj)
        {
            Dictionary<String, Object> fields = fieldsObj.ToObject<Dictionary<String, Object>>();

            Created = fields["created"] != null ? (DateTime)fields["created"] : DateTime.MinValue;
            Updated = fields["updated"] != null ? (DateTime)fields["updated"] : DateTime.MinValue;
            ResolutionDate = fields["resolutiondate"] != null ? (DateTime)fields["resolutiondate"] : DateTime.MinValue;

            IssueType = null;
            if (fields.ContainsKey("issuetype") && fields["issuetype"] != null)
            {
                IssueType = ((JObject)fields["issuetype"]).ToObject<IssueType>();
            }

            Reporter = null;
            if (fields.ContainsKey("reporter") && fields["reporter"] != null)
            {
                Reporter = ((JObject)fields["reporter"]).ToObject<User>();
            }

            Assignee = null;
            if (fields.ContainsKey("assignee") && fields["assignee"] != null)
            {
                Assignee = ((JObject)fields["assignee"]).ToObject<User>();
            }

            Summary = "";
            if (fields.ContainsKey("summary") && fields["summary"] != null)
            {
                Summary = (String)fields["summary"];
            }

            Comment = new CommentSearchResult();
            if (fields.ContainsKey("comment") && fields["comment"] != null)
            {
                Comment = ((JObject)fields["comment"]).ToObject<CommentSearchResult>();
            }

            Description = "";
            if (fields.ContainsKey("description") && fields["description"] != null)
            {
                Description = (String)fields["description"];
            }

            FixVersions = new List<ProjectVersion>();
            if (fields.ContainsKey("fixVersions") && fields["fixVersions"] != null)
            {
                JArray versionArray = (JArray)fields["fixVersions"];
                if (versionArray.Count > 0)
                {
                    FixVersions = ((JArray)fields["fixVersions"]).ToObject<List<ProjectVersion>>();
                }
            }

            AffectsVersions = new List<ProjectVersion>();
            if (fields.ContainsKey("versions") && fields["versions"] != null)
            {
                JArray versionArray = (JArray)fields["versions"];
                if (versionArray.Count > 0)
                {
                    AffectsVersions = ((JArray)fields["versions"]).ToObject<List<ProjectVersion>>();
                }
            }

            Components = new List<Component>();
            if (fields.ContainsKey("components") && fields["components"] != null)
            {
                JArray versionArray = (JArray)fields["components"];
                if (versionArray.Count > 0)
                {
                    Components = ((JArray)fields["components"]).ToObject<List<Component>>();
                }
            }

            Project = null;
            if (fields.ContainsKey("project") && fields["project"] != null)
            {
                Project = ((JObject)fields["project"]).ToObject<Project>();
            }

            Status = null;
            if (fields.ContainsKey("status") && fields["status"] != null)
            {
                Status = ((JObject)fields["status"]).ToObject<Status>();
            }

            Priority = null;
            if (fields.ContainsKey("priority") && fields["priority"] != null)
            {
                Priority = ((JObject)fields["priority"]).ToObject<Priority>();
            }

            Resolution = null;
            if (fields.ContainsKey("resolution") && fields["resolution"] != null)
            {
                Resolution = ((JObject)fields["resolution"]).ToObject<Resolution>();
            }

            Parent = null;
            if (fields.ContainsKey("parent") && fields["parent"] != null)
            {
                //Parent = new Issue();
            }

            Subtasks = null;
            if (fields.ContainsKey("subtasks"))
            {
                JArray subtasks = (JArray)fields["subtasks"];

                //Subtasks = null;
            }

            Worklog = new WorklogSearchResult();
            if (fields.ContainsKey("worklog"))
            {
                Worklog = ((JObject)fields["worklog"]).ToObject<WorklogSearchResult>();
            }

            IssueLinks = new List<IssueLink>();
            if (fields.ContainsKey("issuelinks"))
            {
                JArray linkArray = (JArray)fields["issuelinks"];
                if (linkArray.Count > 0)
                {
                    IssueLinks = linkArray.Select(link => ((JObject)link).ToObject<IssueLink>()).ToList();
                }
            }

            Labels = new List<String>();
            if (fields.ContainsKey("labels"))
            {
                JArray labelArray = (JArray)fields["labels"];
                if (labelArray.Count > 0)
                {
                    Labels = labelArray.Select(label => (String)label).ToList();
                }
            }

            TimeTracking = null;
            if (fields.ContainsKey("timetracking") && fields["timetracking"] != null)
            {
                TimeTracking = ((JObject)fields["timetracking"]).ToObject<TimeTracking>();
            }

            Attachements = new List<Attachement>();
            if (fields.ContainsKey("attachment") && fields["attachment"] != null)
            {
                JArray attachementArray = (JArray)fields["attachment"];
                if (attachementArray.Count > 0)
                {
                    Attachements = ((JArray)fields["attachment"]).ToObject<List<Attachement>>();
                }
            }

            CustomFields = new Dictionary<String, CustomField>();
            foreach (String customFieldName in fields.Keys.Where(key => key.StartsWith("customfield_")))
            {
                switch (fieldsObj[customFieldName].Type)
                {
                    case JTokenType.Object:
                        CustomFields.Add(customFieldName, ((JObject)fieldsObj[customFieldName]).ToObject<CustomField>());
                        break;
                    case JTokenType.Null:
                        CustomFields.Add(customFieldName, null);
                        break;
                    case JTokenType.Array:
                        // TODO Handle Array Type
                        CustomFields.Add(customFieldName, new CustomField(((JArray)fieldsObj[customFieldName]).ToString(Newtonsoft.Json.Formatting.None)));
                        break;
                    case JTokenType.Float:
                        CustomFields.Add(customFieldName, new CustomField(((float)fieldsObj[customFieldName]).ToString()));
                        break;
                    default:
                        CustomFields.Add(customFieldName, new CustomField((String)fieldsObj[customFieldName]));
                        break;
                }

            }
        }
    }
}
