using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[System.Serializable]
public class LineData
{
    public List<StationData> stations = new List<StationData>(); // 20개역을 넘을 수 없으므로 넉넉하게
    public int transferIdx = -1;
    public bool hasDestination = false;
}
