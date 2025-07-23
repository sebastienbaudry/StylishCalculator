using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StylishCalculator.Models
{
    /// <summary>
    /// Represents currency information including code, name, and flag
    /// </summary>
    public class CurrencyInfo
    {
        /// <summary>
        /// Currency code (e.g., USD, EUR)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Full currency name (e.g., US Dollar, Euro)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country name associated with the currency
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Flag image path
        /// </summary>
        public string FlagImagePath { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Currency code</param>
        /// <param name="name">Full currency name</param>
        /// <param name="country">Country name</param>
        /// <param name="flagImagePath">Flag image path</param>
        public CurrencyInfo(string code, string name, string country, string flagImagePath)
        {
            Code = code;
            Name = name;
            Country = country;
            FlagImagePath = flagImagePath;
        }

        /// <summary>
        /// Returns a string representation of the currency
        /// </summary>
        /// <returns>Formatted string with code and name</returns>
        public override string ToString()
        {
            return $"{Code} - {Name}";
        }
    }
}