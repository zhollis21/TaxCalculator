using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxCalculator
{
    public class TaxCalculator
    {
        private static Dictionary<long, double> _single2019TaxBrackets;
        private static Dictionary<long, double> _married2019TaxBrackets;
        private bool _isMarried;
        private long _grossIncome;

        static TaxCalculator()
        {
            _single2019TaxBrackets = new Dictionary<long, double>()
            {
                { 9_700, 0.10 },
                { 39_475, 0.12 },
                { 84_200, 0.22 },
                { 160_725, 0.24 },
                { 204_100, 0.32 },
                { 510_300, 0.35 },
                { long.MaxValue, 0.37 }
            };

            _married2019TaxBrackets = new Dictionary<long, double>()
            {
                { 19_400, 0.10 },
                { 78_950, 0.12 },
                { 168_400, 0.22 },
                { 321_450, 0.24 },
                { 408_200, 0.32 },
                { 612_350, 0.35 },
                { long.MaxValue, 0.37 }
            };
        }

        public void GetFinancialInformationFromUser()
        {
            GetMarriageInformation();

            GetIncomeInformation();
        }

        private void GetMarriageInformation()
        {
            Console.Write("\nAre you legally married? ('y' or 'n'): ");
            char marriedAnswer = Console.ReadLine().ToLower().FirstOrDefault();

            while (marriedAnswer != 'y' && marriedAnswer != 'n')
            {
                Console.Write("Invalid Answer... Please respond with 'y' or 'n': ");
                marriedAnswer = Console.ReadLine().ToLower().FirstOrDefault();
            }

            _isMarried = marriedAnswer == 'y';
        }

        private void GetIncomeInformation()
        {
            _grossIncome = 0;

            Console.Write("\nEnter your yearly income: ");
            var isValidLong = long.TryParse(Console.ReadLine(), out long userIncome);

            while (!isValidLong || userIncome < 1)
            {
                Console.Write("Invalid Answer... Please respond with a valid positive number: ");
                isValidLong = long.TryParse(Console.ReadLine(), out userIncome);
            }

            _grossIncome += userIncome;

            if (_isMarried)
            {
                Console.Write("\nEnter your spouse's yearly income: ");
                isValidLong = long.TryParse(Console.ReadLine(), out long spouseIncome);

                while (!isValidLong || spouseIncome < 1)
                {
                    Console.Write("Invalid Answer... Please respond with a valid positive number: ");
                    isValidLong = long.TryParse(Console.ReadLine(), out spouseIncome);
                }

                _grossIncome += spouseIncome;
            }
        }

        public void CalculateTaxEstimation()
        {
            
        }
    }
}
