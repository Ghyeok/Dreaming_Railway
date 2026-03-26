using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : SingletonManagers<SoundManager>, IManager
{
    private AudioSource[] audioSources = new AudioSource[(int)Sounds.MaxCount];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private AudioSource BGMSource => audioSources[(int)Sounds.BGM];
    private AudioSource SFXSource => audioSources[(int)Sounds.SFX];

    public bool IsBGMOff { get; private set; }
    public bool IsSFXOff { get; private set; }
    public float BgmVolume { get; private set; }
    public float SfxVolume { get; private set; }

    public void Init()
    {
        // AudioSource 생성
        if (transform.Find("BGM") == null)
        {
            GameObject bgmObject = new GameObject("BGM");
            bgmObject.transform.parent = this.transform;
            audioSources[(int)Sounds.BGM] = bgmObject.AddComponent<AudioSource>();
            BGMSource.loop = true;
        }

        if (transform.Find("SFX") == null)
        {
            GameObject sfxObject = new GameObject("SFX");
            sfxObject.transform.parent = this.transform;
            audioSources[(int)Sounds.SFX] = sfxObject.AddComponent<AudioSource>();
        }

        // SaveManager로부터 저장된 데이터 가져오기
        BgmVolume = SaveManager.Instance.LoadBgmVolume();
        SfxVolume = SaveManager.Instance.LoadSfxVolume();
        IsBGMOff = SaveManager.Instance.LoadBgmMute();
        IsSFXOff = SaveManager.Instance.LoadSfxMute();

        // 현재 설정 적용
        ApplyMuteAndVolume();
    }

    private void ApplyMuteAndVolume()
    {
        if (BGMSource != null)
        {
            BGMSource.volume = BgmVolume;
            BGMSource.mute = IsBGMOff;
        }
        if (SFXSource != null)
        {
            SFXSource.volume = SfxVolume;
            SFXSource.mute = IsSFXOff;
        }
    }

    public void PlayAudioClip(string path, Sounds newSoundType, float volumeScale = 1f)
    {
        AudioClip clip = GetOrAddAudioClip(path);
        if (clip == null) return;

        if (newSoundType == Sounds.BGM)
        {
            // 이미 같은 BGM이 재생 중이면 무시
            if (BGMSource.clip == clip && BGMSource.isPlaying) return;

            // 같은 BGM인데 멈춰있으면 이어서 재생
            if (BGMSource.clip == clip && !BGMSource.isPlaying)
            {
                BGMSource.UnPause();
                return;
            }

            // 아예 새로운 BGM이면 교체 후 재생
            BGMSource.clip = clip;
            BGMSource.Play();
        }
        else if (newSoundType == Sounds.SFX)
        {
            SFXSource.PlayOneShot(clip, volumeScale);
        }
    }

    public AudioClip GetOrAddAudioClip(string path)
    {
        if (audioClips.TryGetValue(path, out AudioClip clip))
            return clip;

        clip = Resources.Load<AudioClip>($"Sounds/BGM/{path}");
        if (clip == null)
            clip = Resources.Load<AudioClip>($"Sounds/SFX/{path}");

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

    public float SetBGMVolume(float volume)
    {
        BgmVolume = Mathf.Clamp01(volume);
        SaveManager.Instance.SaveBgmVolume(BgmVolume);
        ApplyMuteAndVolume();
        return BgmVolume;
    }

    public float SetSFXVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);
        SaveManager.Instance.SaveSfxVolume(SfxVolume);
        ApplyMuteAndVolume();
        return SfxVolume;
    }

    public void SetBGMOff(bool isOff)
    {
        IsBGMOff = isOff;
        SaveManager.Instance.SaveBgmMute(IsBGMOff);
        ApplyMuteAndVolume();
    }

    public void SetSFXOff(bool isOff)
    {
        IsSFXOff = isOff;
        SaveManager.Instance.SaveSfxMute(IsSFXOff);
        ApplyMuteAndVolume();
    }

    // 헬퍼 함수들
    public void MainBGM() => PlayAudioClip("TitleTheme", Sounds.BGM);
    public void SubwayBGM() => PlayAudioClip("TrainMusic", Sounds.BGM);
    public void DreamBGM() => PlayAudioClip("DreamMusic", Sounds.BGM);

    public void JumpSFX() => PlayAudioClip("JumpSound", Sounds.SFX);
    public void LandSFX() => PlayAudioClip("LandSound", Sounds.SFX);
    public void Footstep1SFX() => PlayAudioClip("FootstepCloud1", Sounds.SFX);
    public void Footstep2SFX() => PlayAudioClip("FootstepCloud2", Sounds.SFX);
    public void EnterFogSFX() => PlayAudioClip("EnterFog", Sounds.SFX, 0.6f);
    public void ExitDreamSFX() => PlayAudioClip("ExitDream", Sounds.SFX);
    public void GameOverSFX() => PlayAudioClip("GameOver", Sounds.SFX);
    public void GameClearSFX() => PlayAudioClip("LastLine", Sounds.SFX);
}