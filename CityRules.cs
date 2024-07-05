using System.Collections.Generic;

namespace congestion.calculator
{
    public class CityConfigurations
    {
        public Dictionary<string, City> Cities { get; set; }
    }

    public class City
    {
        public int MaxDailyAmountFee { get; set; }
        public List<FeeRule> Fees { get; set; }
    }

    public class FeeRule
    {
        public string Start { get; set; }
        public string End { get; set; }
        public int Fee { get; set; }
    }


}

