using System;
using System.Collections.Generic;
using System.Text;

namespace iGrade.Core.TeacherUserService
{
    public class BaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EntityType"></param>
        /// <param name="teacherId"></param>
        /// <param name="details"></param>
        /// <param name="crudeType">DELETE SAVED UPDATE INSERT</param>
        public void Log(string EntityType ,Guid teacherId , string details , string crudeType )
        {
            try
            {
                bool dbError = false;
                Repository.LogRepository.SaveActivity(
                        new Domain.Log(){
                            TeacherID = teacherId,
                            EntityType = EntityType,
                            ActionDetails = details,
                            ActionType = crudeType
                        } , ref dbError);
            }
            catch (Exception er)
            {

            }
        }
    }
}
