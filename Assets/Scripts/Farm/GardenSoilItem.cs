/* 
 * File: GardenSoilItem.cs
 * Project: Project Dungeon
 * Author: Justin Salanga
 * Date: 02/26/2025 
 */

using System;
using UnityEngine;

/// <summary>
/// Represents an item planted in the GardenSoil farm object, managing the state a planted object.
/// </summary>
[Serializable]
public class GardenSoilItem
{
    /// <summary>
    /// The InventoryItemSO of the seed.
    /// </summary>
    public InventoryItemSO seedItemSO;
    /// <summary>
    /// The time the seed was planted.
    /// </summary>
    public DateTime startTime;
    /// <summary>
    /// The time when the seed will be ready for harvest.
    /// </summary>
    public DateTime endTime;
    /// <summary>
    /// Indicates when the item has been harvested.
    /// </summary>
    public bool harvested;
    /// <summary>
    /// Reference to the connected FarmItemUI object.
    /// NOTE: This will be null when the SelectUI is closed.
    /// </summary>
    public FarmItemUI farmItem;

    /// <summary>
    /// Plants the specified seed and sets the necessary values.
    /// </summary>
    /// <param name="seed">The InventoryItemSO representing the seed to be planted.</param>
    public void PlantSeeds(InventoryItemSO seed)
    {
        if (seed.category != InventoryCategory.Seeds)
        {
            Debug.LogError($"J$ GardenSoilItem Trying to plant a Non-Seed item");
            return;
        }
        seedItemSO = seed;
        startTime = DateTime.Now;
        endTime = DateTime.Now.AddSeconds(seed.dataB); // dataB corresponds to Time to Harvest
        harvested = false;
    }

    /// <summary>
    /// Checks to see if the current planted seed is already harvested.
    /// </summary>
    /// <returns>True if the item has been harvested; otherwise, false.</returns>
    public bool IsEmpty()
    {
        return harvested;
    }

    /// <summary>
    /// Checks if the planted seed can be harvested.
    /// </summary>
    /// <returns>True if the item can be harvested; otherwise, false.</returns>
    public bool CanBeHarvested()
    {
        return DateTime.Now > endTime;
    }

    /// <summary>
    /// Gets the time left until the planted seed can be harvested in HH:mm:ss format.
    /// </summary>
    /// <returns>A string representing the time left until harvest.</returns>
    public string GetTimeLeft()
    {
        DateTime currentTime = DateTime.Now;

        if (currentTime > endTime) 
            return "Ready to Harvest!";

        int totalHours = 0, totalMinutes = 0, totalSeconds = 0;
        TimeSpan timeDifference = endTime - currentTime;
        totalHours = (int)timeDifference.TotalHours;
        totalMinutes = timeDifference.Minutes;
        totalSeconds = timeDifference.Seconds;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", totalHours, totalMinutes, totalSeconds);
    }

    /// <summary>
    /// Gets the percentage of time left until the planted seed can be harvested.
    /// </summary>
    /// <returns>A float representing the percentage of time left until harvest.</returns>
    public float GetPercentageLeft()
    {
        DateTime currentTime = DateTime.Now;
        if (currentTime > endTime) return 1f;
        TimeSpan totalDifference = endTime - startTime;
        TimeSpan currentDifference = endTime - currentTime;

        // Formula for percentage = difference of duration / duration
        return (float)((totalDifference.TotalSeconds - currentDifference.TotalSeconds) / totalDifference.TotalSeconds);
    }

    /// <summary>
    /// Harvests the item and updates its state.
    /// </summary>
    /// <param name="newItem">Reference to the newly connected Empty FarmItemUI object.</param>
    public void Harvest(FarmItemUI newItem)
    {
        if (DateTime.Now < endTime)
        {
            Debug.LogError("J$ GardenSoilItem Trying to harvest an item that isn't ready yet.");
            return;
        }
        harvested = true;
        farmItem = newItem;
    }
}