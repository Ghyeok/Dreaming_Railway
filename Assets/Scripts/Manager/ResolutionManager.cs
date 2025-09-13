using UnityEngine;

public class ResolutionManager : SingletonManagers<ResolutionManager>, IManager
{
    private const int TARGET_WIDTH = 1920;
    private const int TARGET_HEIGHT = 1080;
    private const float TARGET_ASPECT = 16f / 9f;

    public void Init()
    {
        SetResolution();
    }

    private void SetResolution()
    {
#if UNITY_ANDROID
        float deviceAspect = (float)Screen.width / Screen.height;

        if (deviceAspect < TARGET_ASPECT)
        {
            // 화면이 더 세로로 길 경우
            float scale = deviceAspect / TARGET_ASPECT;
            Camera.main.rect = new Rect((1f - scale) / 2f, 0f, scale, 1f);
        }
        else if (deviceAspect > TARGET_ASPECT)
        {
            // 화면이 더 가로로 길 경우
            float scale = TARGET_ASPECT / deviceAspect;
            Camera.main.rect = new Rect(0f, (1f - scale) / 2f, 1f, scale);
        }
        else
        {
            // 완벽한 16:9
            Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
        }

        //Screen.SetResolution(TARGET_WIDTH, TARGET_HEIGHT, true);
#endif
    }
}
