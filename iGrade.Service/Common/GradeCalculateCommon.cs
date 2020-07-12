using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService.Common
{
    public class GradeCalculateCommon
    {
        public static string Grade (Grade grade , List<GradeMark> gradeMarkList , int Mark ,ref string defaultComment  , ref bool errorCalculating) 
        {
            defaultComment = "null";
            string gradeValue = null;
            if(grade == null)
            {
                errorCalculating = true;

                return "-z";
            }

            if (gradeMarkList == null || gradeMarkList.Count <= 0)
            {
                errorCalculating = true;
                return "-z";
            }
            else
            { 
                foreach (var itemMarks in gradeMarkList)
                {
                    if (Mark >= itemMarks.FromMark && Mark <= itemMarks.ToMark)
                    {
                        defaultComment = itemMarks.DefaultComment;
                        gradeValue = itemMarks.GradeValue;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(gradeValue))
                {
                    errorCalculating = true;
                    return "-z";
                }
            }
            return gradeValue.ToUpper();
        }
    }
     
}
