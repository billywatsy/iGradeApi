using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class TestMarkDto
    {
        public Guid StudentID { get; set; }
        public Guid TestID { get; set; }
        public Guid StudentTermRegisterID { get; set; }
        public Guid TeacherClassSubjectID { get; set; }
        public Guid SchoolID { get; set; }
        public Guid TermID { get; set; }
        public Guid ClassID { get; set; }
        public Guid LevelID { get; set; } 
        public string TestTitle { get; set; } 
        public string ClassName { get; set; } 
        public string LevelName { get; set; }
        public string Year { get; set; } 
        public string TermNumber { get; set; } 
        public string SubjectCode { get; set; } 
        public string SubjectName { get; set; } 
        public string RegNumber { get; set; } 
        public string FullName { get; set; } 
        public byte Mark { get; set; } 
        public byte OutOf { get; set; } 
        public byte MarkPercentage { get; set; } 
        public DateTime TestDateCreated { get; set; }
    }
    }

