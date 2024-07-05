using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using congestion.calculator;

public class CongestionTaxCalculator
{
    private readonly int MaxDailyFee;
    private readonly List<FeeRule> TollFees;

    public CongestionTaxCalculator(string city)
    {
        var config = LoadConfig(city);
        MaxDailyFee = config.MaxDailyAmountFee;
        TollFees = config.Fees;
    }

    private City LoadConfig(string cityName)
    {
        var configText = File.ReadAllText("TollFeeConfiguration.json");
        var config = JsonSerializer.Deserialize<CityConfigurations>(configText);
        if (config.Cities.ContainsKey(cityName))
            return config.Cities[cityName];

        throw new Exception("City not found");
    }

    private static readonly HashSet<string> TollFreeVehicleTypes = new HashSet<string>
    {
        TollFreeVehicles.Motorcycle.ToString(),
        TollFreeVehicles.Bus.ToString(),
        TollFreeVehicles.Emergency.ToString(),
        TollFreeVehicles.Diplomat.ToString(),
        TollFreeVehicles.Foreign.ToString(),
        TollFreeVehicles.Military.ToString(),
    };

    private static readonly HashSet<DateTime> PublicHolidays2013 = new HashSet<DateTime>
    {
        new DateTime(2013, 1, 1),
        new DateTime(2013, 3, 28), new DateTime(2013, 3, 29),
        new DateTime(2013, 4, 1), new DateTime(2013, 4, 30),
        new DateTime(2013, 5, 1), new DateTime(2013, 5, 8), new DateTime(2013, 5, 9),
        new DateTime(2013, 6, 5), new DateTime(2013, 6, 6), new DateTime(2013, 6, 21),
        new DateTime(2013, 11, 1),
        new DateTime(2013, 12, 24), new DateTime(2013, 12, 25), new DateTime(2013, 12, 26), new DateTime(2013, 12, 31)
    };

    public int GetTax(IVehicle vehicle, DateTime[] dates)
    {
        if (IsTollFreeVehicle(vehicle)) return 0;

        Array.Sort(dates);
        int totalFee = 0;
        DateTime intervalStart = dates[0];
        int currentIntervalFee = GetTollFee(intervalStart);

        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(date);
            TimeSpan diff = date - intervalStart;

            if (diff.TotalMinutes <= 60)
            {
                if (nextFee > currentIntervalFee)
                {
                    totalFee += nextFee - currentIntervalFee;
                    currentIntervalFee = nextFee;
                }
            }
            else
            {
                totalFee += nextFee;
                intervalStart = date;
                currentIntervalFee = nextFee;
            }
        }

        return Math.Min(totalFee, MaxDailyFee);
    }


    private bool IsTollFreeVehicle(IVehicle vehicle)
    {
        if (vehicle == null) return false;
        string vehicleType = vehicle.GetVehicleType();
        return TollFreeVehicleTypes.Contains(vehicleType);
    }

    public int GetTollFee(DateTime date)
    {
        if (IsTollFreeDate(date)) return 0;

        TimeSpan timeOfDay = date.TimeOfDay;
        foreach (var feeRule in TollFees)
        {
            TimeSpan start = TimeSpan.Parse(feeRule.Start);
            TimeSpan end = TimeSpan.Parse(feeRule.End);
            if (timeOfDay >= start && timeOfDay <= end)
            {
                return feeRule.Fee;
            }
        }

        return 0;
    }

    private bool IsTollFreeDate(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return true;

        return date.Month == 7 || PublicHolidays2013.Contains(date.Date);
    }


    private enum TollFreeVehicles
    {
        Motorcycle = 0,
        Bus = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5

    }


}
