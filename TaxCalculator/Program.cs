using System;

namespace TaxCalculator
{
    public static class Program
    {
        private static TaxCalculator _taxCalculator;

        static Program()
        {
            _taxCalculator = new TaxCalculator();
        }

        static void Main()
        {
            PrintIntroduction();

            _taxCalculator.GetFinancialInformationFromUser();

            _taxCalculator.CalculateTaxEstimation();
        }

        private static void PrintIntroduction()
        {
            Console.WriteLine(
                "\n\tWelcome to TaxCalculator 2019" +
                "\n\tPlease enter your information below and we will estimate your taxes.");
        }
    }
}