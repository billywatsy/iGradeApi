using Newtonsoft.Json;
using System; 
namespace iGrade.Api.Controllers.TeacherUserApi.Model
{
    public class SettingForm
    {
        [JsonProperty("exam")]
        public DateTime Exam { get; set; }
        [JsonProperty("test")]
        public DateTime Test { get; set; }
    }
}
