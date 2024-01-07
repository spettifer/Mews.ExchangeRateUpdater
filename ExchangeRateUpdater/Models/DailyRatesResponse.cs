using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ExchangeRateUpdater.Models;

[ExcludeFromCodeCoverage]
public class DailyRatesResponse
{
    public IEnumerable<DailyRate> Rates { get; set; }
}