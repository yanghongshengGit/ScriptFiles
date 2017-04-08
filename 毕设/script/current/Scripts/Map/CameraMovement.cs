using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 镜头移动组件 （挂在Main Camera游戏对象上）
/// 
/// </summary>
public class CameraMovement : MonoBehaviour
{
	// 镜头移动对象（单例）
    public static CameraMovement mcamera;       // camera movement
    // 星星坐标索引
    public static int StarPointMoveIndex;       // position index
    // 大地图容器
    public RectTransform container;             // container of scroll view

    /// <summary>
    /// PopUp 游戏对象 来自Canvas下的 PopUp,游戏弹出面板对象
    /// 注意这个PopUp是挂在Canvas下的也就是镜头无论如何移动，Canvas面板下的东西则不需要移动。
    /// </summary>
    public GameObject PopUp;                    // popup show when click to item button level

    /// <summary>
    /// StartPoint 游戏对象 来自Screen下的StartPoint
    /// </summary>
    public GameObject StarPoint;                // position start

    /// <summary>
    /// 星星Sprite状态数组 
    /// </summary>
    public Sprite[] star;                       // arrays star of item level

    /// <summary>
    /// 切换场景时的渐变动画
    /// </summary>
    public GameObject fade;                     // fade animation

    float distance = 90.8f / 8680f;

    public static bool movement;

    public static bool setstate;

    public bool isPopup;						// 是否已弹出窗口


    Player map;									// 存档对象


    void Awake()
    {
        mcamera = this;
    }

    void Start()
    {
        setLastpos();
        SetPoint();
        //GoogleMobileAdsScript.advertise.HideBanner();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isPopup)    // 按下Esc且已弹出弹窗
        {
            UnfreezeMap();                                  // 隐藏弹窗
        }
        else if (Input.GetKeyDown(KeyCode.Escape))          // 按下Esc
        {
            ButtonActionController.Click.HomeScene();       // 跳转到开始场景
        }
    }

    /// <summary>
    /// set last position of container
    /// </summary>
    void setLastpos()   // 设置相机结束位置
    {
        // 获取相机开始y坐标
        float lastp = PlayerPrefs.GetFloat("LASTPOS", 0);
        // 相机位置范围0-90
        if (lastp < 0) lastp = 0;
        else if (lastp > 90.8000f) lastp = 90.8f;
        // 相机位置
        transform.position += new Vector3(0, lastp);
        // 设置地图锚点位置
        container.anchoredPosition = new Vector2(container.anchoredPosition.x, -lastp / distance + 4740f);
    }

    void SetPoint()     // 设置星结束位置
    {
        // x坐标
        float x = PlayerPrefs.GetFloat("LASTPOSX", -0.0045f);
        // y坐标
        float y = PlayerPrefs.GetFloat("LASTPOS", -3.587f);
        // 设置星坐标
        StarPoint.transform.position = new Vector3(x, y, StarPoint.transform.position.z);

    }

    /// <summary>
    /// Update positio camera when scroller
    /// </summary>
    public void CameraPosUpdate()   // 更新相机位置
    {
        // 设置相机位置
        transform.position = new Vector3(transform.position.x, -(container.anchoredPosition.y - 4740f) * distance, transform.position.z);
        // 如果鼠标按下
        if (setstate)
            // 可以移动
            movement = true;
    }


    /// <summary>
    /// show infomation of level player
    /// </summary>
    /// <param name="_map"></param>
    public void PopUpShow(Player _map)  // 弹窗显示玩家等级等信息
    {
        // 显示弹窗tag
        isPopup = true;
        // 冻结地图
        CameraMovement.mcamera.FreezeMap();
        // 给player数据对象赋值
        map = _map;
        // 三张星星图
        Image[] stars = new Image[3];

        //直接访问PopUp中的三个星星组件
        stars[0] = PopUp.transform.GetChild(1).GetComponent<Image>();
        stars[1] = PopUp.transform.GetChild(2).GetComponent<Image>();
        stars[2] = PopUp.transform.GetChild(3).GetComponent<Image>();

        //设置星星状态
        for (int i = 0; i < 3; i++)
        {
            if (i < _map.Stars)
                stars[i].sprite = star[0];
            else
                stars[i].sprite = star[1];
        }

        // 显示当前关卡最高分
        PopUp.transform.GetChild(4).GetComponent<Text>().text = _map.HightScore.ToString();
        // 显示等级
        PopUp.transform.GetChild(6).GetComponent<Text>().text = _map.Level.ToString("00");
        // 播放放大动画
        Animation am = PopUp.GetComponent<Animation>();
        am.enabled = true;
        // 显示弹窗
        PopUp.SetActive(true);
    }
    // 跳转到街机模式场景
    public void ArcadeScene()
    {
        ButtonActionController.Click.ArcadeScene(map);
    }
    // 冻结地图
    public void FreezeMap()
    {
        // 数据加载未完成
        DataLoader.enableclick = false;
        // 开启渐变物理碰撞检测组件
        fade.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    // 隐藏弹窗
    public void UnfreezeMap()
    {
        // 播放点击音效
        SoundController.Sound.Click();
        // 隐藏弹窗对象
        PopUp.SetActive(false);
        // 弹窗隐藏tag
        isPopup = false;
        // 数据加载完成
        DataLoader.enableclick = true;
        // 禁用渐变的物理碰撞检测组件
        fade.GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

}
