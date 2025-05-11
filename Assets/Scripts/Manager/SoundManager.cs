using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;


 /* AudioClip : 실제로 재생되는 사운드 파일
 *  AudioSource : 해당 클립을 관리하는 역할
 */

public class SoundManager : SingletonManagers<SoundManager>
{
    private AudioSource[] audioSources = new AudioSource[(int)Define.Sounds.MaxCount];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private AudioSource BGMSource { get { return audioSources[(int)Define.Sounds.BGM]; } } // BGM으로 사용할 AudioSource
    AudioSource SFXSource { get { return audioSources[(int)Define.Sounds.SFX]; } } // SFX(효과음)으로 사용할 AudioSource

    private GameObject bgmObject;
    private GameObject sfxObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        base.Awake();

        bgmObject = new GameObject("BGM");
        bgmObject.transform.parent = this.transform;
        audioSources[(int)Define.Sounds.BGM] = bgmObject.AddComponent<AudioSource>();
        audioSources[(int)Define.Sounds.BGM].loop = true;

        sfxObject = new GameObject("SFX");
        sfxObject.transform.parent = this.transform;
        audioSources[(int)Define.Sounds.SFX] = sfxObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        SubwayBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudioClip(string path, Define.Sounds newSoundType, float volumeScale = 1f)
    {
        AudioClip clip = GetOrAddAudioClip(path);

        if(newSoundType == Define.Sounds.BGM)
        {
            // BGM 재생 로직..
            BGMSource.clip = clip;
            BGMSource.Play();
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
        PlayAudioClip("EnterFog", Define.Sounds.SFX, 0.2f);
    }
    public void ExitDreamSFX()
    {
        PlayAudioClip("ExitDream", Define.Sounds.SFX);
    }
     public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
    }
}
