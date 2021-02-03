using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using RestSharp;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Jira.SDK.Domain
{
    public partial class Issue
    {
        private Jira _jira { get; set; }
        public Jira GetJira()
        {
            return _jira;
        }

        public void SetJira(Jira jira)
        {
            _jira = jira;
        }

        public Issue() { }

        public Issue(String key, JObject fields)
        {
            this.Key = key;
            this.Fields = new IssueFields(fields);
        }

        public String GetCustomFieldValue(String customFieldName)
        {
            Field field = GetJira().Fields.FirstOrDefault(f => f.Name.Equals(customFieldName));
            if (field == null)
            {
                throw new ArgumentException(String.Format("The field with name {0} does not exist.", customFieldName), customFieldName);
            }
            String fieldId = field.ID;
            return (Fields.CustomFields[fieldId] != null ? Fields.CustomFields[fieldId].Value : "");
        }

        public void SetCustomFieldValue(String customFieldName, String value)
        {
            Field field = GetJira().Fields.FirstOrDefault(f => f.Name.Equals(customFieldName));
            if (field == null)
            {
                throw new ArgumentException(String.Format("The field with name {0} does not exist.", customFieldName), customFieldName);
            }

            if (Fields.CustomFields[field.ID] == null)
            {
                Fields.CustomFields[field.ID] = new CustomField(value);
            }
            Fields.CustomFields[field.ID].Value = value;
        }

        public String Key { get; set; }
        public IssueFields Fields { get; set; }
        public String Url
        {
            get
            {
                return string.Format("{0}browse/{1}", _jira.Client.GetBaseUrl(), Key);
            }
        }

        public String Summary
        {
            get { return Fields.Summary; }
            set { Fields.Summary = value; }
        }

        public String Description
        {
            get { return Fields.Description; }
            set { Fields.Description = value; }
        }

        private List<Issue> _subtasks;
        public List<Issue> Subtasks
        {
            get
            {
                if (_subtasks == null)
                {
                    _subtasks = _jira.Client.GetSubtasksFromIssue(this.Key);
                    _subtasks.ForEach(subtask => subtask.SetJira(_jira));
                }
                return _subtasks;
            }
            set
            {
                _subtasks = value;
            }
        }

        public List<Comment> _comments;
        public List<Comment> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = Fields.Comment.Comments;
                }
                return _comments;
            }
        }

        public List<String> Labels
        {
            get
            {
                return Fields.Labels;
            }
        }

        public void AddComment(Comment comment)
        {
            Comments.Add(GetJira().Client.AddCommentToIssue(this, comment));
        }

        /// <summary>
        /// Update issue summary
        /// </summary>
        /// <param name="summary">New summary text</param>
        public void UpdateSummary(string summary)
        {
           Summary = GetJira().Client.UpdateIssueSummary(this, summary);
        }

        public Status Status
        {
            get { return Fields.Status; }
        }

        private TimeTracking _timeTracking;
        public TimeTracking TimeTracking
        {
            get
            {
                if (_timeTracking == null)
                {
                    if (Fields.TimeTracking != null)
                    {
                        _timeTracking = Fields.TimeTracking;
                    }
                    else
                    {
                        Issue issue = _jira.Client.GetIssue(this.Key);
                        _timeTracking = issue.Fields.TimeTracking;
                    }

                    _timeTracking.Issue = this;
                }
                return _timeTracking;
            }
        }

        private List<Worklog> _worklogs;
        public List<Worklog> GetWorklogs()
        {
            if (_worklogs == null)
            {
                if (Fields.Worklog != null)
                {
                    _worklogs = Fields.Worklog.Worklogs;
                }
                else
                {
                    _worklogs =
                       _jira.Client.GetWorkLogs(this.Key).Worklogs;
                }
                _worklogs.ForEach(wl => wl.Issue = this);
            }
            return _worklogs;
        }

        public void RefreshWorklogs()
        {
            // this will force the re-loading of any work logs
            _worklogs = _jira.Client.GetWorkLogs(this.Key).Worklogs;
        }

        public List<Transition> Transitions { get; set; }
        private List<Transition> _transitions;
        public List<Transition> GetTransitions()
        {
            if (_transitions == null)
            {
                if (Transitions != null)
                {
                    _transitions = Transitions;
                }
                else
                {
                    _transitions =
                       _jira.Client.GetTransitions(this.Key);
                }
            }
            return _transitions;
        }

        public void TransitionIssue(Transition transition, Comment comment)
        {
            _jira.Client.TransitionIssue(this, transition, comment);
        }

        private Issue _parent;
        public Issue Parent
        {
            get
            {
                if (_parent == null && Fields.Parent != null)
                {
                    _parent = _jira.Client.GetIssue(Fields.Parent.Key);
                    _parent.SetJira(_jira);
                }
                return _parent;
            }
        }

        public IssueType IssueType
        {
            get
            {
                return Fields.IssueType;
            }
        }

        public User Assignee
        {
            get { return Fields.Assignee ?? User.UndefinedUser; }
            set { Fields.Assignee = value; }
        }

        public User Reporter
        {
            get { return Fields.Reporter ?? User.UndefinedUser; }
            set { Fields.Reporter = value; }
        }

        public DateTime Created
        {
            get { return Fields.Created; }
            set { Fields.Created = value; }
        }
        public DateTime Updated
        {
            get { return Fields.Updated; }
            set { Fields.Updated = value; }
        }

        public DateTime Resolved
        {
            get
            {
                if (Fields.ResolutionDate.CompareTo(DateTime.MinValue) == 0)
                {
                    Fields.ResolutionDate = DateTime.MaxValue;
                }
                return Fields.ResolutionDate;
            }
        }

        private Project _project = null;
        public Project Project
        {
            get
            {
                if (_project == null)
                {
                    _project = this.Fields.Project;
                    _project.SetJira(_jira);
                }
                return _project;
            }
        }

        public Sprint Sprint { get; set; }

        public Dictionary<String, String> CustomFields
        {
            get;
            set;
        }

        private List<IssueLink> IssueLinks
        {
            get
            {
                return Fields.IssueLinks;
            }
            set
            {
                Fields.IssueLinks = value;
            }
        }      

        /// <summary>
        /// List of attachements
        /// </summary>
        public List<Attachement> _attachements;
        public List<Attachement> Attachements
        {
            get
            {
                if (_attachements == null)
                {
                    _attachements = Fields.Attachements;

                    // Download attachements
                    foreach (Attachement attachement in _attachements)
                    {
                        GetJira().Client.DownloadAttachement(attachement);
                    }
                }
                return _attachements;
            }
        }

        public void AddAttachement(Attachement attachement)
        {
            Attachements.Add(GetJira().Client.AddAttachementToIssue(this, attachement));
        }

        /// <summary>
        /// This method returns all issues which where cloned from this one.
        /// </summary>
        /// <returns>The list of issues which where cloned from this one</returns>
        public List<Issue> GetClones()
        {
            List<Issue> clones = IssueLinks.Where(link => link.Type.ToEnum() == IssueLinkType.IssueLinkTypeEnum.Cloners && link.InwardIssue != null).Select(link => link.InwardIssue).ToList();
            loadIssues(clones);

            return clones;
        }

        /// <summary>
        /// This method returns all issues which where cloned from this one.
        /// </summary>
        /// <returns>The list of issues which where cloned from this one</returns>
        public Issue GetClonedIssue()
        {
            Issue cloned = IssueLinks.Where(link => link.Type.ToEnum() == IssueLinkType.IssueLinkTypeEnum.Cloners && link.OutwardIssue != null).Select(link => link.OutwardIssue).FirstOrDefault();
            if (cloned != null)
            {
                loadIssues(new List<Issue>() { cloned });
            }

            return cloned;
        }

        /// <summary>
        /// This method returns all issues which are blocking this one.
        /// </summary>
        /// <returns>The list of issues which are blocking this one</returns>
        public List<Issue> GetBlockingIssues()
        {
            List<Issue> blockingIssues = IssueLinks.Where(link => link.Type.ToEnum() == IssueLinkType.IssueLinkTypeEnum.Blocks && link.InwardIssue != null).Select(link => link.InwardIssue).ToList();
            loadIssues(blockingIssues);

            return blockingIssues;
        }

        /// <summary>
        /// This method returns all of the issues which are blocked by this one.
        /// </summary>
        /// <returns>The list of issues which are blocked by this one</returns>
        public List<Issue> GetImpactedIssues()
        {
            List<Issue> impactedIssues = IssueLinks.Where(link => link.Type.ToEnum() == IssueLinkType.IssueLinkTypeEnum.Blocks && link.OutwardIssue != null).Select(link => link.OutwardIssue).ToList();
            loadIssues(impactedIssues);

            return impactedIssues;
        }

        /// <summary>
        /// This method returns all of the issues which are duplicates from this one.
        /// </summary>
        /// <returns>The list of issues which are duplicates from this one</returns>
        public List<Issue> GetDuplicateIssues()
        {
            List<Issue> duplicateIssues = IssueLinks.Where(link => link.Type.ToEnum() == IssueLinkType.IssueLinkTypeEnum.Duplicate).Select(link => (link.InwardIssue != null ? link.InwardIssue : link.OutwardIssue)).ToList();
            loadIssues(duplicateIssues);

            return duplicateIssues;
        }

        /// <summary>
        /// This method returns all of the issues which relate to this one.
        /// </summary>
        /// <returns>The list of issues which are relate to this one</returns>
        public List<Issue> GetRelatedIssues()
        {
            List<Issue> relatedIssues = IssueLinks.Where(link => link.Type.ToEnum() == IssueLinkType.IssueLinkTypeEnum.Relates).Select(link => (link.InwardIssue != null ? link.InwardIssue : link.OutwardIssue)).ToList();
            loadIssues(relatedIssues);

            return relatedIssues;
        }

        /// <summary>
        /// This method iterates every issue in the issue list and makes sure this issue is loaded and ready for querying.
        /// </summary>
        /// <param name="issues"></param>
        private void loadIssues(List<Issue> issues)
        {
            issues.Where(issue => issue != null).ToList().ForEach(issue =>
            {
                issue.SetJira(this._jira);
                issue.Load();
            });
        }

        public void Load()
        {
            Issue issue = _jira.Client.GetIssue(this.Key);
            this.Fields = issue.Fields;
        }

        #region Custom fields for Jira Agile
        public Int32 SprintID
        {
            get
            {
                String sprintDescription = GetCustomFieldValue("Sprint");
                if (!String.IsNullOrEmpty(sprintDescription))
                {
                    MatchCollection matches = Regex.Matches(sprintDescription, ",id=(?<SprintID>\\d+)]");
                    Int32 id = -1;

                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            id = Int32.Parse(match.Groups["SprintID"].Value);
                        }
                    }

                    return id;
                }
                return -1;
            }
        }

        public String SprintNames
        {
            get
            {
                String sprintDescription = GetCustomFieldValue("Sprint");
                if (!String.IsNullOrEmpty(sprintDescription))
                {
                    MatchCollection matches = Regex.Matches(sprintDescription, ",name=(?<SprintName>.*?),");
                    String names = "";

                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            names += match.Groups["SprintName"].Value;
                            if (match.NextMatch().Success)
                            {
                                names += ", ";
                            }
                        }
                    }

                    return names;
                }
                return "";
            }
        }

        private Epic _epic;
        public Epic Epic
        {
            get
            {
                if (_epic == null && !String.IsNullOrEmpty(GetCustomFieldValue("Epic Link")))
                {                   
                    Issue issue = GetJira().Client.GetIssue(GetCustomFieldValue("Epic Link"));
                    if ( issue != null)
                    {
                        issue.SetJira(GetJira());
                        _epic = Epic.FromIssue(issue);
                    }
                    
                }
                return _epic;
            }
            set
            {
                _epic = value;
            }
        }

        public Int32 Rank
        {
            get
            {
                MatchCollection matches = Regex.Matches(GetCustomFieldValue("Rank"), ",^(?<Rank>\\d+)]");
                Int32 rank = -1;

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        rank = Int32.Parse(match.Groups["Rank"].Value);
                    }
                }

                return rank;
            }
            set
            {
                SetCustomFieldValue("Rank", value.ToString());
            }
        }

        public String Severity
        {
            get
            {
                return (GetCustomFieldValue("Severity") != null ? GetCustomFieldValue("Severity") : "");
            }
        }
        #endregion

        #region equality

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Issue)
                return Key.Equals(((Issue)obj).Key);
            return false;
        }

        #endregion

        public override string ToString()
        {
            return Key;
        }
    }
}
