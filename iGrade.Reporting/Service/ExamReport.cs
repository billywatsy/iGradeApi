using iGrade.Domain;
using iGrade.Domain.Dto;
using iGrade.Reporting.Domain;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using iGrade.Reporting.Extension;

namespace iGrade.Reporting.Service
{
    public class ExamReport
    {
        private UowRepository _uofRepository;
        public ExamReport(UowRepository uowRepository)
        {
            _uofRepository = uowRepository;
        }
        public StudentSubjectMarksDto GetStudentMarksListByCurrentTermAndStudentTermRegistrationAndSubjectCode(Guid studentTermRegistrationId, string subjectCode)
        {
            StudentSubjectMarksDto studentSubjectMarksDto = new StudentSubjectMarksDto();

            bool dbFlag = false;
            var studetTermReg = _uofRepository.StudentTermRegisterRepository.GetByEnrollemtID(studentTermRegistrationId, ref dbFlag);
            if (studetTermReg == null) return studentSubjectMarksDto;

            var student = _uofRepository.StudentRepository.GetStudentById(studetTermReg.StudentID, ref dbFlag);
            if (student == null) return studentSubjectMarksDto;

            var subject = _uofRepository.SubjectRepository.GetSubjectByCode(subjectCode, student.SchoolID, ref dbFlag);
            if (subject == null) return studentSubjectMarksDto;

            var marks = _uofRepository.ExamRepository.GetListExamByStudentIdAndSubjectCode(student.SchoolID, (Guid)student.StudentID, subjectCode, ref dbFlag) ?? new List<ExamDto>();

            if (marks == null) return studentSubjectMarksDto;
            
            foreach (var testMark in marks)
            {
                studentSubjectMarksDto.PercentageAscendingByDate.Add(new StudentSubjectMarksByDateDto()
                {
                    Mark = testMark.Mark,
                    Date = testMark.EndOfTermDate
                });
            }

            studentSubjectMarksDto.PercentageAscendingByDate.ToSetPositiveOrNegativeExtensionMarkDateValue();
            
            return studentSubjectMarksDto;
        }


        public StudentSubjectMarksDto GetStudentMarkExamHistoryMarksListByRegNumberAndSchoolIDAndSubjectCode(string regNumber,string subjectCode , Guid schoolID)
        {

            bool dbFlag = false;

            var student = _uofRepository.StudentRepository.GetStudentByRegNumber(regNumber, schoolID, ref dbFlag);
            if (student == null) return null;


            var subject = _uofRepository.SubjectRepository.GetSubjectByCode(subjectCode, schoolID, ref dbFlag);
            if (subject == null) return null;


            var marks = _uofRepository.ExamRepository.GetListExamByStudentIdAndSubjectCode(schoolID ,(Guid)student.StudentID, subjectCode , ref dbFlag) ?? new List<ExamDto>();

            if (marks == null | marks?.Count() <= 0) return null;

            StudentSubjectMarksDto studentSubjectMarksDto = new StudentSubjectMarksDto();
            studentSubjectMarksDto.SubjectCode = subject.SubjectCode;
            studentSubjectMarksDto.SubjectName = subject.SubjectName;
            studentSubjectMarksDto.PercentageAscendingByDate = new List<StudentSubjectMarksByDateDto>();
             
             
            foreach (var testMark in marks)
            {
                studentSubjectMarksDto.PercentageAscendingByDate.Add(new StudentSubjectMarksByDateDto()
                {
                    Mark = testMark.Mark,
                    Date = testMark.EndOfTermDate
                });
            }
            studentSubjectMarksDto.PercentageAscendingByDate = studentSubjectMarksDto.PercentageAscendingByDate.ToSetPositiveOrNegativeExtensionMarkDateValue();

            return studentSubjectMarksDto;
        }


