using UnityEngine;

public class SaveManager : SingletonManagers<SaveManager>, IManager
{
    private const string KEY_MAX_CLEAR_DAY = "MaxClearStage";
    private const string KEY_BGM_VOLUME = "BGM_VOLUME";
    private const string KEY_SFX_VOLUME = "SFX_VOLUME";
    private const string KEY_BGM_MUTE = "BGM_MUTE";
    private const string KEY_SFX_MUTE = "SFX_MUTE";

    public void Init() { }

    /// <summary>
    /// 최고 기록을 반환한다. 저장값이 없으면 기본값 -1을 할당한다.
    /// </summary>
    public int LoadMaxClearDay()
    {
        return PlayerPrefs.GetInt(KEY_MAX_CLEAR_DAY, -1);
    }

    /// <summary>
    /// 최고 기록을 저장한다.
    /// </summary>
    /// <param name="day"></param>
    public void SaveMaxClearDay(int day)
    {
        PlayerPrefs.SetInt(KEY_MAX_CLEAR_DAY, day);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// BGM 볼륨 값을 반환한다. 저장값이 없다면 기본값 1.0(최대)을 할당한다.
    /// </summary>
    public float LoadBgmVolume()
    {
        return PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 1.0f); // 기본값 1.0 (최대)
    }

    /// <summary>
    /// BGM 볼륨 값을 저장한다.
    /// </summary>
    public void SaveBgmVolume(float volume)
    {
        PlayerPrefs.SetFloat(KEY_BGM_VOLUME, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SFX 볼륨 값을 반환한다. 저장값이 없다면 기본값 1.0(최대)을 할당한다.
    /// </summary>
    public float LoadSfxVolume()
    {
        return PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 1.0f); // 기본값 1.0 (최대)
    }

    /// <summary>
    /// SFX 볼륨 값을 저장한다.
    /// </summary>
    public void SaveSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(KEY_SFX_VOLUME, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// BGM 음소거 여부를 반환한다. 저장값이 없다면 기본값 0(음소거 해제)을 할당한다.
    /// </summary>
    public bool LoadBgmMute() => PlayerPrefs.GetInt(KEY_BGM_MUTE, 0) == 1;

    /// <summary>
    /// BGM 음소거 여부를 저장한다.
    /// </summary>
    public void SaveBgmMute(bool isMute)
    {
        PlayerPrefs.SetInt(KEY_BGM_MUTE, isMute ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SFX 음소거 여부를 반환한다. 저장값이 없다면 기본값 0(음소거 해제)을 할당한다.
    /// </summary>
    public bool LoadSfxMute() => PlayerPrefs.GetInt(KEY_SFX_MUTE, 0) == 1;

    /// <summary>
    /// SFX 음소거 여부를 저장한다.
    /// </summary>
    public void SaveSfxMute(bool isMute)
    {
        PlayerPrefs.SetInt(KEY_SFX_MUTE, isMute ? 1 : 0);
        PlayerPrefs.Save();
    }
}