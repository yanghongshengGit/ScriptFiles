using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家信息 显示对象脚本
/// </summary>
public class PLayerInfo : MonoBehaviour
{
	//玩家信息(本类）对象（单例）
    public static PLayerInfo Info;      // infomations of player
	//玩家每关数据对象（静态）
	public static Player MapPlayer;     // player object
	//模式Arcade or Classic 二选一（静态）
    public static byte MODE;            // mode : Arcade or Classic 
	//模式背景（静态） 这个数据用在什么地方？
    public static int BACKGROUND;       // background of mode
	//分数
    public int Score;
	//常量字符串"classichightscore"
    public const string KEY_CLASSIC_HISCORE = "classichightscore";
	//等级文本？（文本网格为了3d文字效果）
    public TextMesh textlv;

    void Awake()
    {
		//初始化(本类）对象（单例）
        Info = this;
		//用玩家数据对象中Background 初始化模式背景
        BACKGROUND = MapPlayer.Background;

    }

    void Start()
    {
		//初始化分数 = 0
        Score = 0;
		//初始化特效产生器effect数据--等级
        EffectSpawner.effect.SetLevel(MapPlayer.Level);
		//初始化特效产生器effect数据--现最高分
        EffectSpawner.effect.SetBest(MapPlayer.HightScore);
		//初始化特效产生器effect数据--分数
        EffectSpawner.effect.SetScore(Score);
		//初始化文本网格对象等级文本
        textlv.text = MapPlayer.Level.ToString();
    }
}
