using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iGrade.Domain.Dto
{
    public class SmsSendDataDto
    {
        public string Title { get; set; }
        public string To { get; set; }
        public string SmsText { get; set; }
        public string SentBy { get; set; }
        public DateTime DateSmsShouldBeDelivered { get; set; }
        public int CountNumberOfSms { get; set; }
    }
}
