using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService.Common
{
    public static class PolicyCommon
    {
        #region Overal
        public static  int OveralLevel =             15;
        public static int OveralClass =             100; 
        public static long OveralStudent   =    1000000;
        public static long OveralTeacher   =     100000;
        public static long OveralSubjects  =        500;
        public static long OveralExam      =  100000000; 
        public static long OveralTest      =   10000000; 
        public static long OveralTestMark  = 1000000000; 
        #endregion

        #region TermData
        public static int TeeacherClassSubject = 2000;
        public static int StudentTermRegister = 5000;
        public static int StudentInClass = 200;
        #endregion
    }
}
