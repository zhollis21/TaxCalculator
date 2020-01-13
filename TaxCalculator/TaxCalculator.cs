using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxCalculator
{
    public class TaxCalculator
    {
        private static List<Bracket> _single2019TaxBrackets;
        private static List<Bracket> _married2019TaxBrackets;
        private bool _isMarried;
        private long _grossIncome;
        private long _spouseGrossIncome;
        private double _taxExemptIncome;

        static TaxCalculator()
        {
            _single2019TaxBrackets = new List<Bracket>()
            {
                new Bracket(9_700, 0.10),
                new Bracket(39_475, 0.12),
                new Bracket(84_200, 0.22),
                new Bracket(160_725, 0.24),
                new Bracket(204_100, 0.32),
                new Bracket(510_300, 0.35),
                new Bracket(long.MaxValue, 0.37)
            };

            _married2019TaxBrackets = new List<Bracket>()
            {
                new Bracket(19_400, 0.10),
                new Bracket(78_950, 0.12),
                new Bracket(168_400, 0.22),
                new Bracket(321_450, 0.24),
                new Bracket(408_200, 0.32),
                new Bracket(612_350, 0.35),
                new Bracket(long.MaxValue, 0.37)
            };
        }

        public void GetFinancialInformationFromUser()
        {
            GetMarriageInformation();

            GetIncomeInformation();

            IdentifyExemptIncome();
        }

        private void IdentifyExemptIncome()
        {
            _taxExemptIncome = 0;

            bool has401k = GetYesOrNoAnswer("Do you have a non-Roth 401k?");
            if (has401k)
            {
                _taxExemptIncome += (GetNonNegativePercent("Enter the contribution percentage") / 100) * _grossIncome;
            }

            if (_isMarried)
            {
                has401k = GetYesOrNoAnswer("Does your spouse have a non-Roth 401k?");
                if (has401k)
                {
                    _taxExemptIncome += (GetNonNegativePercent("Enter the contribution percentage") / 100) * _spouseGrossIncome;
                }
            }

            Console.WriteLine();
        }

        private static bool GetYesOrNoAnswer(string prompt)
        {
            Console.Write($"\n{prompt} ('y' or 'n'): ");

            char answer = Console.ReadLine().ToLower().FirstOrDefault();

            while (answer != 'y' && answer != 'n')
            {
                Console.Write("Invalid Answer... Please respond with 'y' or 'n': ");
                answer = Console.ReadLine().ToLower().FirstOrDefault();
            }

            return answer == 'y';
        }

        private static long GetNonNegativeLong(string prompt)
        {
            Console.Write($"\n{prompt}: ");
            var isValidLong = long.TryParse(Console.ReadLine(), out long answer);

            while (!isValidLong || answer < 0)
            {
                Console.Write("Invalid Answer... Please respond with a valid/non-negative number: ");
                isValidLong = long.TryParse(Console.ReadLine(), out answer);
            }

            return answer;
        }

        private static double GetNonNegativePercent(string prompt)
        {
            Console.Write($"\n{prompt} (0 - 100): ");
            var isValidDouble = double.TryParse(Console.ReadLine(), out double answer);

            while (!isValidDouble || answer < 0 || answer > 100)
            {
                Console.Write("Invalid Answer... Please respond with a valid/non-negative number between 0 and 100: ");
                isValidDouble = double.TryParse(Console.ReadLine(), out answer);
            }

            return answer;
        }

        private void GetMarriageInformation()
        {
            _isMarried = GetYesOrNoAnswer("Are you legally married?");
            Console.WriteLine();
        }

        private void GetIncomeInformation()
        {
            _grossIncome = 0;

            _grossIncome += GetNonNegativeLong("Enter your yearly income");

            if (_isMarried)
            {
                _spouseGrossIncome += GetNonNegativeLong("Enter your spouse's yearly income");
            }
            Console.WriteLine();
        }

        public void CalculateTaxEstimation()
        {
            double untaxedIncome = (_grossIncome + _spouseGrossIncome) - _taxExemptIncome;
            long totalIncome = _grossIncome + _spouseGrossIncome;
            double taxEstimate = 0;

            // Select the correct tax bracket based on marriage status
            var taxBrackets = _isMarried ? _married2019TaxBrackets : _single2019TaxBrackets;

            long previousBracketIncomeLimit = 0;
            foreach (var bracket in taxBrackets)
            {
                long bracketRange = bracket.IncomeLimit - previousBracketIncomeLimit;

                double applicableUntaxedIncome;

                // How much income are we taxing in this bracket?
                // If we have more untaxedIncome than the bracketRange, we just tax the bracket range
                // If we have less untaxedIncome than the bracketRange, we only tax the remaining untaxedIncome
                applicableUntaxedIncome = Math.Min(untaxedIncome, bracketRange);

                var taxForThisBracket = applicableUntaxedIncome * bracket.TaxRate;
                Console.WriteLine(
                    $"{applicableUntaxedIncome} taxed at a rate of {Math.Round(bracket.TaxRate * 100, 2)}% " +
                    $"for a total of {taxForThisBracket} in taxes.");

                taxEstimate += taxForThisBracket;
                untaxedIncome -= applicableUntaxedIncome;

                if (untaxedIncome <= 0)
                {
                    break;
                }

                previousBracketIncomeLimit = bracket.IncomeLimit;
            }

            Console.WriteLine(
                $"\nTotal Gross Income: ${totalIncome}" +
                $"\nTotal Tax Exempt Income: ${Math.Round(_taxExemptIncome, 2)}" +
                $"\nTotal Net Income: ${Math.Round((totalIncome - taxEstimate) - _taxExemptIncome, 2)}" +
                $"\nTotal Taxes: ${Math.Round(taxEstimate, 2)}" +
                $"\nAverage Percent Taxed: {Math.Round((taxEstimate / (totalIncome - _taxExemptIncome) * 100), 2)}%");
        }
    }
}