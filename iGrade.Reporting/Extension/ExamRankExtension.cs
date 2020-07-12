using iGrade.Domain.Dto;
using iGrade.Reporting.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iGrade.Reporting.Extension
{
    public static class ExamRankExtension
    {
        private static List<ExamRankDto> ToRankMapper(this List<ExamDto> list)
        {
            List<ExamRankDto> exams = new List<ExamRankDto>();
            foreach (var exam in list)
            {
                exams.Add(new ExamRankDto()
                {
                    StudentTermRegisterID = exam.StudentTermRegisterID,
                    StudentID = exam.StudentID,
                    TeacherClassSubjectID = exam.TeacherClassSubjectID,
                    SchoolID = exam.SchoolID,
                    TermID = exam.TermID,
                    Year = exam.Year,
                    TermNumber = exam.TermNumber,
                    SubjectCode = exam.SubjectCode,
                    SubjectName = exam.SubjectName,
                    RegNumber = exam.RegNumber,
                    FullName = exam.FullName,
                    Mark = exam.Mark,
                    Grade = exam.Grade,
                    Comment = exam.Comment,
                    IsMale = exam.IsMale,
                    LevelName = exam.LevelName,
                    ClassName = exam.ClassName
                }
                );
            }
            return exams ?? new List<ExamRankDto>();
        }
        public static List<ExamRankDto> ToClassSubjectRank(this List<ExamDto> list)
        {
            return list.ToRankMapper().ToClassSubjectRank();
        }

        /// <summary>
        /// List of the classes to be ranked , this will rank student rank for a subject in the class he/she is in
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<ExamRankDto> ToClassSubjectRank(this List<ExamRankDto> list)
        {
            if (list == null)
            {
                return new List<ExamRankDto>();
            }
            var getUnique_Class = list.Select(c => c.ClassName.ToLower() + "-" + c.LevelName.ToLower()).ToList().Distinct();

            if (getUnique_Class == null)
            {
                return new List<ExamRankDto>();
            }
            List<ExamRankDto> finalList = new List<ExamRankDto>();
            foreach (string name in getUnique_Class)
            {  
                var uniqueSuubjectList = list.Where(c => c.ClassName.ToLower() + "-" + c.LevelName.ToLower() == name)
                                             .Select(c => c.SubjectCode.ToLower()).Distinct();

                foreach (var subjectCode in uniqueSuubjectList)
                {
                    var totalWhoWroteSubject = list.Where(c => c.ClassName.ToLower() + "-" + c.LevelName.ToLower() == name)
                                            .Where(c => c.SubjectCode.ToLower() == subjectCode)
                                            .OrderByDescending(c => c.Mark)
                                            .ToList();

                    int last_score = -9;
                    int rows = 0;
                    int rank = 0;
                    var outOf = totalWhoWroteSubject?.Count() ?? 0;
                    foreach (var studentInClass in totalWhoWroteSubject)
                    {
                        rows++;
                        if (last_score != studentInClass.Mark)
                        {
                            last_score = studentInClass.Mark;
                            rank = rows;
                        }
                        studentInClass.Rank_SubjectClass_Position = rank;
                        studentInClass.Rank_SubjectClass_OutOf = outOf;

                        finalList.Add(studentInClass);
                    }
                }
            }
            return finalList;
        }
        public static List<ExamRankDto> ToLevelSubjectRank(this List<ExamDto> list)
        {
            return list.ToRankMapper().ToLevelSubjectRank();
        }

        /// <summary>
        /// Rank subject for the level
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<ExamRankDto> ToLevelSubjectRank(this List<ExamRankDto> list)
        {
            var getUnique_Level = list.Select(c => c.LevelName.ToLower()).Distinct();

            if (getUnique_Level == null)
            {
                return new List<ExamRankDto>();
            }
            List<ExamRankDto> finalList = new List<ExamRankDto>();
            foreach (string name in getUnique_Level)
            {
                var uniqueSuubjectList = list.Where(c => c.LevelName.ToLower() == name)
                                             .Select(c => c.SubjectCode.ToLower()).Distinct();

                foreach (var subjectCode in uniqueSuubjectList)
                {
                    var totalWhoWroteSubject = list.Where(c => c.LevelName.ToLower() == name)
                                            .Where(c => c.SubjectCode.ToLower() == subjectCode)
                                            .OrderByDescending(c => c.Mark)
                                            .ToList();

                    int last_score = -9;
                    int rows = 0;
                    int rank = 0;
                    var outOf = totalWhoWroteSubject?.Count() ?? 0;
                    foreach (var studentInClass in totalWhoWroteSubject)
                    {
                        rows++;
                        if (last_score != studentInClass.Mark)
                        {
                            last_score = studentInClass.Mark;
                            rank = rows;
                        }
                        studentInClass.Rank_SubjectLevel_Position = rank;
                        studentInClass.Rank_SubjectLevel_OutOf = outOf;
                        finalList.Add(studentInClass);
                    }
                }
            }
            return finalList;
        }

        public static List<ExamRankDto> ToLevelOveralRank(this List<ExamDto> list)
        {
            return list.ToRankMapper().ToLevelOveralRank();
        }
        public static List<ExamRankDto> ToLevelOveralRank(this List<ExamRankDto> list)
        {
            if(list == null || list?.Count() <= 0)
            {
                return new List<ExamRankDto>();
            }
            var newList = list.GroupBy(x => new { x.RegNumber, x.TermNumber, x.Year, x.ClassName, x.LevelName })
                   .Select(y => new
                   {
                       RegNumber = y.Key.RegNumber,
                       TermNumber = y.Key.TermNumber,
                       Year = y.Key.Year,
                       ClassName = y.Key.ClassName,
                       LevelName = y.Key.LevelName,
                       TotalWritten = list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          ?.Count() ?? 0,
                       TotalMarks = (
                                        list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Count() >= 1 ? list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Sum(c => c.Mark) : 0
                                    ),
                       TotalAverage = (
                                        list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Count() >= 1 ? list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Average(c => c.Mark) : 0
                                    )
                   }
                   ).OrderByDescending(c => c.TotalAverage)
                   .ToList();

            if (newList == null || newList.Count() <= 0)
            {
                return new List<ExamRankDto>();
            }
            int last_score = -9;
            int rows = 0;
            int rank = 0;
            var outOf = newList.Count();

            Dictionary<string, int> ranks = new Dictionary<string, int>();
            foreach (var studentInLevel in newList)
            {
                rows++;
                if (last_score != studentInLevel.TotalAverage)
                {
                    last_score = Convert.ToInt32(studentInLevel.TotalAverage);
                    rank = rows;
                }
                ranks.Add(studentInLevel.RegNumber, rank);
            }

            foreach (var studentRank in list)
            {
                if (ranks == null)
                {
                    continue;
                }
                var levelRank = ranks.Where(c => c.Key == studentRank.RegNumber).FirstOrDefault();

                studentRank.Mark_OveralAverage = Convert.ToInt32(newList.FirstOrDefault(c => c.RegNumber == studentRank.RegNumber)?.TotalAverage ?? 0);
                try
                {
                    studentRank.Rank_OveralLevel_Position = levelRank.Value;
                    studentRank.Rank_OveralLevel_OutOf = outOf;
                }
                catch
                {
                }
            }

            
            return list;
        }

        public static List<ExamRankDto> ToClassOveralRank(this List<ExamDto> list)
        {
            return list.ToRankMapper().ToClassOveralRank();
        }
        public static List<ExamRankDto> ToClassOveralRank(this List<ExamRankDto> list)
        {
            if (list == null)
            {
                return new List<ExamRankDto>();
            }
            var getUnique_Class = list.Select(c => c.ClassName.ToLower() + "-" + c.LevelName.ToLower()).ToList().Distinct();

            if (getUnique_Class == null)
            {
                return new List<ExamRankDto>();
            }


            Dictionary<string, int> ranks = new Dictionary<string, int>();
            Dictionary<string, int> classTotal = new Dictionary<string, int>();
            List<ExamRankDto> finalList = new List<ExamRankDto>();
            foreach (string name in getUnique_Class)
            {
                var studentInClass = list.Where(c => c.ClassName.ToLower() + "-" + c.LevelName.ToLower() == name).ToList();

                if (studentInClass == null)
                {
                    continue;
                }

                var newList = studentInClass.GroupBy(x => new { x.RegNumber, x.TermNumber, x.Year, x.ClassName, x.LevelName })
                   .Select(y => new
                   {
                       RegNumber = y.Key.RegNumber,
                       TermNumber = y.Key.TermNumber,
                       Year = y.Key.Year,
                       ClassName = y.Key.ClassName,
                       LevelName = y.Key.LevelName,
                       TotalWritten = list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Count(),
                       TotalMarks = (
                                        list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Count() >= 1 ? list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Sum(c => c.Mark) : 0
                                    ),
                       TotalAverage = (
                                        list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Count() >= 1 ? list.Where(c => c.RegNumber == y.Key.RegNumber)
                                          .Average(c => c.Mark) : 0
                                    ),
                       TotalStudentsInClass = 0 // to be set in the loop
                   }
                   ).OrderByDescending(c => c.TotalAverage)
                   .ToList();

                if (newList == null || newList.Count() <= 0)
                {
                    return new List<ExamRankDto>();
                }
                int last_score = -9;
                int rows = 0;
                int rank = 0;
                var outOf = newList.Count();

                foreach (var studentInLevel in newList)
                {
                    rows++;
                    if (last_score != studentInLevel.TotalAverage)
                    {
                        last_score = Convert.ToInt32(studentInLevel.TotalAverage);
                        rank = rows;
                    }
                    ranks.Add(studentInLevel.RegNumber, rank);
                    classTotal.Add(studentInLevel.RegNumber, outOf);
                }

            }


            // set ranks

            foreach (var studentRank in list)
            {
                if (ranks == null)
                {
                    continue;
                }
                var levelRank = ranks.Where(c => c.Key == studentRank.RegNumber).FirstOrDefault();
                var overal = classTotal.Where(c => c.Key == studentRank.RegNumber).FirstOrDefault();

                try
                {

                    studentRank.Rank_OveralClass_Position = levelRank.Value;
                    studentRank.Rank_OveralClass_OutOf = overal.Value;
                }
                catch
                {
                }
            }
            return list;
        }

    }
}