        public List<StudentSubjectMarksDto> GetStudentMarkExamHistoryMarksListByRegNumberAndSchoolID(string regNumber , Guid schoolID)
        {
            
            bool dbFlag = false;

            var student = _uofRepository.StudentRepository.GetStudentByRegNumber(regNumber , schoolID , ref dbFlag);
            if (student == null) return null;
            
            var marks = _uofRepository.ExamRepository.GetListExamByStudentId((Guid)student.StudentID , ref dbFlag) ?? new List<ExamDto>();

            if (marks == null | marks?.Count() <= 0) return null;
            
            List<StudentSubjectMarksDto>  studentSubjectMarksDtos = new List<StudentSubjectMarksDto>();

            var subjects = marks.Select(c => new { Code = c.SubjectCode, Name = c.SubjectName }).Distinct();

            foreach (var su in subjects)
            {
                StudentSubjectMarksDto studentSubjectMarksDto = new StudentSubjectMarksDto();
                studentSubjectMarksDto.SubjectCode = su.Code;
                studentSubjectMarksDto.SubjectName = su.Name;
                studentSubjectMarksDto.PercentageAscendingByDate = new List<StudentSubjectMarksByDateDto>();

                var myMarks = marks.Where(c => c.SubjectCode == su.Code);

                if (myMarks == null | myMarks?.Count() <= 0) return null;
                foreach (var testMark in myMarks)
                {
                    studentSubjectMarksDto.PercentageAscendingByDate.Add(new StudentSubjectMarksByDateDto()
                    {
                        Mark = testMark.Mark,
                        Date = testMark.EndOfTermDate
                    });
                }
                studentSubjectMarksDto.PercentageAscendingByDate = studentSubjectMarksDto.PercentageAscendingByDate.ToSetPositiveOrNegativeExtensionMarkDateValue();
                studentSubjectMarksDtos.Add(studentSubjectMarksDto);
            }
            return studentSubjectMarksDtos;
        }
        
        /// <summary>
        /// / Only alow upto 6 subjects
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="levelId"></param>
        /// <param name="termId"></param>
        /// <param name="subjectIds"></param>
        /// <param name="sbError"></param>
        /// <returns></returns>
        public DataTable GetRankListExamByLevelIDAndTermIDAndSubjectIDs(Guid schoolId, Guid levelId, Guid termId, List<Guid> subjectIds, ref StringBuilder sbError)
        {
            DataTable data = new DataTable();
            data.Columns.Add("RegNumber");
            data.Columns.Add("Name");
            data.Columns.Add("AveragePercentage");
            data.Columns.Add("LevelRank");
            data.Columns.Add("LevelTotal");
            data.Columns.Add("ClassRank");
            data.Columns.Add("ClassTotal");
            data.Columns.Add("SubjectRank" , typeof(List<object>));
            bool dbFlag = false;
            if (subjectIds == null || subjectIds?.Count() <= 0 || subjectIds?.Count() >= 6)
            {
                if (subjectIds == null || subjectIds?.Count() <= 0 )
                {
                    sbError.Append("No subject selected found");
                }
                else if (subjectIds.Count() >= 6)
                {
                    sbError.Append("Only less than six subjects are allowed");
                }
                return data;
            }

            var list = _uofRepository.ExamRepository.GetListByLevelIDandTermIDAndSubjectIDs(schoolId, levelId, termId, subjectIds, ref dbFlag);
            if (list == null || list.Count() <= 0)
            {
                return data;
            }

            var listRank = list.ToClassSubjectRank()
                        .ToLevelSubjectRank()
                        .ToClassOveralRank()
                        .ToLevelOveralRank();

            var uniqueStudent = listRank.Select(c => c.RegNumber).Distinct();
            var uniqueSubject = listRank.Select(c => c.SubjectCode).Distinct();

            foreach (var subject in uniqueSubject)
            {
                data.Columns.Add("SubCode_" + subject); 
            }
            foreach (var studentReg in uniqueStudent)
            {
                var student = listRank.FirstOrDefault(c => c.RegNumber == studentReg);

                if (student == null) continue;

                var studentExams = listRank.Where(c => c.RegNumber == studentReg);
                if (studentExams == null) continue;

                DataRow dataRow = data.NewRow();
                dataRow["RegNumber"] = student.RegNumber;
                dataRow["Name"] = student.FullName;
                dataRow["AveragePercentage"] = student.Mark_OveralAverage;
                dataRow["LevelRank"] = student.Rank_OveralLevel_Position;
                dataRow["LevelTotal"] =  student.Rank_OveralLevel_OutOf;
                dataRow["ClassRank"] = student.Rank_OveralClass_Position ;
                dataRow["ClassTotal"] =  student.Rank_OveralClass_OutOf;

                var listStudentSubjectRank = new List<object>();
                foreach (var exam in studentExams)
                {
                    dataRow["SubCode_" + exam.SubjectCode] = exam.Mark;

                    listStudentSubjectRank.Add(new
                                                {
                                                     subjectCode = exam.SubjectCode ,
                                                     subjectName = exam.SubjectName ,
                                                     mark = exam.Mark,
                                                     subject_Class_Rank_Position = exam.Rank_SubjectClass_Position,
                                                     subject_Class_Rank_OutOf = exam.Rank_SubjectClass_OutOf ,
                                                     subject_Level_Rank_Position = exam.Rank_SubjectLevel_Position,
                                                     subject_Level_Rank_OutOf = exam.Rank_SubjectLevel_OutOf
                                                }
                                               );
                }
                dataRow["SubjectRank"] = listStudentSubjectRank;
                data.Rows.Add(dataRow);
            }
            return data;
        }

