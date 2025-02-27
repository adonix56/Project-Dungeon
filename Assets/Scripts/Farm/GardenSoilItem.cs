using System;
using UnityEngine;

[Serializable]
public class GardenSoilItem
{
    public InventoryItemSO inventoryItemSO;
    public DateTime startTime;
    public DateTime endTime;
    public bool harvested;
    public FarmItem farmItem;

    public void PlantSeeds(InventoryItemSO iSO)
    {
        if (iSO.category != InventoryCategory.Seeds)
        {
            Debug.LogError($"J$ GardenSoilItem Trying to plant a Non-Seed item");
            return;
        }
        inventoryItemSO = iSO;
        startTime = DateTime.Now;
        endTime = DateTime.Now.AddSeconds(iSO.dataB); // dataB corresponds to Time to Harvest
        harvested = false;
    }

    public bool IsEmpty()
    {
        return harvested;
    }

    public bool CanBeHarvested()
    {
        return DateTime.Now > endTime;
    }

    public string GetTimeLeft()
    {
        DateTime currentTime = DateTime.Now;
        if (currentTime > endTime) return "Ready to Harvest!";
        int totalHours = 0, totalMinutes = 0, totalSeconds = 0;
        TimeSpan timeDifference = endTime - currentTime;
        totalHours = (int)timeDifference.TotalHours;
        totalMinutes = timeDifference.Minutes;
        totalSeconds = timeDifference.Seconds;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", totalHours, totalMinutes, totalSeconds);
    }

    public float GetPercentageLeft()
    {
        DateTime currentTime = DateTime.Now;
        if (currentTime > endTime) return 1f;
        TimeSpan totalDifference = endTime - startTime;
        TimeSpan currentDifference = endTime - currentTime;

        return (float)((totalDifference.TotalSeconds - currentDifference.TotalSeconds) / totalDifference.TotalSeconds);
    }

    public void Harvest(FarmItem newItem)
    {
        harvested = true;
        farmItem = newItem;
    }
}