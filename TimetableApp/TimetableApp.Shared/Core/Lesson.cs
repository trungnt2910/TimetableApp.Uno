using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TimetableApp.Core
{
    [Serializable]
    public partial class Lesson
    {
        //Stuff that will be displayed:
        public string Subject { get; set; }
        public string TeacherName { get; set; }
        public string Notes { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Dictionary<string, string> AdditionalTags { get; set; }

        //Instead of maunally saving ID and Password, we're saving a credentials object
        //Depending on the type of class, the Teacher may give Students other stuff, such as
        //an authorization key, a JSON file, some spyware,...
        public ICredentials Credentials { get; set; }
        
        //This is for binding support.
        [JsonIgnore]
        public bool SupportsOneClick { get => Credentials?.SupportsOneClick == true; }

        [JsonIgnore]
        public string DayOfWeek { get; set; }

        public Lesson()
        {
            Subject = Subject ?? string.Empty;
            TeacherName = TeacherName ?? string.Empty;
            Notes = Notes ?? string.Empty;
            AdditionalTags = AdditionalTags ?? new Dictionary<string, string>();
        }

        //Displays the string:
        public string ToMarkdown()
        {
            //Trailing spaces must be kept here, else the Markdown will not display correctly.
            string richString =
$@"**Subject**: {Subject}  
**Name**: {TeacherName}  
**Start**: {StartTime}  
**End**: {EndTime}  
**Lesson type**: {Credentials.Type}  
**Lesson credentials**: {Credentials}  
**Supports OneClick**: {Credentials.SupportsOneClick}  
**Notes**: {Notes}  
**Additional Info**:  
";
            foreach (var kvp in AdditionalTags)
            {
                richString += $"{kvp.Key}: {kvp.Value}  " + Environment.NewLine;
            }

            return richString;
        }

        public void EnterClass(StudentInfo info)
        {
            Credentials.EnterClass(info);
        }
    }
}
