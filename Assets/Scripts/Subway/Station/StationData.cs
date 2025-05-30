using UnityEngine;

public enum StationType
{
    Normal,
    Transfer,
    Destination
}

[System.Serializable]
public class StationData
{
    public StationType stationType;
    public float travelTime;
    public float stopTime;

    public StationData(float minTravelTime, float maxTravelTime, float minStopTime, float maxStopTime)
    {
        this.travelTime = Random.Range(minTravelTime, maxTravelTime);
        this.stopTime = Random.Range(minStopTime, maxStopTime);
        this.stationType = StationType.Normal;
    }

    public StationData(float minTravelTime, float maxTravelTime, float minStopTime, float maxStopTime, StationType type)
    {
        this.travelTime = Random.Range(minTravelTime, maxTravelTime);
        this.stopTime = Random.Range(minStopTime, maxStopTime);
        this.stationType = type;
    }
}
