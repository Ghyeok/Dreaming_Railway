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
    public float minTravelTime = 20f;
    public float maxTravelTime = 30f;

    public float minStopTime = 7f;
    public float maxStopTime = 9f;

    public StationType stationType;
    public float travelToNextStationTime;
    public float stopTime;
}
