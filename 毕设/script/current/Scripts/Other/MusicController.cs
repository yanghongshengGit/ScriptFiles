using UnityEngine;
using System.Collections;

/// <summary>
/// 背景音乐游戏对象 组件 (挂在 Music游戏对象下)
/// background music in game
/// </summary>
public class MusicController : MonoBehaviour {
	// 音乐对象(单例)
    public static MusicController Music;
	// 音乐片段存储数组
    public AudioClip[] MusicClips;
	// 音频播放组件
    public AudioSource audiosource;

    void Awake()
    {
        if (Music == null)
        {
			// 切换场景也保留对象
            DontDestroyOnLoad(gameObject);
            Music = this;
        }
        else if (Music != this)
        {
			// 销毁对象
            Destroy(gameObject);
        }
        
    }

    public void MusicON()
    {
		// 静音关闭
        audiosource.mute = false; 
    }

    public void MusicOFF(){
		// 静音开启
        audiosource.mute = true; 
        
    }
	// 主场景时播放的背景音乐
    public void BG_menu()
    {
		// 播放的音乐片段 = MusicClips[0]
        audiosource.clip = MusicClips[0];
		// 开始播放
        audiosource.Play();
    }
	// game场景时播放的背景音乐
    public void BG_play()
    {
		// 播放的音乐片段 = MusicClips[1]
        audiosource.clip = MusicClips[1];
		// 开始播放
        audiosource.Play();
    }

}
