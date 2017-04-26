using UnityEngine;
using System.Collections;

/// <summary>
/// 特效产生器 组件 （挂载到SpawnController游戏对象上）
/// 职责:产生各种特效
/// </summary>
public class EffectSpawner : MonoBehaviour
{
	//特效产生器对象（单例）
    public static EffectSpawner effect;

    /// <summary>
    /// 父容器对象
    /// </summary>
    public GameObject parent;

    /// <summary>
    /// 特效Prefab列表
    /// </summary>
    public GameObject[] EffectPrefabs;

    /// <summary>
    /// 魔法瓶子动画 引用
    /// </summary>
    public Animator redglass;

    /// <summary>
    /// 宝石销毁动画缓存,避免销毁后在重新创建动画对象
    /// </summary>
    public GameObject[,] JewelCrashArray;

    /// <summary>
    /// 宝石销毁动画缓存的父容器，即所有销毁动画均挂在此对象像。
    /// </summary>
    public GameObject JewelCrashParent;

	public UnityEngine.UI.Text level;		// 等级（文本）
	public UnityEngine.UI.Text best;		// 现最高分（文本）
	public UnityEngine.UI.Text Score;		// 分数（文本）
	public UnityEngine.UI.Image Energy;		// 能量球（图片）

    public float REFRESH_COMBO_TIME = 2f;			// 刷新一次效果列表时间？

    private const float BOOM_TIME = 0.5f;			// 爆炸效果时间

    private const float ICECRASH_TIME = 0.5f;		// 雪崩效果时间

    private const float JEWELCASH_TIME = 0.35f;		// 宝石消除效果时间

    private const float SCORESHOW_TIME = 0.5f;		// 分数小说效果时间

    private const float THUNDER_TIME = 0.4f;		// 雷电效果时间

    private const float FILEARROW_TIME = 0.4f;		// 火焰箭效果时间

    private int ComboCount = 0;			// 效果组合计数

	private int ThunderCount = 0;		// 火球效果计数

    private int PowerCount = 0;         // 技能计数

    public float ComboCountdown;        // 下落刷新时间

    float EnergyStack = 0;				// 能量条当前值

	bool isEnergyInc;					// 能量条是否增加

    void Awake()
    {
		// 单例
        effect = this;
		// 为宝石消除动画数组申请空间
        JewelCrashArray = new GameObject[7, 9];
    }
	// 更新刷新时间
    public void ContinueCombo()
    {
        ComboCountdown = REFRESH_COMBO_TIME;
    }
	// 增加特效计数
    public void ComBoInc()
    {
        ComboCount++;
    }
	// 分数增加
    public void ScoreInc(Vector3 pos)
    {
        int scorebonus = 10 + ComboCount * 10;		// 奖励分
        if (PLayerInfo.MODE != 1)					// 街机模式
        {
			// 如果当前分数 < 极限分数
            if (PLayerInfo.Info.Score < PLayerInfo.MapPlayer.Level * 5000)
                Timer.timer.ScoreBarProcess(scorebonus);	// 分数进度增加
			// 如果还在游戏中
            else if (GameController.action.GameState == (int)Timer.GameState.PLAYING)
            {
                Timer.timer.ClassicLvUp();					// 赢了，提升玩家等级
            }
        }
        else 										// 传统模式
        {
			// 如果还在游戏中
            if (GameController.action.GameState == (int)Timer.GameState.PLAYING)
                PLayerInfo.Info.Score += scorebonus;		// 增加分数
            BonusEffect();		// 添加奖励特效
            MiniStar(pos);		// 播放闪星特效
        }

        ScoreEff(scorebonus, pos);					// 分数增加特效
        SetScore(PLayerInfo.Info.Score);			// 设置现在分数
    }
	// 添加奖励特效
    private void BonusEffect()
    {
        ThunderCount++;					// 火球特效计数+1
        PowerCount++;
        EnergyStack += 1 / 21f;			// 能量条+ 1/21
        EnergyInc();					// 能量条增长
		if (ThunderCount >= 21)
        {
            GameController.action.DestroyRandom();	// 下落 播放特效 生成新序列
			ThunderCount = 0;						// 置空
            Energy.fillAmount = 0;                  // 能量进度置0
            EnergyStack = 0;
        }
        if (PowerCount >= 32)
        {
            PowerCount = 0;
            GameController.action.isAddPower = true;
        }
    }

