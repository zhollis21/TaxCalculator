using System;
using System.Collections.Generic;
using System.Linq;

namespace TaxCalculator
{
    public class TaxCalculator
    {
        private static readonly List<Bracket> _single2019TaxBrackets;
        private static readonly List<Bracket> _married2019TaxBrackets;
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

        public void CalculateTaxEstimation()
        {
            double untaxedIncome = (_grossIncome + _spouseGrossIncome) - _taxExemptIncome;
            long totalIncome = _grossIncome + _spouseGrossIncome;
            double taxEstimate = 0;

            // Select the correct tax bracket based on marriage status
            var taxBrackets = _isMarried ? _married2019TaxBrackets : _single2019TaxBrackets;

            // Used to determine the range of a tax bracket by subtracting 

            // the current income limit from the previous income limit.
            long previousBracketIncomeLimit = 0;

            foreach (var bracket in taxBrackets)
            {
                // Calculate the range of income that can be taxed under this bracket
                long bracketRange = bracket.IncomeLimit - previousBracketIncomeLimit;
                
                // If the user has less untaxed income than the bracket range, 
                // then we can only tax however much untaxed income they have left

                double applicableUntaxedIncome = Math.Min(untaxedIncome, bracketRange);

                var taxForThisBracket = applicableUntaxedIncome * bracket.TaxRate;
                Console.WriteLine(
                    $"{applicableUntaxedIncome:C} taxed at a rate of {bracket.TaxRate:P0} " +
                    $"for a total of {taxForThisBracket:C} in taxes.");

                taxEstimate += taxForThisBracket;
                untaxedIncome -= applicableUntaxedIncome;

                if (untaxedIncome <= 0)
                {
                    break;
                }

                previousBracketIncomeLimit = bracket.IncomeLimit;
            }

            Console.WriteLine(
                $"\nTotal Gross Income: {totalIncome:C}" +
                $"\nTotal Tax Exempt Income: {_taxExemptIncome:C}" +
                $"\nTotal Net Income: {((totalIncome - taxEstimate) - _taxExemptIncome):C}" +
                $"\nTotal Taxes: {taxEstimate:C}" +
                $"\nAverage Percent Taxed: {(taxEstimate / (totalIncome - _taxExemptIncome)):P}");
        }

        private void IdentifyExemptIncome()
        {
            _taxExemptIncome = 0;

            bool has401k = GetYesOrNoAnswer("Do you have a non-Roth 401k?");
            if (has401k)
            {
                var percentContribution = GetNonNegativePercent("Enter the contribution percentage");
                var nontaxable401kIncome = percentContribution * _grossIncome;

                Console.WriteLine(
                    $"{percentContribution:P} of {_grossIncome:C} contributed, " +
                    $"for a total of {nontaxable401kIncome:C} to your 401k.");

                _taxExemptIncome += nontaxable401kIncome;
            }

            if (_isMarried)
            {
                has401k = GetYesOrNoAnswer("Does your spouse have a non-Roth 401k?");
                if (has401k)
                {
                    var spousePercentContribution = GetNonNegativePercent("Enter the contribution percentage");
                    var spouseNontaxable401kIncome = spousePercentContribution * _spouseGrossIncome;

                    Console.WriteLine(
                        $"{spousePercentContribution:P} of {_spouseGrossIncome:C} contributed, " +
                        $"for a total of {spouseNontaxable401kIncome:C} to your spouse's 401k.");

                    _taxExemptIncome += spouseNontaxable401kIncome;
                }
            }

            Console.WriteLine();
        }

        private void GetMarriageInformation()
        {
            _isMarried = GetYesOrNoAnswer("Are you legally married?");
            Console.WriteLine();
        }

        private void GetIncomeInformation()
        {
            _grossIncome = GetNonNegativeLong("Enter your yearly income");

            if (_isMarried)
            {
                _spouseGrossIncome += GetNonNegativeLong("Enter your spouse's yearly income");
            }

            Console.WriteLine();
        }

        private static bool GetYesOrNoAnswer(string question)
        {
            Console.Write($"\n{question} ('y' or 'n'): ");

            char answer = Console.ReadLine().ToLower().FirstOrDefault();

            while (answer != 'y' && answer != 'n')
            {
                Console.Write("Invalid Answer... Please respond with 'y' or 'n': ");
                answer = Console.ReadLine().ToLower().FirstOrDefault();
            }

            return answer == 'y';
        }

        private static long GetNonNegativeLong(string question)
        {
            Console.Write($"\n{question}: ");
            var isValidLong = long.TryParse(Console.ReadLine(), out long answer);

            while (!isValidLong || answer < 0)
            {
                Console.Write("Invalid Answer... Please respond with a valid/non-negative number: ");
                isValidLong = long.TryParse(Console.ReadLine(), out answer);
            }

            return answer;
        }

        private static double GetNonNegativePercent(string question)
        {
            Console.Write($"\n{question} (0 - 100): ");
            var isValidDouble = double.TryParse(Console.ReadLine(), out double answer);

            while (!isValidDouble || answer < 0 || answer > 100)
            {
                Console.Write("Invalid Answer... Please respond with a valid/non-negative number between 0 and 100: ");
                isValidDouble = double.TryParse(Console.ReadLine(), out answer);
            }

            return answer / 100;
        }
    }
}
