using UnityEngine;

public class SubwayMiniMove : MonoBehaviour
{
    public RectTransform targetImage;

    public Vector2 position1;
    public Vector2 position2;
    public Vector2 position3;
    public Vector2 position4;
    public Vector2 position5;

    //버튼 클릭시 좌표 선정
    public void MoveToPositionDay1()
    {
        targetImage.anchoredPosition = position1;
    }

    public void MoveToPositionDay2()
    {
        targetImage.anchoredPosition = position2;
    }
    public void MoveToPositionDay3()
    {
        targetImage.anchoredPosition = position3;
    }
    public void MoveToPositionDay4()
    {
        targetImage.anchoredPosition = position4;
    }
    public void MoveToPositionDay5()
    {
        targetImage.anchoredPosition = position5;
    }
}
