using iGrade.Reporting.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace iGrade.Reporting.Extension
{
    public static class ToPositiveOrNegativeExtensionMarkDate
    {
        public static List<StudentSubjectMarksByDateDto> ToSetPositiveOrNegativeExtensionMarkDateValue(this List<StudentSubjectMarksByDateDto> list)
        {

            /*
                23   23 May
                2    24 May -21
                4    25     +2
            
             */ 
            int totalItems = list?.Count() ?? 1;
            if (list == null || totalItems <= 1)
            {
                return list;
            }

            list = list.OrderBy(c => c.Date).ToList();
            
            int index = 0;
            foreach (var mark in list)
            {
                
                if (index <= 0)
                {
                }
                else
                {
                    mark.ValueDifferenceFromPreviosMark = mark.Mark - (list[index - 1].Mark);
                }
                index++;
            }
            var ordr = list.OrderByDescending(c => c.Date).ToList(); 
            return ordr;
        }
    }
}
