using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/* AudioClip : 실제로 재생되는 사운드 파일
*  AudioSource : 해당 클립을 관리하는 역할
*/

public class SoundManager : SingletonManagers<SoundManager>, IManager
{
    private AudioSource[] audioSources = new AudioSource[(int)Define.Sounds.MaxCount];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private AudioSource BGMSource { get { return audioSources[(int)Define.Sounds.BGM]; } } // BGM으로 사용할 AudioSource
    AudioSource SFXSource { get { return audioSources[(int)Define.Sounds.SFX]; } } // SFX(효과음)으로 사용할 AudioSource

    private GameObject bgmObject;
    private GameObject sfxObject;

    public bool IsBGMOff { get; private set; }
    public bool IsSFXOff { get; private set; }

    public float bgmVolume;
    public float sfxVolume;

    public void Init()
    {
        if (!PlayerPrefs.HasKey("BGM_VOLUME")) PlayerPrefs.SetFloat("BGM_VOLUME", 1.0f);
        if (!PlayerPrefs.HasKey("SFX_VOLUME")) PlayerPrefs.SetFloat("SFX_VOLUME", 1.0f);
        if (!PlayerPrefs.HasKey("BGM_MUTE")) PlayerPrefs.SetInt("BGM_MUTE", 0);
        if (!PlayerPrefs.HasKey("SFX_MUTE")) PlayerPrefs.SetInt("SFX_MUTE", 0);
        PlayerPrefs.Save();

        if (transform.Find("BGM") == null)
        {
            bgmObject = new GameObject("BGM");
            bgmObject.transform.parent = this.transform;
            audioSources[(int)Define.Sounds.BGM] = bgmObject.AddComponent<AudioSource>();
            audioSources[(int)Define.Sounds.BGM].loop = true;
        }

        sfxObject = new GameObject("SFX");
        sfxObject.transform.parent = this.transform;
        audioSources[(int)Define.Sounds.SFX] = sfxObject.AddComponent<AudioSource>();

        bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1.0f);
        IsBGMOff = PlayerPrefs.GetInt("BGM_MUTE", 0) == 1;
        IsSFXOff = PlayerPrefs.GetInt("SFX_MUTE", 0) == 1;

        ApplyMuteAndVolume();

        MainBGM();
    }

    private void ApplyMuteAndVolume()
    {
        if (audioSources[(int)Define.Sounds.BGM] != null)
        {
            audioSources[(int)Define.Sounds.BGM].volume = Mathf.Clamp01(bgmVolume);
            audioSources[(int)Define.Sounds.BGM].mute = IsBGMOff;
        }
        if (audioSources[(int)Define.Sounds.SFX] != null)
        {
            audioSources[(int)Define.Sounds.SFX].volume = Mathf.Clamp01(sfxVolume);
            audioSources[(int)Define.Sounds.SFX].mute = IsSFXOff;
        }
    }

    public void PlayAudioClip(string path, Define.Sounds newSoundType, float volumeScale = 1f)
    {
        AudioClip clip = GetOrAddAudioClip(path);

        if(newSoundType == Define.Sounds.BGM)
        {
            if (BGMSource.clip == clip && BGMSource.isPlaying)
            {
                return;
            }
            else if (BGMSource.clip == clip && !BGMSource.isPlaying)
            {
                BGMSource.UnPause();
                return;
            }

            BGMSource.clip = clip;
            BGMSource.Play();
            return;
        }

        if (newSoundType == Define.Sounds.SFX)
        {
            // 효과음 재생 로직..
            SFXSource.clip = clip;
            SFXSource.PlayOneShot(clip, volumeScale);
        }

    }

    public AudioClip GetOrAddAudioClip(string path)
    {
        if (audioClips.TryGetValue(path, out AudioClip clip)) // Get
            return clip;

        clip = Resources.Load<AudioClip>($"Sounds/BGM/{path}"); // Add
        if (clip == null)
            clip = Resources.Load<AudioClip>($"Sounds/SFX/{path}"); // Add

        if (clip == null)
        {
            Debug.LogWarning($"오디오 클립을 찾을 수 없습니다 : {path}");
        }
        else
        {
            audioClips[path] = clip;
        }

        return clip;
    }

    public void MainBGM()
    {
        PlayAudioClip("TitleTheme", Define.Sounds.BGM);
    }

    public void SubwayBGM()
    {
        PlayAudioClip("TrainMusic", Define.Sounds.BGM);
    }

    public void DreamBGM()
    {
        PlayAudioClip("DreamMusic", Define.Sounds.BGM);
    }
    
    public void JumpSFX()
    {
        PlayAudioClip("JumpSound", Define.Sounds.SFX);
    }

    public void LandSFX()
    {
        PlayAudioClip("LandSound", Define.Sounds.SFX);
    }

    public void Footstep1SFX()
    {
        PlayAudioClip("FootstepCloud1", Define.Sounds.SFX);
    }

    public void Footstep2SFX()
    {
        PlayAudioClip("FootstepCloud2", Define.Sounds.SFX);
    }

    public void EnterFogSFX()
    {
        PlayAudioClip("EnterFog", Define.Sounds.SFX, 0.6f);
    }

    public void ExitDreamSFX()
    {
        PlayAudioClip("ExitDream", Define.Sounds.SFX);
    }

    public void GameOverSFX()
    {
        PlayAudioClip("GameOver", Define.Sounds.SFX);
    }

    public void GameClearSFX()
    {
        PlayAudioClip("LastLine", Define.Sounds.SFX);
    }

    public float SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFX_VOLUME", sfxVolume);
        PlayerPrefs.Save();
        ApplyMuteAndVolume();
        return sfxVolume;
    }

    public float SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("BGM_VOLUME", bgmVolume);
        PlayerPrefs.Save();
        ApplyMuteAndVolume();
        return bgmVolume;
    }

    public void SetSFXOff()
    {
        IsSFXOff = true;
        PlayerPrefs.SetInt("SFX_MUTE", 1);
        PlayerPrefs.Save();
        ApplyMuteAndVolume();
    }

    public void SetSFXOn()
    {
        IsSFXOff = false;
        PlayerPrefs.SetInt("SFX_MUTE", 0);
        PlayerPrefs.Save();
        ApplyMuteAndVolume();
    }

    public void SetBGMOff()
    {
        IsBGMOff = true;
        PlayerPrefs.SetInt("BGM_MUTE", 1);
        PlayerPrefs.Save();
        ApplyMuteAndVolume();
    }

    public void SetBGMOn()
    {
        IsBGMOff = false;
        PlayerPrefs.SetInt("BGM_MUTE", 0);
        PlayerPrefs.Save();
        ApplyMuteAndVolume();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            MainBGM();
        }
    }
}
