using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order.Models
{
    public class CostCsvDataModel
    {
        public string cnst { get; set; }
        public string mnth { get; set; }
        public string profit { get; set; }

        public CostCsvDataModel(string f, string s, string th)
        {
            cnst = $"Разовые расходы: {f}";
            mnth = $"Расходы в месяц: {s}";
            profit = $"Окупится за {th} месяцев";
        }
    }
}
