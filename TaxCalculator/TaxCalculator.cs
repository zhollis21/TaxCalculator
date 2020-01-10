﻿using System;
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
                // TODO: Store couple income's seperately
                _taxExemptIncome = GetNonNegativeDouble("Enter the contribution percentage");
            }
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

        private static double GetNonNegativeDouble(string prompt)
        {
            Console.Write($"\n{prompt}: ");
            var isValidDouble = double.TryParse(Console.ReadLine(), out double answer);

            while (!isValidDouble || answer < 0)
            {
                Console.Write("Invalid Answer... Please respond with a valid/non-negative number: ");
                isValidDouble = double.TryParse(Console.ReadLine(), out answer);
            }

            return answer;
        }

        private void GetMarriageInformation()
        {
            _isMarried = GetYesOrNoAnswer("Are you legally married?");
        }

        private void GetIncomeInformation()
        {
            _grossIncome = 0;

            _grossIncome += GetNonNegativeLong("Enter your yearly income");

            if (_isMarried)
            {
                _grossIncome += GetNonNegativeLong("Enter your spouse's yearly income");
            }
            Console.WriteLine();
        }

        public void CalculateTaxEstimation()
        {
            long untaxedIncome = _grossIncome;
            double taxEstimate = 0;

            // Select the correct tax bracket based on marriage status
            var taxBrackets = _isMarried ? _married2019TaxBrackets : _single2019TaxBrackets;

            long previousBracketIncomeLimit = 0;
            foreach (var bracket in taxBrackets)
            {
                long bracketRange = bracket.IncomeLimit - previousBracketIncomeLimit;

                long applicableUntaxedIncome;

                if (untaxedIncome > bracketRange)
                {
                    applicableUntaxedIncome = bracketRange;
                }
                else
                {
                    applicableUntaxedIncome = untaxedIncome;
                }

                taxEstimate += applicableUntaxedIncome * bracket.TaxRate;
                Console.WriteLine(
                    $"{applicableUntaxedIncome} taxed at a rate of {Math.Round(bracket.TaxRate * 100)}% " +
                    $"for a total of {applicableUntaxedIncome * bracket.TaxRate} in taxes.");

                untaxedIncome -= bracketRange;

                if (untaxedIncome <= 0)
                {
                    break;
                }

                previousBracketIncomeLimit = bracket.IncomeLimit;
            }

            Console.WriteLine(
                $"\nTotal Gross Income: ${_grossIncome}" +
                $"\nTotal Net Income: ${_grossIncome - taxEstimate}" +
                $"\nTotal Taxes: ${taxEstimate}" +
                $"\nAverage Percent Taxed: {Math.Round((taxEstimate / _grossIncome) * 100)}%");
        }
    }
}
