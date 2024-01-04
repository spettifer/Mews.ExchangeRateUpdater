using System.Collections.Generic;

namespace ExchangeRateUpdater.Models;

public class DailyRatesResponse
{
    public IEnumerable<DailyRate> Rates { get; set; }
}