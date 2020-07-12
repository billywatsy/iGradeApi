using Dapper;
using iGrade.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Repository
{
    public class SchoolInformationRepository : BaseRepository
    {

        public bool Save(SchoolInformation school, string modifiedby , ref bool dbError)
        {
            try
            { 
                using (var connection = GetConnection())
                {

                    var update = @"
                                UPDATE SchoolInformation
   SET  RoadName = @RoadName , Street = @Street , City = @City , Country = @Country , Lattitude = @Lattitude , Longitude = @Longitude ,
      Phone = @Phone ,  Email = @Email  , lastmodifiedby = @modifiedby
 WHERE SchoolID = @SchoolID   AND IsDeleted IS NULL
                              ";
                    var id = connection.Execute(update,
                                 new
                                 {
                                     SchoolID = school.SchoolId,
                                     RoadName = school.RoadName,
                                     Street = school.Street,
                                     City = school.City,
                                     Country = school.Country,
                                     Lattitude = school.Lattitude,
                                     Longitude = school.Longitude,
                                     Phone = school.Phone,
                                     Email = school.Email ,
                                     modifiedby = modifiedby
                                 });

                    if (id <= 0)
                    { 
var insert = @"
                                INSERT INTO SchoolInformation
           (SchoolID
           ,RoadName
           ,Street
           ,City
           ,Country
           ,Lattitude
           ,Longitude
           ,Phone
           ,Email  , lastmodifiedby
        )
     VALUES
           (@SchoolID
           ,@RoadName 
           ,@Street 
           ,@City 
           ,@Country 
           ,@Lattitude 
           ,@Longitude
           ,@Phone
           ,@Email , @modifiedby)
                                ";
                        var idsss = connection.Execute(insert,
                                     new
                                     {
                                         SchoolID = school.SchoolId,
                                         RoadName = school.RoadName,
                                         Street = school.Street,
                                         City = school.City,
                                         Country = school.Country,
                                         Lattitude = school.Lattitude,
                                         Longitude = school.Longitude,
                                         Phone = school.Phone,
                                         Email = school.Email
                                     });
                        return true;
                    }

                }
                return true;
            }
            catch (Exception er)
            {
                dbError = true;
                DbLog.Error(er);
                return false;
            }
        }


        public SchoolInformation GetById(Guid schoolId, ref bool dbFlag)
        {
            var sql = @"SELECT * FROM SchoolInformation  
                        WHERE SchoolId = @schoolId AND ISdeleted is null";

            using (var connection = GetConnection())
            {
                var list = connection.Query<SchoolInformation>(sql,
                    new { schoolId = schoolId })
                                 .FirstOrDefault();
                return list;
            }
        } 
    }
}
