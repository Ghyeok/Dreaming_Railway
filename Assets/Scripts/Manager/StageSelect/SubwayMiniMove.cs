using UnityEngine;

public class SubwayMiniMove : MonoBehaviour
{
    public RectTransform targetImage;

    public RectTransform position0;
    public RectTransform position1;
    public RectTransform position2;
    public RectTransform position3;
    public RectTransform position4;
    public RectTransform position5;

    //버튼 클릭시 좌표 선정
    public void MoveToPositionDay0()
    {
        targetImage.anchoredPosition = position0.anchoredPosition;
    }
    public void MoveToPositionDay1()
    {
        targetImage.anchoredPosition = position1.anchoredPosition;
    }
    public void MoveToPositionDay2()
    {
        targetImage.anchoredPosition = position2.anchoredPosition;
    }
    public void MoveToPositionDay3()
    {
        targetImage.anchoredPosition = position3.anchoredPosition;
    }
    public void MoveToPositionDay4()
    {
        targetImage.anchoredPosition = position4.anchoredPosition;
    }
    public void MoveToPositionDay5()
    {
        targetImage.anchoredPosition = position5.anchoredPosition;
    }
}
