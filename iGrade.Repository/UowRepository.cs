using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Repository
{
    public class UowRepository
    { 
        private AdminRepository _adminRepository;
        private AbsentFromSchoolRepository _absentRepository;
        private AbsentFromLessonRepository _absentLessonRepository; 
        private ClassRepository _classRepository;
        private ClassTeacherRepository _classTeacherRepository;
        private DashboardRepository _dashboardRepository;
        private DepartmentRepository _departmentRepository;
        private CommitteMemberRepository _committeMemberRepository;
        private StudentTermRegisterRepository _studentTermRegisterRepositoryRepository; 
        private ExamRepository _examRepository;
        private EmailSmsRepository _emailSmsRepository;
        private FeeTermRepository _feeTermRepository;
        private FeeTypeRepository _feeTypeRepository;
        private GradeRepository _gradeRepository;
        private GradeMarkRepository _gradeMarkRepository; 
        private LessonPlanRepository _lessonPlanRepository; 
        private LessonPlanCommentRepository _lessonPlanCommentRepository; 
        private ParentRepository _parentRepository;
        private StudentRepository _studentRepository;
        private SubjectRepository _subjectRepository;
        private SchoolGroupRepository _schoolGroupRepository;
        private SchoolRepository _schoolRepository;
        private SubscriptionRepository _subscribeRepository;
        private StudentTermReviewRepository _studentTermReviewRepository;
        private SchoolInformationRepository _schoolInformationRepository;
        private TeacherDepartmentRepository _teacherDepartmentRepository;
        private SettingRepository _settingRepository;
        private TeacherRepository _teacherRepository;
        private TeacherClassSubjectRepository _teacherClassSubjectRepository;
        private TeacherClassSubjectFileTypeRepository _teacherClassSubjectFileTypeRepository;
        private TeacherClassSubjectFileRepository _teacherClassSubjectFileRepository;
        private TestRepository _testRepository;
        private TestMarkRepository _testMarkRepository;
        private TermRepository _termRepository;
        private LevelRepository _levelRepository;
        private LogRepository _logRepository;
		
        public UowRepository()
        {
            Init();
        }

        private void Init()
        {
            _adminRepository = _adminRepository ?? new AdminRepository();
            _absentRepository = _absentRepository ?? new AbsentFromSchoolRepository();
            _absentLessonRepository = _absentLessonRepository ?? new AbsentFromLessonRepository();
            _classRepository = _classRepository ?? new ClassRepository();
            _classTeacherRepository = _classTeacherRepository ?? new ClassTeacherRepository();
            _committeMemberRepository = _committeMemberRepository ?? new CommitteMemberRepository();
            _dashboardRepository = _dashboardRepository ?? new DashboardRepository();
            _departmentRepository = _departmentRepository ?? new DepartmentRepository();
            _studentTermRegisterRepositoryRepository = _studentTermRegisterRepositoryRepository ?? new StudentTermRegisterRepository();
            _examRepository = _examRepository ?? new ExamRepository();
            _emailSmsRepository = _emailSmsRepository ?? new EmailSmsRepository();
            _feeTypeRepository = _feeTypeRepository ?? new FeeTypeRepository();
            _feeTermRepository = _feeTermRepository ?? new FeeTermRepository();
            _gradeRepository = _gradeRepository ?? new GradeRepository(); 
            _gradeMarkRepository = _gradeMarkRepository ?? new GradeMarkRepository(); 
            _lessonPlanRepository = _lessonPlanRepository ?? new LessonPlanRepository();
            _lessonPlanCommentRepository = _lessonPlanCommentRepository ?? new LessonPlanCommentRepository();
            _parentRepository = _parentRepository ?? new ParentRepository(); 
            _studentRepository = _studentRepository ?? new StudentRepository();
            _subjectRepository = _subjectRepository ?? new SubjectRepository();
            _schoolRepository = _schoolRepository ?? new SchoolRepository();
            _subscribeRepository = _subscribeRepository ?? new SubscriptionRepository();
            _settingRepository = _settingRepository ?? new SettingRepository();
            _studentTermReviewRepository = _studentTermReviewRepository ?? new StudentTermReviewRepository();
            _schoolGroupRepository = _schoolGroupRepository ?? new SchoolGroupRepository();
            _schoolInformationRepository = _schoolInformationRepository ?? new SchoolInformationRepository();
            _teacherDepartmentRepository = _teacherDepartmentRepository ?? new TeacherDepartmentRepository();
            _teacherRepository = _teacherRepository ?? new TeacherRepository();
            _teacherClassSubjectRepository = _teacherClassSubjectRepository ?? new TeacherClassSubjectRepository();
            _teacherClassSubjectFileTypeRepository = _teacherClassSubjectFileTypeRepository ?? new TeacherClassSubjectFileTypeRepository();
            _teacherClassSubjectFileRepository = _teacherClassSubjectFileRepository ?? new TeacherClassSubjectFileRepository();
            _testRepository = _testRepository ?? new TestRepository();
            _testMarkRepository = _testMarkRepository ?? new TestMarkRepository();
            _termRepository = _termRepository ?? new TermRepository();
            _levelRepository = _levelRepository ?? new LevelRepository();
            _logRepository = _logRepository ?? new LogRepository();
        }

        public AdminRepository AdminRepository { get { return _adminRepository; } }
        public AbsentFromSchoolRepository AbsentFromSchoolRepository { get { return _absentRepository; } }
        public AbsentFromLessonRepository AbsentFromLesson { get { return _absentLessonRepository; } }
        public ClassRepository ClassRepository { get { return _classRepository; } }
        public CommitteMemberRepository CommitteMemberRepository { get { return _committeMemberRepository; } }
        public ClassTeacherRepository ClassTeacherRepository { get { return _classTeacherRepository; } }
        public DashboardRepository DashboardRepository { get { return _dashboardRepository; } }
        public DepartmentRepository DepartmentRepository { get { return _departmentRepository; } }
        public StudentTermRegisterRepository StudentTermRegisterRepository { get { return _studentTermRegisterRepositoryRepository; } }
        public ExamRepository ExamRepository { get { return _examRepository; } } 
        public EmailSmsRepository EmailSmsRepository { get { return _emailSmsRepository; } }
        public FeeTermRepository FeeTermRepository { get { return _feeTermRepository; } }
        public FeeTypeRepository FeeTypeRepository { get { return _feeTypeRepository; } }
        public GradeRepository GradeRepository { get { return _gradeRepository; } }
        public GradeMarkRepository GradeMarkRepository { get { return _gradeMarkRepository; } } 
        public LessonPlanRepository LessonPlanRepository { get { return _lessonPlanRepository; } }
        public LessonPlanCommentRepository LessonPlanCommentRepository { get { return _lessonPlanCommentRepository; } }
        public ParentRepository ParentRepository { get { return _parentRepository; } }
        public SettingRepository SettingRepository { get { return _settingRepository; } }
        public StudentRepository StudentRepository { get { return _studentRepository; } }
        public SubjectRepository SubjectRepository { get { return _subjectRepository; } }
        public SchoolRepository SchoolRepository { get { return _schoolRepository; } }
        public SubscriptionRepository SubscribeRepository { get { return _subscribeRepository; } }
        public SchoolGroupRepository SchoolGroupRepository { get { return _schoolGroupRepository; } }
        public SchoolInformationRepository SchoolInformationRepository { get { return _schoolInformationRepository; } }
        public StudentTermReviewRepository StudentTermReviewRepository { get { return _studentTermReviewRepository; } }
        public TeacherDepartmentRepository TeacherDepartmentRepository { get { return _teacherDepartmentRepository; } }
        public TeacherRepository TeacherRepository { get { return _teacherRepository; } }
        public TeacherClassSubjectRepository TeacherClassSubjectRepository { get { return _teacherClassSubjectRepository; } }
        public TeacherClassSubjectFileTypeRepository TeacherClassSubjectFileTypeRepository { get { return _teacherClassSubjectFileTypeRepository; } }
        public TeacherClassSubjectFileRepository TeacherClassSubjectFileRepository { get { return _teacherClassSubjectFileRepository; } }
        public TestRepository TestRepository { get { return _testRepository; } }
        public TestMarkRepository TestMarkRepository { get { return _testMarkRepository; } }
        public TermRepository TermRepository { get { return _termRepository; } }
        public LevelRepository LevelRepository { get { return _levelRepository; } }        
        public LogRepository LogRepository { get { return _logRepository; } }        
    }
}
