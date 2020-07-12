using Newtonsoft.Json.Converters;

namespace iGrade.Domain.DateFormater
{
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