        public static int ToOveralAverageExam(List<ExamDto> list)
        {
            if (list == null)
            {
                return 0;
            }
            try
            {
                return Convert.ToInt32(list.Average(c => c.Mark));
            }
            catch
            {
                return 0;
            }
        }

        public List<ExamRankDto> GetRankListExamByClassIDAndTermID(Guid schoolId, Guid classId, Guid termId, List<string> subjectCodeList, ref StringBuilder sbError, bool isShowAllSubject)
        {
            bool dbFlag = false;
            if (subjectCodeList == null || subjectCodeList.Count() <= 0)
            {
                return new List<ExamRankDto>();
            }

            var list = _uofRepository.ExamRepository.GetListByClassIDandTermID(schoolId, classId, termId, ref dbFlag) ?? new List<ExamDto>();

            var newList = new List<ExamDto>();
            foreach (var code in subjectCodeList)
            {
                foreach (var student in list)
                {
                    if (student.SubjectCode.ToLower() == code.ToLower())
                    {
                        newList.Add(student);
                    }
                }
            }
            if (newList == null || newList.Count() <= 0)
            {
                return new List<ExamRankDto>();
            }

            if (isShowAllSubject)
            {
                return list.ToClassSubjectRank()
                           .ToClassOveralRank();
            }
            return newList.ToClassSubjectRank()
                          .ToClassOveralRank();
        }
        
        public DataTable ClassSchoolSheetDataTable(Guid schoolID, Guid classID, Guid termID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var examList = _uofRepository.ExamRepository.GetListByClassIDandTermID(schoolID, classID, termID, ref dbFlag);
            var studentList = _uofRepository.StudentTermRegisterRepository.GetListByClassIDAndTermID(classID, termID, ref dbFlag);

            if (studentList == null || studentList.Count() <= 0)
            {
                return null;
            }
            if (examList == null || examList.Count() <= 0)
            {
                return null;
            }

            var subjectUnique = examList.Select(o => o.SubjectCode).Distinct();

            DataTable finalScoreSheet = new DataTable();
            finalScoreSheet.Columns.Add("RegNumber");
            finalScoreSheet.Columns.Add("StudentTermRegisterID");
            finalScoreSheet.Columns.Add("StudentName");
            finalScoreSheet.Columns.Add("TotalMarks");
            finalScoreSheet.Columns.Add("TotalWritten");
            finalScoreSheet.Columns.Add("StudentAverage");

            foreach (var subject in subjectUnique)
            {
                finalScoreSheet.Columns.Add("SubCode_"+subject);
            }

            foreach (var student in studentList)
            {
                DataRow row = finalScoreSheet.NewRow();
                row["RegNumber"] = student.RegNumber;
                row["StudentTermRegisterID"] = student.StudentTermRegisterID;
                row["StudentName"] = student.StudentName;

                var studentSubjectList = examList.Where(c => c.StudentTermRegisterID == student.StudentTermRegisterID).ToList();
                if (studentSubjectList == null)
                {
                    row["TotalMarks"] = 0;
                    finalScoreSheet.Rows.Add(row);
                    continue;
                }

                decimal totalMark = 0;
                decimal average = 0;
                decimal totalWritten = 0;
                foreach (var mark in studentSubjectList)
                {
                    totalMark += mark.Mark;
                    totalWritten++;
                    row["SubCode_" + mark.SubjectCode] = mark.Mark;
                }

                if (totalWritten != 0)
                {
                    average = decimal.Round(totalMark / totalWritten, 2);
                }
                row["TotalMarks"] = totalMark;
                row["TotalWritten"] = totalWritten;
                row["StudentAverage"] = average;
                finalScoreSheet.Rows.Add(row);
            }
            return finalScoreSheet;
        }

