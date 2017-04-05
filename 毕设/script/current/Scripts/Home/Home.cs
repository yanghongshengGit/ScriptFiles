using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// 主场景（开始场景）脚本
public class Home : MonoBehaviour
{
    void Start()
    {
        // hidden banner (banner only show on Game Play scene)
        //GoogleMobileAdsScript.advertise.HideBanner();
        MusicController.Music.BG_menu();    // 播放背景音乐
    }

    void Update()
    {
        // Exit game if click Escape key or back on mobile
        if (Input.GetKeyDown(KeyCode.Escape))   // 如果按下Esc键
        {
            ExitOK();   // 退出游戏
        }
    }

    /// <summary>
    /// Exit game
    /// </summary>
    public void ExitOK()
    {
        Application.Quit(); // 退出程序
    }

}
