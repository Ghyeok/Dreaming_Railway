using UnityEngine;
using UnityEngine.UI;

public class UI_Image : UI_Bind
{
    public enum Images
    {
        TimerImage,
        // 필요한 이미지 추가..
    }

    public Image timerImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<Image>(typeof(Images));

        timerImage = GetImage((int)Images.TimerImage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