    private void EnergyInc()
    {
        if (!isEnergyInc)
            StartCoroutine(IEEnergyInc());
    }

    IEnumerator IEEnergyInc()
    {
        // //Debug.Log (Energy.gameObject.transform.position);
        isEnergyInc = true;
        float d = 1 / 210f;
        while (EnergyStack > 0)
        {
            Energy.fillAmount += d;
            EnergyStack -= d;
            yield return null;
            if (Energy.fillAmount == 1)
                Energy.fillAmount = 0;
        }
        EnergyStack = 0;
        isEnergyInc = false;
    }

    private void ScoreEff(int score, Vector3 pos)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[4]);
        tmp.transform.GetChild(0).GetComponent<TextMesh>().text = score.ToString();
        tmp.transform.SetParent(parent.transform, false);
        tmp.transform.position = new Vector3(pos.x, pos.y, tmp.transform.position.z);
        Destroy(tmp, SCORESHOW_TIME);
    }

    public void SetLevel(int lv)
    {
        level.text = lv.ToString();
    }

    public void SetBest(int bestscore)
    {
        best.text = bestscore.ToString();
    }

    public void SetScore(int _score)
    {
        Score.text = _score.ToString();
    }

    /// <summary>
    /// 创建宝石销毁动画并返回
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject JewelCash(Vector3 pos)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[0]);
        tmp.transform.SetParent(JewelCrashParent.transform, false);
        tmp.transform.localPosition = new Vector3(pos.x, pos.y, -0.2f);
        return tmp;
        // Destroy(tmp, JEWELCASH_TIME);
    }

    public void Thunder(Vector3 pos)
    {
        // Debug.Break();
        //GameObject tmp = (GameObject)Instantiate(EffectPrefabs[3]);
        //tmp.transform.SetParent(parent.transform, false);
        //tmp.transform.position = new Vector3 (pos.x,pos.y,-2.1f);
        //Destroy(tmp, THUNDER_TIME);
        MGE(Energy.transform.position, pos, -0.4f);
    }

    /// <summary>
    /// 销毁动画
    /// </summary>
    /// <param name="pos"></param>
    public void boom(Vector3 pos)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[1]);
        SoundController.Sound.Boom();
        tmp.transform.SetParent(parent.transform, false);   //播放在特效层中
        tmp.transform.position = pos;
        Destroy(tmp, BOOM_TIME);    //延时销毁
    }

    /// <summary>
    /// (buff效果)
    /// 播放爆炸动画
    /// </summary>
    /// <param name="obj"></param>
    public void Enchant(GameObject obj)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[2]);
        tmp.transform.SetParent(obj.transform, false);
    }

    /// <summary>
    /// (buff效果)
    /// 在宝石的第一个子对像下产生一个Buff对象
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="power"></param>
    public void ThunderRow(GameObject obj, int power)
    {
        // 实例化火箭触发特效
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[5]);
        // 设置父节点
        tmp.transform.SetParent(obj.transform.GetChild(0).transform, false);
        // 如果技能类型是纵向火箭
        if (power == 3)
            // 设置特效角度为纵向
            tmp.transform.localEulerAngles = new Vector3(0, 0, 90);
    }

    /// <summary>
    /// 播放火攻击，行或列
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="c"></param>
    public void FireArrow(Vector3 pos, bool c)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[6]);
        tmp.transform.SetParent(parent.transform, false);  //播放在特效层中
        tmp.transform.position = new Vector3(pos.x, pos.y, -2.2f);
        if (c)
            tmp.transform.localEulerAngles = new Vector3(0, 0, 90);
        Destroy(tmp, FILEARROW_TIME);
    }

    /// <summary>
    /// (buff效果)
    /// 产生一个时钟技能
    /// </summary>
    /// <param name="obj"></param>
    public void Clock(GameObject obj)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[7]);
        tmp.transform.SetParent(obj.transform.GetChild(0).transform, false);
    }

    public void StarWinEffect(Vector3 pos)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[8]);
        tmp.transform.SetParent(parent.transform, false);  //播放在特效层中
        tmp.transform.position = new Vector3(pos.x, pos.y, tmp.transform.position.z);
        Animation anim = tmp.GetComponent<Animation>();
        StarEffectAnim(anim, tmp);
        Destroy(tmp, 1f);

    }

    /// <summary>
    /// 在指定的格子上播放冰冻动画
    /// </summary>
    /// <param name="pos"></param>
    public void IceCrash(Vector2 pos)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[9]);
        tmp.transform.SetParent(parent.transform, false);  //播放在特效层中
        tmp.transform.position = GribManager.cell.GribCell[(int)pos.x, (int)pos.y].transform.position;
        Destroy(tmp, ICECRASH_TIME);//延时销毁

    }

    /// <summary>
    /// 在指定的格子上播放锁动画
    /// </summary>
    /// <param name="pos"></param>
    public void LockCrash(Vector2 pos)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[10]);
        tmp.transform.SetParent(parent.transform, false);  //播放在特效层中
        tmp.transform.position = GribManager.cell.GribCell[(int)pos.x, (int)pos.y].transform.position;
        Destroy(tmp, ICECRASH_TIME);//延时销毁
    }

    void StarEffectAnim(Animation anim, GameObject tmp)
    {
        //Debug.Break();
        anim.enabled = true;
        AnimationClip animclip = new AnimationClip();
#if UNITY_5
                animclip.legacy = true;
#endif
        AnimationCurve curveScalex = AnimationCurve.Linear(0, tmp.transform.localScale.x, 1, 3);
        //AnimationCurve curveScaley = AnimationCurve.Linear(0, tmp.transform.localScale.y, 1, 3);
        AnimationCurve curvex = AnimationCurve.Linear(0, tmp.transform.position.x, 1, 0);
        AnimationCurve curvey = AnimationCurve.Linear(0, tmp.transform.position.y, 1, 0);
        AnimationCurve curvez = AnimationCurve.Linear(0, tmp.transform.position.z, 1, tmp.transform.position.z);
        AnimationCurve curveColora = AnimationCurve.Linear(0, 1, 1, 0);

        animclip.SetCurve("", typeof(Transform), "m_LocalScale.x", curveScalex);
        animclip.SetCurve("", typeof(Transform), "m_LocalScale.y", curveScalex);
        animclip.SetCurve("", typeof(Transform), "localPosition.x", curvex);
        animclip.SetCurve("", typeof(Transform), "localPosition.y", curvey);
        animclip.SetCurve("", typeof(Transform), "localPosition.z", curvez);
        animclip.SetCurve(tmp.transform.GetChild(0).name, typeof(SpriteRenderer), "m_Color.a", curveColora);
        // animclip.SetCurve("", typeof(Animation), "m_Enabled", curvenable);
        anim.wrapMode = WrapMode.Once;
        anim.AddClip(animclip, "Startwin");
        anim.Play("Startwin");
    }

    public IEnumerator ComboTick()
    {
        while (true)
        {
            if (ComboCountdown > 0)
                ComboCountdown -= Time.deltaTime;
            else
                ComboCount = 0;
            yield return null;
        }
    }

    // 播放火球特效
    public GameObject MGE(Vector3 pos, Vector3 target)
    {
        // 实例化火球特效
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[11]);
        // 设置特效父节点
        tmp.transform.SetParent(parent.transform, false);
        // 设置特效坐标
        tmp.transform.position = new Vector3(pos.x, pos.y, -0.22f);
        // 火球转向弧度
        float AngleRad = Mathf.Atan2(target.y - pos.y, target.x - pos.x);
        // 火球转向角度
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        // 设置火球旋转角度
        tmp.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
        // 火球移动到目标位置
        Ulti.MoveTo(tmp, target, 0.4f);
        // 销毁特效
        Destroy(tmp, 0.4f);
        // 播放射击音效
        SoundController.Sound.Gun();
        // 返回特效
        return tmp;
    }

    public GameObject MGE(Vector3 pos, Vector3 target, float z)
    {

        GameObject tmp = MGE(pos, target);
        tmp.transform.position += new Vector3(pos.x, pos.y, z);
        return tmp;
    }

    /// <summary>
    /// 播放积分特效
    /// </summary>
    public void glass()
    {
        if (PLayerInfo.MODE == 1)
            redglass.enabled = true;
        //redglass.Play("glass");
        ////Debug.Log("bla");
    }

    public void MiniStar(Vector3 startpos)
    {
        GameObject tmp = (GameObject)Instantiate(EffectPrefabs[12]);
        tmp.transform.SetParent(parent.transform, false);
        Ulti.MoveTo(tmp, startpos, new Vector2(-2.485f, 4.418f), 1.2f, -2.2f);
        Destroy(tmp, 1.2f);
    }
}
