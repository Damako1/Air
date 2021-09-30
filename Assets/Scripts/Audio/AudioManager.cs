using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] AudioSource sFXPlayer;

    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="audioClip">音源</param>
    /// <param name="volume">音量</param>
    public void PlaySFX(AudioData audioData)
    {
        sFXPlayer.PlayOneShot(audioData.audioClip,audioData.volume);

        //AudioSource.Play() 无法播放复数的音效，当播放新的音效，会把原来的音效停止
        //AudioSource.PlayOneShot() 不会停止原来的音效
    }

    /// <summary>
    /// 播放随机音效
    /// </summary>
    public void PlayRandomSFX(AudioData audioData)
    {
        sFXPlayer.pitch = Random.Range(MIN_PITCH,MAX_PITCH);
        PlaySFX(audioData);
    }


    public void PlayRandomSFX(AudioData[] audioData)
    {
        PlayRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }
}


[System.Serializable]public class AudioData
{
    [Header("音效")]
    public AudioClip audioClip;

    [Header("音量")]
    public float volume;
}