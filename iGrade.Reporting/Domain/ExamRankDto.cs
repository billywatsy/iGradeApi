using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Reporting.Domain
{
    public class ExamRankDto
    {
        public Guid StudentTermRegisterID { get; set; }
        public Guid StudentID { get; set; }
        public Guid TeacherClassSubjectID { get; set; }
        public Guid SchoolID { get; set; }
        public Guid TermID { get; set; }
        public int Year { get; set; }
        public int TermNumber { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string RegNumber { get; set; }
        public string FullName { get; set; }
        public byte Mark { get; set; }
        public string Grade { get; set; }
        public string Comment { get; set; }
        public bool IsMale { get; set; }
        public string LevelName { get; set; }
        public string ClassName { get; set; }

        // extras 
        public int Mark_OveralAverage { get; set; }
        public int Mark_OveralMarks { get; set; }
        public int Mark_NumberWritten { get; set; }
        public int Rank_SubjectClass_Position { get; set; }
        public int Rank_SubjectClass_OutOf { get; set; }
        public int Rank_SubjectLevel_Position { get; set; }
        public int Rank_SubjectLevel_OutOf { get; set; }
        public int Rank_OveralClass_Position { get; set; }
        public int Rank_OveralClass_OutOf { get; set; }
        public int Rank_OveralLevel_Position { get; set; }
        public int Rank_OveralLevel_OutOf { get; set; }
    }
}
