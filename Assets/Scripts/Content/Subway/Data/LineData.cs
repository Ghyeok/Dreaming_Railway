using System.Collections.Generic;

[System.Serializable]
public class LineData
{
    public List<StationData> stations = new List<StationData>(); // 20개역을 넘을 수 없으므로 넉넉하게
    public int transferIdx = -1; // 환승역 인덱스
    public bool hasDestination = false; // 현재 Line에 도착역이 있는지
}
