using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OffSite.Web.Utils
{
    public class Helpers
    {
        public static IDictionary<string, string> GetFormData(IFormCollection items)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>();

            var relatedData = items.Keys;
            foreach (var item in relatedData)
            {
                var tets = item;
            }
            //for (int i = 0; i < relatedData.Count; i++)
            //{
            //    var test = items.GetValues(relatedData[i]);
            //    if (test[0] != null)
            //    {
            //        formData.Add(relatedData[i], test[0]);
            //    }
            //}

            return formData;
        }

        public static int GetWorkingDays(DateTime from, DateTime to)
        {
            if (from > to)
            {
                // Todo exception...
                return -1;
            }

            int dayDifference = to.Subtract(from).Days + 1;
            return Enumerable
                .Range(1, dayDifference)
                .Select(x => from.AddDays(x))
                .Count(x => x.DayOfWeek != DayOfWeek.Saturday && x.DayOfWeek != DayOfWeek.Sunday);
        }
    }
}
