using UnityEngine;
using System.Collections;

/// <summary>
/// 声音游戏对象 组件 (挂在 Sound游戏对象下)
/// sound in game : button, effect, win, lose...
/// </summary>
public class SoundController : MonoBehaviour
{
	// 音效控制对象（单例）
    public static SoundController Sound; // instance of SoundController
	// 音效片段存储数组
    public AudioClip[] SoundClips;      // array sound clips
	// 音效播放组件
    public AudioSource audiosource;     // audio source
    void Awake()
    {
        if (Sound == null)
        {
            // 切换场景也不销毁对象
            DontDestroyOnLoad(gameObject);
            Sound = this;
        }
        else if (Sound != this)
        {
            // 销毁对象
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 打开音效
    /// </summary>
    public void SoundON()
    {
        audiosource.mute = false;
    }

    /// <summary>
    /// 关闭音效
    /// </summary>
    public void SoundOFF()
    {
        audiosource.mute = true;
    }
    // 点击按钮音效
    public void Click()
    {
		// 播放一个声音片段，参数 音效片段，音量
        audiosource.PlayOneShot(SoundClips[0]);

    }
    // 宝石销毁音效
    public void JewelCrash()
    {
        audiosource.PlayOneShot(SoundClips[1]);
    }
    // 解锁音效
    public void LockCrash()
    {
        audiosource.PlayOneShot(SoundClips[2]);
    }
    // 冰崩音效
    public void IceCrash()
    {
        audiosource.PlayOneShot(SoundClips[3]);
    }
    // 胜利音效
    public void Win()
    {
        audiosource.PlayOneShot(SoundClips[4]);
    }
    // 失败音效
    public void Lose()
    {
        audiosource.PlayOneShot(SoundClips[5]);
    }
    // 星星宝石出现音效
    public void StarIn()
    {
        audiosource.PlayOneShot(SoundClips[6]);
    }
    // 火箭音效
    public void Fire()
    {
        audiosource.PlayOneShot(SoundClips[7]);
    }
    // 火球音效
    public void Gun()
    {
        audiosource.PlayOneShot(SoundClips[8]);
    }
    // 爆炸音效
    public void Boom()
    {
        audiosource.PlayOneShot(SoundClips[9]);
    }

}
