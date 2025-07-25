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
        bgmObject = new GameObject("BGM");
        bgmObject.transform.parent = this.transform;
        audioSources[(int)Define.Sounds.BGM] = bgmObject.AddComponent<AudioSource>();
        audioSources[(int)Define.Sounds.BGM].loop = true;

        sfxObject = new GameObject("SFX");
        sfxObject.transform.parent = this.transform;
        audioSources[(int)Define.Sounds.SFX] = sfxObject.AddComponent<AudioSource>();

        bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1.0f);

        audioSources[(int)Define.Sounds.BGM].volume = bgmVolume;
        audioSources[(int)Define.Sounds.SFX].volume = sfxVolume;

        IsBGMOff = PlayerPrefs.GetInt("BGM_MUTE", 0) == 1;
        IsSFXOff = PlayerPrefs.GetInt("SFX_MUTE", 0) == 1;

        SetBGMOnOffState();
        SetSFXOnOffState();

        MainBGM();
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
        PlayAudioClip("EnterFog", Define.Sounds.SFX, 0.5f);
    }

    public void ExitDreamSFX()
    {
        PlayAudioClip("ExitDream", Define.Sounds.SFX);
    }

    public void GameOverSFX()
    {
        PlayAudioClip("GameOver", Define.Sounds.SFX);
    }

    private void SetBGMOnOffState()
    {
        audioSources[(int)Define.Sounds.BGM].mute = IsBGMOff;
    }

    private void SetSFXOnOffState()
    {
        audioSources[(int)Define.Sounds.SFX].mute = IsSFXOff;
    }

    public float SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SFXSource.volume = sfxVolume;

        PlayerPrefs.SetFloat("SFX_VOLUME", sfxVolume);
        PlayerPrefs.Save();

        return sfxVolume;

    }

    public float SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        BGMSource.volume = bgmVolume;

        PlayerPrefs.SetFloat("BGM_VOLUME", bgmVolume);
        PlayerPrefs.Save();

        return bgmVolume;
    }

    public void SetSFXOff()
    {
        IsSFXOff = true;
        SFXSource.mute = true;

        PlayerPrefs.SetInt("SFX_MUTE", 1);
        PlayerPrefs.Save();
    }

    public void SetSFXOn()
    {
        IsSFXOff = false;
        SFXSource.mute = false;

        PlayerPrefs.SetInt("SFX_MUTE", 0);
        PlayerPrefs.Save();
    }

    public void SetBGMOff()
    {
        IsBGMOff = true;
        BGMSource.mute = true;

        PlayerPrefs.SetInt("BGM_MUTE", 1);
        PlayerPrefs.Save();
    }

    public void SetBGMOn()
    {
        IsBGMOff = false;
        BGMSource.mute = false;

        PlayerPrefs.SetInt("BGM_MUTE", 0);
        PlayerPrefs.Save();
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
            Debug.Log($"메인 씬 로드 : {gameObject.name}");
            MainBGM();
        }
    }
}
