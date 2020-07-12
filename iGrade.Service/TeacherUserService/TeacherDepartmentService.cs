using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iGrade.Domain.Dto;

namespace iGrade.Core.TeacherUserService
{
    public class TeacherDepartmentService : BaseService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public TeacherDepartmentService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public List<TeacherDepartmentDto> GetListBySchoolID(ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherDepartmentRepository.GetListBySchoolID(_user.SchoolID, ref dbFlag);
            return list;
        }

        public List<TeacherDepartmentDto> GetListByDepartmentID(Guid departmentId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherDepartmentRepository.GetListByDepartmentID(departmentId, ref dbFlag);
            return list;
        }
        
        public List<TeacherDepartmentDto> GetListByTeacherID(Guid teacherId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var list = _uofRepository.TeacherDepartmentRepository.GetListByTeacherId(teacherId, ref dbFlag);
            return list;
        }

        
        public bool Save(TeacherDepartment teacherDepartment , ref StringBuilder sbError )
        {
            if(teacherDepartment == null )
            {
                sbError.Append("No data found");
                return false;
            }


            bool dbFlag = false;
            var department = _uofRepository.DepartmentRepository.GetDepartment(teacherDepartment.DepartmentId, ref dbFlag);
            if (department == null)
            {
                sbError.Append("Department does not exist");
                return false;
            }

            var teacher = _uofRepository.TeacherRepository.GetTeacherById(teacherDepartment.TeacherId, ref dbFlag);
            if (teacher == null)
            {
                sbError.Append("Teacher does not exist");
                return false;
            }

            if(department.SchoolID != teacher.SchoolID)
            {
                sbError.Append("department and teacher mismatch");
                return false;
            }
            else if(_user.SchoolID != teacher.SchoolID)
            {
                sbError.Append("school and teacher mismatch");
                return false;
            }

            var save = _uofRepository.TeacherDepartmentRepository.Save(teacherDepartment,_user.Username, ref dbFlag);
            return save;
        }

		 
         
        public bool Delete(Guid teacherDepartmentId  , ref StringBuilder sbError)
        {
            bool dbFlag = false;

            return _uofRepository.TeacherDepartmentRepository.Delete(teacherDepartmentId, _user.Username, ref dbFlag);     
        }
      
    }
}
 