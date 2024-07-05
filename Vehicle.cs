namespace congestion.calculator
{
    public class Car : IVehicle
    {
        public string GetVehicleType() => "Car";
    }
    public class Buss : IVehicle
    {
        public string GetVehicleType() => "Bus";
    }
    public class Emergency : IVehicle
    {
        public string GetVehicleType() => "Emergency";
    }
    public class Diplomat : IVehicle
    {
        public string GetVehicleType() => "Diplomat";
    }
    public class Foreign : IVehicle
    {
        public string GetVehicleType() => "Foreign";
    }
    public class Military : IVehicle
    {
        public string GetVehicleType() => "Military";
    }
    public class Motorcycle : IVehicle
    {
        public string GetVehicleType()
        {
            return "Motorcycle";
        }
    }


}