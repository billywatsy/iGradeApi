using iGrade.Repository;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Core.TeacherUserService
{
    public class DepartmentService
    {
        private UowRepository _uofRepository;
        private iGrade.Domain.Dto.LoggedUser _user;
        public DepartmentService(iGrade.Domain.Dto.LoggedUser user, UowRepository uofRepository)
        {
            _uofRepository = uofRepository;
            _user = user; 
        }

        public Department GetDepartment(Guid departmentId)
        {
            bool dbFlag = false;
            var list = _uofRepository.DepartmentRepository.GetDepartment(departmentId, ref dbFlag);
            return list;
        }
        
        public List<Department> GetDepartments()
        {
            bool dbFlag = false;
            var list = _uofRepository.DepartmentRepository.GetListDepartments(_user.SchoolID, ref dbFlag);
            return list;
        }

        public Department Save(Department department , ref StringBuilder sbError)
        {
            bool dbFlag = false;

            if (department.DepartmentId == null || department.DepartmentId == Guid.Empty)
            {
                department.SchoolID = _user.SchoolID;
            }
            else
            {
                var isLevelFromSchool = _uofRepository.DepartmentRepository.GetDepartment((Guid)department.DepartmentId, ref dbFlag);

                if (isLevelFromSchool.DepartmentId != department?.DepartmentId)
                {
                    sbError.Append("department not from school");
                    return null;
                }
                department.SchoolID = isLevelFromSchool.SchoolID;

            }

            var list = _uofRepository.DepartmentRepository.GetListDepartments(_user.SchoolID, ref dbFlag);

            if(list.Count() > 100)
            {
                sbError.Append("You have reached maximum departments allowed");
            }
            else
            {
                if (!string.IsNullOrEmpty(department.DepartmentId.ToString()))
                {
                    var dbDepartment = list.Where(c => c.DepartmentId == department.DepartmentId).FirstOrDefault();
                    if (dbDepartment == null)
                    {
                        sbError.Append("Department does not exist for school");
                        return null;
                    }
                    else
                    {
                        department.Code = dbDepartment.Code;
                        department.SchoolID = _user.SchoolID;
                    }
                }
                else
                {
                    department.SchoolID = _user.SchoolID;
                }
            }
            var isSaved = _uofRepository.DepartmentRepository.Save(department , _user.Username, ref dbFlag);

            return isSaved;
        }

        public bool Delete(Guid departmentId, ref StringBuilder sbError)
        {
            bool dbFlag = false;
            var department = _uofRepository.DepartmentRepository.GetDepartment(departmentId, ref dbFlag);

            if (department == null)
            {
                sbError.Append("department Does not Exist");
                return false;
            }

            if (department.SchoolID != _user.SchoolID)
            {
                sbError.Append("Error department does not exist for school");
                return false;
            }

            return _uofRepository.DepartmentRepository.Delete((Guid)department.DepartmentId, _user.Username, ref dbFlag);
        }
    }
}
 