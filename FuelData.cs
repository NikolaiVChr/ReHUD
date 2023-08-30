using Newtonsoft.Json;

namespace R3E;

public class FuelData : CombinationUserData<FuelCombination>
{
    protected override string DataFilePath => "userData.json";

    // combinations[trackLayoutId][carId];

    protected override FuelData NewInstance()
    {
        return new FuelData();
    }

    protected override FuelCombination NewCombinationInstance()
    {
        return new FuelCombination();
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class FuelCombination
{
    private const int MAX_DATA_SIZE = 20;
    
    [JsonProperty]
    private LinkedList<double> fuelUsage = new();
    [JsonProperty]
    private LinkedList<double> lapTimes = new();
    [JsonProperty]
    private double bestLapTime = -1;

    private double averageFuelUsage = -1;
    private double averageLapTime = -1;
    private double lastFuel = -1;

    public void AddFuelUsage(double fuelUsage, bool valid)
    {
        if (fuelUsage <= 0)
            return;

        lastFuel = fuelUsage;

        if (valid)
        {
            averageFuelUsage = -1;
            this.fuelUsage.AddLast(fuelUsage);
            if (this.fuelUsage.Count > MAX_DATA_SIZE)
            {
                this.fuelUsage.RemoveFirst();
            }
        }
    }
    public void AddFuelUsage(double fuelUsage)
    {
        AddFuelUsage(fuelUsage, true);
    }

    public void AddLapTime(double lapTime)
    {
        if (lapTime <= 0)
            return;

        averageLapTime = -1;
        this.lapTimes.AddLast(lapTime);
        if (this.lapTimes.Count > MAX_DATA_SIZE)
        {
            this.lapTimes.RemoveFirst();
        }

        if (bestLapTime == -1 || lapTime < bestLapTime)
        {
            bestLapTime = lapTime;
        }
    }

    public double GetAverageFuelUsage()
    {
        if (averageFuelUsage != -1)
            return averageFuelUsage;

        if (fuelUsage.Count == 0)
            return 0;

        double sum = 0;
        foreach (double fuel in fuelUsage)
        {
            sum += fuel;
        }
        averageFuelUsage = sum / fuelUsage.Count;
        return averageFuelUsage;
    }

    public double? GetLastLapFuelUsage()
    {
        if (lastFuel != -1 && fuelUsage.Count > 0)
            return lastFuel;

        return null;
    }

    public double GetAverageLapTime()
    {
        if (averageLapTime != -1)
            return averageLapTime;

        if (lapTimes.Count == 0)
            return 0;

        double sum = 0;
        foreach (double lapTime in lapTimes)
        {
            sum += lapTime;
        }
        averageLapTime = sum / lapTimes.Count;
        return averageLapTime;
    }

    public double GetBestLapTime()
    {
        return bestLapTime;
    }
}
