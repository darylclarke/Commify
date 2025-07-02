using System;

namespace TaxCalculator.Api.Models;

public class InvalidSalaryException : Exception
{
    public InvalidSalaryException(string message) : base(message) { }
} 