        public DataTable LevelSchoolSheetDataTable(Guid schoolID, Guid levelID, Guid termID, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var examList = _uofRepository.ExamRepository.GetListByLevelIDandTermID(schoolID, levelID, termID, ref dbFlag);
            var studentList = _uofRepository.StudentTermRegisterRepository.GetLevelListByLevelIDAndSchoolIDAndTermID(levelID, schoolID , termID, ref dbFlag);

            if (studentList == null || studentList.Count() <= 0)
            {
                return null;
            }
            if (examList == null || examList.Count() <= 0)
            {
                return null;
            }

            var subjectUnique = examList.Select(o => o.SubjectCode).Distinct();


            DataTable finalScoreSheet = new DataTable();
            finalScoreSheet.Columns.Add("RegNumber");
            finalScoreSheet.Columns.Add("StudentName");
            finalScoreSheet.Columns.Add("ClassName");
            finalScoreSheet.Columns.Add("TotalMarks");
            finalScoreSheet.Columns.Add("TotalWritten");
            finalScoreSheet.Columns.Add("Average");

            foreach (var subject in subjectUnique)
            {
                finalScoreSheet.Columns.Add("SubCode_" + subject);
            }

            foreach (var student in studentList)
            {
                DataRow row = finalScoreSheet.NewRow();
                row["RegNumber"] = student.RegNumber;
                row["StudentName"] = student.StudentName;
                row["ClassName"] = student.ClassName;

                var studentSubjectList = examList.Where(c => c.StudentTermRegisterID == student.StudentTermRegisterID).ToList();
                if (studentSubjectList == null)
                {
                    row["TotalMarks"] = 0;
                    finalScoreSheet.Rows.Add(row);
                    continue;
                }

                decimal totalMark = 0;
                decimal average = 0;
                decimal totalWritten = 0;
                foreach (var mark in studentSubjectList)
                {
                    totalMark += mark.Mark;
                    totalWritten++;
                    row["SubCode_" + mark.SubjectCode] = mark.Mark;
                }

                if (totalWritten != 0)
                {
                    average = totalMark / totalWritten;
                }
                row["TotalMarks"] = totalMark;
                row["TotalWritten"] = totalWritten;
                row["Average"] = average;
                finalScoreSheet.Rows.Add(row);
            }
            return finalScoreSheet;
        } 
        
        public List<StudentTermMarkAverage> GetAllAverageByTermAndSubjectForStudent(Guid studentID)
        {
            bool dbError = false;

            var student = _uofRepository.StudentRepository.GetStudentById(studentID, ref dbError);

            if(student == null)
            {
                return null;
            }
            var terms = _uofRepository.TermRepository.GetListTermBySchoolID(student.SchoolID, ref dbError);

            if(terms == null)
            {
                return null;
            }
            var list = _uofRepository.ExamRepository.GetListByStudentID(studentID  , ref dbError);

            List<StudentTermMarkAverage> report = new List<StudentTermMarkAverage>();

            if (list == null && list?.Count() <= 0)
            {
                return null;
            }
            
            var uniqueTerms = list.Select(c => c.TermID).Distinct();
            var uniqueSubjects = list.Select(c => c.SubjectCode).Distinct();

            foreach (var termId in uniqueTerms)
            {

                Term term = terms.Where(c => c.TermID == termId)?.FirstOrDefault();

                if(term == null) 
                {
                    continue;
                } 
                
                /*
                 All Term Must At Have subject else it does not exist 
                 */
                foreach (var subject in uniqueSubjects)
                {
                    
                    var subjectFullName = list.Where(c => c.SubjectCode == subject).FirstOrDefault();
                    var markTermSubject = list.Where(c => c.SubjectCode == subject && c.TermID == termId)?.FirstOrDefault();

                    StudentTermMarkAverage studentTermMarkAverage = new StudentTermMarkAverage();
                    studentTermMarkAverage.TermFromDate = term.StartDate;
                    studentTermMarkAverage.SubjectCode = subject;
                    studentTermMarkAverage.SubjectName = subjectFullName?.SubjectName ?? subject;
                    studentTermMarkAverage.Average = markTermSubject?.Mark ?? 0;

                    report.Add(studentTermMarkAverage);
                }
            }
            return report;
        }
    }
     
}

