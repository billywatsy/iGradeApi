using iGrade.Domain.Dto;
using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iGrade.Reporting.Domain;
using iGrade.Domain;
using System.Data;
using iGrade.Reporting.Extension;

namespace iGrade.Reporting.Service
{
   public  class TestReport { 
    private UowRepository _uofRepository;
    public TestReport(UowRepository uowRepository)
    {
        _uofRepository = uowRepository;
    } 

    public List<TestMarkDto> GetTestMarkListTestMarkByStudentIdAndTermId(Guid studentId, Guid termID, ref StringBuilder sbError)
    {
        bool dbEror = false;
        var student = _uofRepository.StudentTermRegisterRepository.GetByStudentIDAndTermId(studentId, termID, ref dbEror);
        if (student == null)
        {
            return null;
        }
        var studentInformation = _uofRepository.StudentTermRegisterRepository.GetByEnrollemtID((Guid)student.StudentTermRegisterID, ref dbEror);
        var term = _uofRepository.TermRepository.GetByID(termID, ref dbEror);

        if (studentInformation == null)
        {
            return null;
        }

        if (term == null)
        {
            return null;
        }

        var tests = _uofRepository.TestMarkRepository.GetListTestMarksByStudentTermRegisterIDAndTermID((Guid)studentInformation.StudentTermRegisterID, termID, ref dbEror);
        if (tests == null)
        {
            return null;
        }
        return tests;
    }
        public static List<Domain.TestMarkAverage> ToAverageTestMark(List<TestMarkDto> list)
        {
            if (list == null)
            {
                return new List<Domain.TestMarkAverage>();
            }
            List<TestMarkAverage> report = new List<TestMarkAverage>();
            var uniqueSubject = list.Select(c => c.SubjectCode).Distinct();
            foreach (var mark in uniqueSubject)
            {
                var testList = list.Where(c => c.SubjectCode == mark);
                var testSubject = testList.FirstOrDefault();

                TestMarkAverage row = new TestMarkAverage();
                row.SubjectCode = testSubject.SubjectCode;
                row.SubjectName = testSubject.SubjectName;
                row.TestWritten = testList?.Count() ?? 0;

                var testAverageValue = 0;


                if (row.TestWritten > 0)
                {
                    testAverageValue = Convert.ToInt32(testList.Average(c => c.MarkPercentage));
                }

                row.TestAverage = testAverageValue;

                try
                {
                    row.TestPassed = testList?.Where(c => c.MarkPercentage >= 50M)?.Count() ?? 0;
                }
                catch
                {
                }

                try
                {
                    row.TestFailed = testList?.Where(c => c.MarkPercentage < 50M)?.Count() ?? 0;
                }
                catch
                {

                }
                report.Add(row);
            }
            return report;
        }

        public static int ToOveralAverageTestMark(List<TestMarkDto> list)
        {
            if (list == null)
            {
                return 0;
            }
            try
            {
                return Convert.ToInt32(list.Average(c => c.MarkPercentage));
            }
            catch
            {
                return 0;
            }
        }

        public StudentSubjectMarksDto GetTestMarkBottomStudentSubjectHistory(Guid studentTermRegisterId, string subjectCode)
        {
            StudentSubjectMarksDto studentSubjectMarksDto = new StudentSubjectMarksDto();

            bool dbFlag = false;

            var studentTermRegister = _uofRepository.StudentTermRegisterRepository.GetByEnrollemtID(studentTermRegisterId, ref dbFlag);
            if (studentTermRegister == null) return studentSubjectMarksDto;

            var student = _uofRepository.StudentRepository.GetStudentById(studentTermRegister.StudentID, ref dbFlag);
            if (student == null) return studentSubjectMarksDto;

            var subject = _uofRepository.SubjectRepository.GetSubjectByCode(subjectCode, student.SchoolID, ref dbFlag);
            if (subject == null) return studentSubjectMarksDto;
            
            studentSubjectMarksDto.PercentageAscendingByDate = new List<StudentSubjectMarksByDateDto>();

            var marks = _uofRepository.TestMarkRepository.GetListTestMarksStudentIDAndSubjectIDLatest100((Guid)student.StudentID, (Guid)subject.SubjectID, ref dbFlag)?.OrderByDescending(c => c.TestDateCreated)?.ToList() ?? new List<TestMarkDto>() ;
            if (marks == null) return studentSubjectMarksDto;

            foreach (var testMark in marks)
            {
                studentSubjectMarksDto.PercentageAscendingByDate.Add(new StudentSubjectMarksByDateDto()
                {
                    Mark = testMark.MarkPercentage, 
                    Date = testMark.TestDateCreated
                });
            }

            studentSubjectMarksDto.PercentageAscendingByDate = studentSubjectMarksDto.PercentageAscendingByDate.ToSetPositiveOrNegativeExtensionMarkDateValue();

            studentSubjectMarksDto.PercentageAscendingByDate = studentSubjectMarksDto.PercentageAscendingByDate.OrderBy(c => c.Date).ToList();
            return studentSubjectMarksDto;
        }

