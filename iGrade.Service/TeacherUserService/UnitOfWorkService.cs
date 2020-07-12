using iGrade.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Core.TeacherUserService
{
    public class UnitOfWorkService
    { 
        private StudentService _studentService; 
        private ExamService _examService; 
        private TestMarkService _testMarkService; 
        private StudentTermRegisterService _studentTermRegisterServiceService; 
        private SettingService _settingService; 
        
        public UnitOfWorkService(Domain.Dto.LoggedUser user , UowRepository uowRepository)
        {
            _studentService = new StudentService(user , uowRepository);
            _examService = new ExamService(user , uowRepository);
            _testMarkService = new TestMarkService(user , uowRepository);
            _studentTermRegisterServiceService = new StudentTermRegisterService(user , uowRepository);
            _settingService = new SettingService(user , uowRepository); 
        }
        public StudentService StudentService { get { return _studentService; } }
        public ExamService ExamService { get { return _examService; } }
        public TestMarkService TestMarkService { get { return _testMarkService; } }
        public StudentTermRegisterService StudentTermRegisterService { get { return _studentTermRegisterServiceService; } }
        public SettingService SettingService { get { return _settingService; } }
    }
}
