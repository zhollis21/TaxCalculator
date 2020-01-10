using System;
using System.Collections.Generic;
using System.Text;

namespace TaxCalculator
{
    public class Bracket
    {
        public Bracket(long incomeLimit, double taxRate)
        {
            IncomeLimit = incomeLimit;

            TaxRate = taxRate;
        }

        public long IncomeLimit { get; set; }

        public double TaxRate { get; set; }
    }
}