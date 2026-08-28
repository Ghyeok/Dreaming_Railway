using UnityEngine;

public class SaveManager : SingletonManagers<SaveManager>, IManager
{
    private const string KEY_MAX_CLEAR_DAY = "MaxClearStage";
    private const string KEY_BGM_VOLUME = "BGM_VOLUME";
    private const string KEY_SFX_VOLUME = "SFX_VOLUME";
    private const string KEY_BGM_MUTE = "BGM_MUTE";
    private const string KEY_SFX_MUTE = "SFX_MUTE";

    public void Init() { }
    public int LoadMaxClearDay()
    {
        // 저장된 값이 없으면 기본값 -1을 반환합니다.
        return PlayerPrefs.GetInt(KEY_MAX_CLEAR_DAY, -1);
    }

    public void SaveMaxClearDay(int day)
    {
        PlayerPrefs.SetInt(KEY_MAX_CLEAR_DAY, day);
        PlayerPrefs.Save();
    }

    public float LoadBgmVolume()
    {
        return PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 1.0f); // 기본값 1.0 (최대)
    }

    public void SaveBgmVolume(float volume)
    {
        PlayerPrefs.SetFloat(KEY_BGM_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float LoadSfxVolume()
    {
        return PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 1.0f); // 기본값 1.0 (최대)
    }

    public void SaveSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(KEY_SFX_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public bool LoadBgmMute() => PlayerPrefs.GetInt(KEY_BGM_MUTE, 0) == 1;
    public void SaveBgmMute(bool isMute)
    {
        PlayerPrefs.SetInt(KEY_BGM_MUTE, isMute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool LoadSfxMute() => PlayerPrefs.GetInt(KEY_SFX_MUTE, 0) == 1;
    public void SaveSfxMute(bool isMute)
    {
        PlayerPrefs.SetInt(KEY_SFX_MUTE, isMute ? 1 : 0);
        PlayerPrefs.Save();
    }

}