using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//所有界面按钮管理
public class ButtonActionController : MonoBehaviour
{
	// 按钮管理对象（单例）
    public static ButtonActionController Click;     // instance of ButtonActionController
	// 按钮图数组
    public Sprite[] ButtonSprite;                   //sprite array of buttons

    void Awake()
    {
        if (Click == null)
        {
            DontDestroyOnLoad(gameObject);		// 切换场景不销毁脚本组件所在对象
            Click = this;
        }
        else if (Click != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// When select classic mode
    /// </summary>
    /// <param name="level">number of level</param>
    public void ClassicScene(int level)			// 切换到传统模式
    {
        SoundController.Sound.Click();			// 点击音效
        Time.timeScale = 1;						// 游戏时间倍速 = 1 正常
		PLayerInfo.MODE = 0;					// 游戏模式Classic
        PLayerInfo.MapPlayer = new Player();	// 玩家存档对象
        PLayerInfo.MapPlayer.Level = level;		// 当前等级
        PLayerInfo.MapPlayer.HightScore = level;// 最高分
        PLayerInfo.MapPlayer.HightScore = PlayerPrefs.GetInt(PLayerInfo.KEY_CLASSIC_HISCORE, 0);//获取最高分
		SceneManager.LoadScene("PlayScene");	// 载入playScene
    }


    /// <summary>
    /// When select arcade mode
    /// </summary>
    /// <param name="player">info of level to play</param>
	public void ArcadeScene(Player player)		// 切换到街机模式
    {
		SoundController.Sound.Click();			// 点击音效
        Time.timeScale = 1;						// 游戏时间倍速 = 1 正常
		PLayerInfo.MODE = 1;                    // 游戏模式Arcade
        PLayerInfo.MapPlayer = player;          // 玩家存档对象
        StartCoroutine(GotoScreen("PlayScene"));// 当前等／／／／／／／／/////／／／／／／    	十大 
	}

	public void SelectMap(int mode)				// mode = 1（街机模式）进入mapScene
    {
        SoundController.Sound.Click();
        if (mode == 1)
            Application.LoadLevel("MapScene");
        else
            HomeScene();

        CameraMovement.StarPointMoveIndex = -1;	// 镜头移动初始坐标索引
    }

    /// <summary>
    /// Go to a scene with name
    /// </summary>
    /// <param name="screen">name of the scene to direction</param>
    IEnumerator GotoScreen(string screen)		// 跳转场景（协程）
    {
        yield return new WaitForSeconds(0);
        Application.LoadLevel(screen);
    }

    public void HomeScene()						// 跳到主场景
    {
        SoundController.Sound.Click();
        Time.timeScale = 1;
        Application.LoadLevel("HomeScene");
    }

    /// <summary>
    /// Set and change state of music
    /// </summary>
    /// <param name="button">Image button</param>
    public void BMusic(UnityEngine.UI.Button button)	//播放／停止 音乐		？传入参数没使用，不知道怎么切换的图片
    {

        if (PlayerPrefs.GetInt("MUSIC", 0) != 1)		// 读取key = MUSIC 的 int值
        {
            PlayerPrefs.SetInt("MUSIC", 1); // music off
        }
        else
        {
            PlayerPrefs.SetInt("MUSIC", 0); // music on
        }

    }
    /// <summary>
    /// Set and change state of sound background
    /// </summary>
    /// <param name="button">Image button</param>
    public void BSound(UnityEngine.UI.Image button)		// 播放／停止 音效
    {
        if (PlayerPrefs.GetInt("SOUND", 0) != 1)
        {
            PlayerPrefs.SetInt("SOUND", 1);
            button.overrideSprite = ButtonSprite[3];	//切换精灵图
        }
        else
        {
            PlayerPrefs.SetInt("SOUND", 0);
            button.overrideSprite = ButtonSprite[2];
        }
    }
}