        public DataTable GetClassTestAverages(Guid classId , Guid termId)
        {
            bool dbFlag = false;
            var classObj = _uofRepository.ClassRepository.GetClassByID(classId, ref dbFlag);

            var termObj = _uofRepository.TermRepository.GetByID(termId, ref dbFlag);


            var tests = _uofRepository.TestMarkRepository.GetListTestMarksByClassIdAndTermID(classId, termId, ref dbFlag);

            

            var uniqueStudents = tests.Select(c => c.RegNumber).Distinct();
            var uniqueSubjectCode = tests.Select(c => c.SubjectCode).Distinct();

            
            DataTable finalScoreSheet = new DataTable();
            finalScoreSheet.Columns.Add("RegNumber");
            finalScoreSheet.Columns.Add("StudentName");
            finalScoreSheet.Columns.Add("ClassName");
            finalScoreSheet.Columns.Add("TotalAverage");
            finalScoreSheet.Columns.Add("ListOfTests" , typeof(List<StudentSubjectMarksDto>));

            foreach (var subject in uniqueSubjectCode)
            {
                finalScoreSheet.Columns.Add("SubCode_" + subject);
            }

            foreach (var regNumber in uniqueStudents)
            {

                var student = tests.FirstOrDefault(c => c.RegNumber == regNumber);

                DataRow row = finalScoreSheet.NewRow();
                row["RegNumber"] = student.RegNumber;
                row["StudentName"] = student.FullName;
                row["ClassName"] = student.ClassName;

                var studentSubjectList = tests.Where(c => c.RegNumber == student.RegNumber).ToList();
                if (studentSubjectList == null)
                {
                    row["TotalAverage"] = 0;
                    finalScoreSheet.Rows.Add(row);
                    continue;
                }

                var uniqueStudentSubject = studentSubjectList.Select(c => new { SubjectCode = c.SubjectCode , SubjectName = c.SubjectName }).Distinct();
                if (uniqueStudentSubject == null)
                {
                    row["TotalAverage"] = 0;
                    finalScoreSheet.Rows.Add(row);
                    continue;
                }

                List<StudentSubjectMarksDto> studentSubjectMarksDtos = new List<StudentSubjectMarksDto>();
                List<int> totalAverage = new List<int>();
                decimal totalWritten = 0;

                foreach (var studentSubject in uniqueStudentSubject)
                {
                    StudentSubjectMarksDto studentSubjectMarksDto = new StudentSubjectMarksDto();
                    studentSubjectMarksDto.SubjectCode = studentSubject.SubjectCode;
                    studentSubjectMarksDto.SubjectName = studentSubject.SubjectName;
                    studentSubjectMarksDto.PercentageAscendingByDate = new List<StudentSubjectMarksByDateDto>();

                    var subjectsMarks = studentSubjectList.Where(c => c.SubjectCode == studentSubject.SubjectCode);

                    if(subjectsMarks != null && subjectsMarks?.Count() >= 1)
                    {
                        foreach (var testMark in subjectsMarks)
                        {
                            studentSubjectMarksDto.PercentageAscendingByDate.Add(new StudentSubjectMarksByDateDto()
                            {
                                Mark = testMark.MarkPercentage,
                                Date = testMark.TestDateCreated
                            });
                        }
                        studentSubjectMarksDto.PercentageAscendingByDate = studentSubjectMarksDto.PercentageAscendingByDate.ToSetPositiveOrNegativeExtensionMarkDateValue();
                        var subjectAverage = Convert.ToInt32(subjectsMarks.Average(c => c.MarkPercentage));
                        totalAverage.Add(subjectAverage);
                        row["SubCode_" + studentSubject.SubjectCode] = subjectAverage;

                        if(subjectAverage >= 1)
                        {
                            totalWritten++;
                        } 
                    }
                    studentSubjectMarksDtos.Add(studentSubjectMarksDto);
                }

                row["ListOfTests"] = studentSubjectMarksDtos;

                if (totalWritten >= 1)
                {
                    row["TotalAverage"] = Convert.ToInt32(totalAverage.Average());
                } 
                finalScoreSheet.Rows.Add(row);
            }

            return finalScoreSheet;


        }

    }
}

