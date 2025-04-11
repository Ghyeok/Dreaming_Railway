using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineData", menuName = "Scriptable Objects/LineData")]
public class LineDataSO : ScriptableObject
{
    public List<StationData> stations = new List<StationData>(); // station의 총 개수는 몇개인가??
}
