using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 控制器游戏对象 组件 (挂在 GameController游戏对象下)
/// </summary>
public class GameController : MonoBehaviour
{
    // 游戏控制对象（单例）
    public static GameController action;
    // 下落速度
    public static float DROP_SPEED = 8;
    // 下落延迟时间
    public static float DROP_DELAY = 0.5f;
    // 当前状态（游戏中，暂停，）
    public int GameState;
    // 不是空的位置的数量
    public int CellNotEmpty;
    // 选中框
    public GameObject Selector;
    // 特殊图块技能
    public enum Power
    {
        BOOM = 1,               //爆炸
        ROW_LIGHTING = 2,       //消除行
        COLLUMN_LIGHTING = 3,   //消除列
        MAGIC = 8,
        TIME = 4,               //追加时间
    }
    // 下落生成控制器
    public SpawnController drop;
    // 禁选框
    public GameObject NoSelect;
    // 星形图块（结束图块）
    public JewelObj JewelStar;
    // 是否是星图块
    public bool isStar;
    // 是否显示星图块
    public bool isShowStar;
    // 是否添加技能
    public bool isAddPower;
    // 是否播放入场动画
    public Animation StartAnim;
    // ??
    private JewelObj JewelScript;
    private JewelObj JewelScript1;
    // 点击图块
    private GameObject Pointer;
    // 选中的图块
    private GameObject Selected;
    // 鼠标按下过程中 = true
    bool ishold;
    void Awake()
    {
        action = this;
    }
    // 协程初始化
    IEnumerator Start()
    {
        // 街机模式
        if (PLayerInfo.MODE ==  1)
            // 根据关卡名字创建表格
            StartCoroutine(GribManager.cell.GribMapCreate(PLayerInfo.MapPlayer.Name));
        else
            // 传统模式
            StartCoroutine(GribManager.cell.GribMapCreate("classic"));
        // 等1.5s
        yield return new WaitForSeconds(1.5f);
        // 等待特效时间
        StartCoroutine(EffectSpawner.effect.ComboTick());
        // 开启计时
        Timer.timer.TimeTick(true);
        // 设置当前状态为 正在游戏中
        GameState = (int)Timer.GameState.PLAYING;
        // 隐藏禁选框
        NoSelect.SetActive(false);
    }

    void Update()
    {
        // 等待玩家操作
        JewelSelecter();
        backpress();
    }
    //process click action
    void JewelSelecter()
    {
        // 鼠标左键按下
        if (Input.GetMouseButtonDown(0))
        {
            // 按下中
            ishold = true;
            // 
            if (Pointer == null)
            {
                // 获取点击图块对象
                Pointer = JewelTouchChecker(Input.mousePosition);
            }
            // 停止提示动画
            Supporter.sp.StopSuggestionAnim();
            // 如果不是宝石对象
            if (Pointer != null && !Pointer.name.Contains("Jewel"))
                Pointer = null;
        }
        // 鼠标左键按下没松
        else if (Input.GetMouseButton(0) && ishold)
        {
            // 如果点击图块不是空
            if (Pointer != null)
            {
                // 显示选择框
                EnableSelector(Pointer.transform.position);
                // 获取当前鼠标位置的宝石对象
                Selected = JewelTouchChecker(Input.mousePosition);
                // 如果当前宝石对象不为空且原来宝石对象不为空
                if (Selected != null && Pointer != Selected && Selected.name.Contains("Jewel"))
                {
                    // 检查两个宝石对象的距离是否符合要求
                    if (DistanceChecker(Pointer, Selected))
                    {
                        // 检查消除
                        RuleChecker(Pointer, Selected);
                        // 清空对象
                        Pointer = null;
                        Selected = null;
                        // 隐藏选择框
                        Selector.SetActive(false);
                    }
                    else
                    {
                        // 将鼠标当前位置对象作为点击对象
                        Pointer = Selected;
                        // 当前选择为空
                        Selected = null;
                        // 显示选择框
                        EnableSelector(Pointer.transform.position);
                    }
                }
            }
        }
        // 鼠标左键弹起
        else if (Input.GetMouseButtonUp(0))
        {
            // 保持按下标志为false
            ishold = false;
        }
    }
    //检查两个宝石对象的距离是否<=1
    bool DistanceChecker(GameObject obj1, GameObject obj2)
    {
        // 获取宝石对象坐标
        Vector2 v1 = obj1.GetComponent<JewelObj>().jewel.JewelPosition;
        Vector2 v2 = obj2.GetComponent<JewelObj>().jewel.JewelPosition;
        // 距离是否<=1
        if (Vector2.Distance(v1, v2) <= 1)
        {
            return true;
        }

        return false;
    }
    //消除检查
    public void RuleChecker(GameObject obj1, GameObject obj2)
    {
        // 获取对象上的宝石对象脚本
        JewelObj Jewel1 = obj1.GetComponent<JewelObj>();
        JewelObj Jewel2 = obj2.GetComponent<JewelObj>();

        LogUtils.TraceNow("Pointer:" + Jewel1.jewel.JewelPosition.x + "," + Jewel1.jewel.JewelPosition.y);
        LogUtils.TraceNow("Selected:" + Jewel2.jewel.JewelPosition.x + "," + Jewel2.jewel.JewelPosition.y);

        // (以obj2为中心)将横向要消除的对象列表与纵向要消除的对象列表合并
        List<JewelObj> NeiObj1 = Ulti.ListPlus(
            Jewel1.GetCollumn(Jewel2.jewel.JewelPosition, Jewel1.jewel.JewelType, null),
            Jewel1.GetRow(Jewel2.jewel.JewelPosition, Jewel1.jewel.JewelType, null),
            Jewel1);
        // (以obj1为中心)将横向要消除的对象列表与纵向要消除的对象列表合并
        List<JewelObj> NeiObj2 = Ulti.ListPlus(Jewel2.GetCollumn(Jewel1.jewel.JewelPosition, Jewel2.jewel.JewelType, null),
                                         Jewel2.GetRow(Jewel1.jewel.JewelPosition, Jewel2.jewel.JewelType, null), Jewel2);


        // 如果obj1或obj2的类型是星形
        if (Jewel1.jewel.JewelType == 99 || Jewel2.jewel.JewelType == 99)
            // 同时obj1或obj2的类型是特殊技能-清除相同图块
            if (Jewel1.jewel.JewelType == 8 || Jewel2.jewel.JewelType == 8)
            {
                // 播放移动无效的动画
                Jewel1.SetBackAnimation(obj2);
                Jewel2.SetBackAnimation(obj1);
                return;
            }
        // obj1或obj2消除列表>=3, 或 obj1或obj2的类型是特殊技能-清除相同图块
        if (NeiObj1.Count >= 3 || NeiObj2.Count >= 3 || Jewel1.jewel.JewelType == 8 || Jewel2.jewel.JewelType == 8)
        {
            // 交换obj1,obj2位置
            Ulti.MoveTo(obj1, obj2.transform.localPosition, 0.2f);
            Ulti.MoveTo(obj2, obj1.transform.localPosition, 0.2f);
            // 交换obj1,obj2在数组中位置
            SwapJewelPosition(obj1, obj2);
            // 进行消除运算
            JewelProcess(NeiObj1, NeiObj2, obj1, obj2);
        }
        else
        {
            // 播放移动无效的动画
            Jewel1.SetBackAnimation(obj2);
            Jewel2.SetBackAnimation(obj1);
        }
    }

    // 按Esc键返回
    void backpress()
    {
        // 按Esc时正在游戏状态
        if (Input.GetKeyDown(KeyCode.Escape) && GameState == (int)Timer.GameState.PLAYING)
        {
            // 暂停计时
            Timer.timer.Pause();
        }
        // 按Esc时正在暂停状态
        else if (Input.GetKeyDown(KeyCode.Escape) && GameState == (int)Timer.GameState.PAUSE)
        {
            // 恢复计时
            Timer.timer.Resume();
        }
    }
    // 宝石消除运算过程
    void JewelProcess(List<JewelObj> list1, List<JewelObj> list2, GameObject obj1, GameObject obj2)
    {
        // 获取消除列表数量
        int c1 = list1.Count;
        int c2 = list2.Count;
        // 如果obj1消除数量>2
        if (c1 > 2)
        {
            // 进入分级消除
            ListProcess(list1, obj2, obj1, obj1.GetComponent<JewelObj>().jewel.JewelType);
        }
        // 或是obj1的类型为特殊技能-消除相同
        else if (obj1.GetComponent<JewelObj>().jewel.JewelType == 8)
        {
            // 消除原本对象
            obj2.GetComponent<JewelObj>().Destroy();
            // 消除相同类型对象
            PDestroyType(obj2.GetComponent<JewelObj>().jewel.JewelType, obj2.transform.position);
            // 消除交换的对象
            obj1.GetComponent<JewelObj>().Destroy();
        }
        // obj2消除数量>2
        if (c2 > 2)
        {
            // 进入分级消除
            ListProcess(list2, obj1, obj2, obj2.GetComponent<JewelObj>().jewel.JewelType);
        }
        // 或是obj2的类型为特殊技能-消除相同
        else if (obj2.GetComponent<JewelObj>().jewel.JewelType == 8)
        {
            // 消除原本对象
            obj1.GetComponent<JewelObj>().Destroy();
            // 消除相同类型对象
            PDestroyType(obj1.GetComponent<JewelObj>().jewel.JewelType, obj1.transform.position);
            // 消除交换的对象
            obj2.GetComponent<JewelObj>().Destroy();
        }

    }
    // 宝石消除运算过程(重载)
    public void JewelProcess(List<JewelObj> list1, GameObject obj1)
    {
        int c1 = list1.Count;
        if (c1 > 2)
        {
            ListProcess(list1, obj1, null, obj1.GetComponent<JewelObj>().jewel.JewelType);
        }

    }
    // 对要消除数量分级，根据级别消除
    bool ListProcess(List<JewelObj> list, GameObject obj, GameObject obj1, int type)
    {
        Vector3 v;// 宝石对象坐标
        // 交换的对象
        if (obj1 != null)
        {
            // 获取对象坐标
            JewelScript = obj1.GetComponent<JewelObj>();
            v = new Vector3(JewelScript.jewel.JewelPosition.x, JewelScript.jewel.JewelPosition.y);
        }
        // 本对象
        else
        {
            // 获取对象坐标
            JewelScript = obj.GetComponent<JewelObj>();
            v = new Vector3(JewelScript.jewel.JewelPosition.x, JewelScript.jewel.JewelPosition.y);
        }
        // 获取列表数量
        int c = list.Count;
        if (c == 3)
        {
            // 消除宝石 生成新宝石
            DestroyJewel(list);
            // 增加特效计数
            EffectSpawner.effect.ComBoInc();
            // 开启滑落检测
            dropjewel();
            return false;
        }
        else if (c == 4)
        {
            // 移动并消除宝石 生成新宝石
            ReGroup(list, type, (int)Power.BOOM, v);
            // 有限随机消除一个宝石对象
            DestroyRandom();
            // 增加特效计数
            EffectSpawner.effect.ComBoInc();
            // 开启滑落检测
            dropjewel();
        }
        else if (c >= 5)
        {
            // 移动并消除宝石 生成新宝石类型五彩
            ReGroup(list, 8, (int)Power.MAGIC, v);
            // 增加特效计数
            EffectSpawner.effect.ComBoInc();
            // （有限随机）随机删除一个宝石
            DestroyRandom();
            // （有限随机）随机删除一个宝石
            DestroyRandom();
            // 开启下落检查
            dropjewel();
        }

        return true;
    }

    /// <summary>
    /// 开启滑落检测
    /// </summary>
    void dropjewel()
    {
        // 下落时间
        drop.DELAY = DROP_DELAY;
        // 启动updata检查下落
        drop.enabled = true;
    }
    // 消除宝石
    void DestroyJewel(List<JewelObj> list)
    {
        // 播放消除音效
        //SoundController.Sound.JewelCrash();
        // 播放积分特效
        EffectSpawner.effect.glass();
        // 遍历要消除宝石列表
        foreach (var item in list)
        {
            // 写入log
            LogUtils.TraceNowJewelObj(item);
            // 销毁对象
            item.Destroy();
        }
    }
    // 消除宝石
    void ReGroup(List<JewelObj> list, int type, int power, Vector2 pos)
    {
        // 播放消除音效
        SoundController.Sound.JewelCrash();
        // 播放积分特效
        EffectSpawner.effect.glass();
        // 遍历要消除宝石列表
        foreach (var item in list)
        {
            // 移动消除宝石
            item.ReGroup(pos);
        }
        // 生成有技能的宝石对象
        StartCoroutine(SpawnJewelPower(type, power, pos));
    }
    // 获取鼠标当前位置的宝石对象，参数鼠标位置
    GameObject JewelTouchChecker(Vector3 mouseposition)
    {
        // 获取鼠标当前坐标
        Vector3 wp = Camera.main.ScreenToWorldPoint(mouseposition);
        // 转化为二维坐标
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        // 检测是否有碰撞
        if (Physics2D.OverlapPoint(touchPos))
        {
            // 返回碰撞的宝石对象
            return Physics2D.OverlapPoint(touchPos).gameObject;
        }
        return null;
    }

    //swap map jewel position
    /// <summary>
    /// 交换Map中的宝石位置
    /// </summary>
    /// <param name="jewel1"></param>
    /// <param name="jewel2"></param>
    void SwapJewelPosition(GameObject jewel1, GameObject jewel2)
    {
        // 获取对象上的宝石对象脚本
        JewelObj tmp1 = jewel1.GetComponent<JewelObj>();
        JewelObj tmp2 = jewel2.GetComponent<JewelObj>();

        //交互宝时对象在Map中的位置
        Vector2 tmp = tmp1.jewel.JewelPosition;
        tmp1.jewel.JewelPosition = tmp2.jewel.JewelPosition;
        tmp2.jewel.JewelPosition = tmp;

        //交换对象
        GameObject Objtmp = JewelSpawner.spawn.JewelGrib[(int)tmp1.jewel.JewelPosition.x, (int)tmp1.jewel.JewelPosition.y];
        JewelSpawner.spawn.JewelGrib[(int)tmp1.jewel.JewelPosition.x, (int)tmp1.jewel.JewelPosition.y] = jewel2;
        JewelSpawner.spawn.JewelGrib[(int)tmp2.jewel.JewelPosition.x, (int)tmp2.jewel.JewelPosition.y] = Objtmp;

        //交换脚本
        JewelObj scripttmp = tmp1;
        JewelSpawner.spawn.JewelGribScript[(int)tmp2.jewel.JewelPosition.x, (int)tmp2.jewel.JewelPosition.y] = tmp2;
        JewelSpawner.spawn.JewelGribScript[(int)tmp1.jewel.JewelPosition.x, (int)tmp1.jewel.JewelPosition.y] = scripttmp;
        if (tmp1.jewel.JewelType == 99 || tmp2.jewel.JewelType == 99)
            WinChecker();

    }
    // 生成新宝石
    IEnumerator SpawnJewelPower(int type, int power, Vector2 pos)
    {
        // 等0.4f
        yield return new WaitForSeconds(0.4f);
        // 生成特效
        GameObject tmp = JewelSpawner.spawn.SpawnJewelPower(type, power, pos);
        // 等0.2f
        yield return new WaitForSeconds(0.2f);
        // 恢复碰撞器
        tmp.GetComponent<Collider2D>().enabled = true;
    }


    /// <summary>
    /// 播放格子除动画特效
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CellRemoveEffect(int x, int y)
    {
        // 播放左边格子特效
        if (x - 1 >= 0 && GribManager.cell.GribCellObj[x - 1, y] != null)
            GribManager.cell.GribCellObj[x - 1, y].RemoveEffect();
        // 播放右边格子特效
        if (x + 1 < 7 && GribManager.cell.GribCellObj[x + 1, y] != null)
            GribManager.cell.GribCellObj[x + 1, y].RemoveEffect();
        // 播放下边格子特效
        if (y - 1 >= 0 && GribManager.cell.GribCellObj[x, y - 1] != null)
            GribManager.cell.GribCellObj[x, y - 1].RemoveEffect();
        // 播放上边格子特效
        if (y + 1 < 9 && GribManager.cell.GribCellObj[x, y + 1] != null)
            GribManager.cell.GribCellObj[x, y + 1].RemoveEffect();
    }

    /// <summary>
    /// 销毁一行
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="y"></param>
    public void PDestroyRow(int _x, int y)
    {
        // 开始检查下落
        dropjewel();
        // 播放火箭音效
        SoundController.Sound.Fire();
        // 格子列表
        List<CellObj> celleffect = new List<CellObj>();
        // 宝石列表
        List<JewelObj> jeweldes = new List<JewelObj>();
        // 遍历横坐标
        for (int x = 0; x < 7; x++)
        {
            // 除了该对象
            if (_x != x)
            {
                // 这个位置的格子添加到格子列表
                if (GribManager.cell.GribCellObj[x, y] != null && GribManager.cell.GribCellObj[x, y].cell.CellEffect > 0)
                    celleffect.Add(GribManager.cell.GribCellObj[x, y]);
                // 这个位置的宝石添加到宝石列表
                if (JewelSpawner.spawn.JewelGribScript[x, y] != null && JewelSpawner.spawn.JewelGribScript[x, y].jewel.JewelType != 99 && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 0)
                    jeweldes.Add(JewelSpawner.spawn.JewelGribScript[x, y]);
            }
        }
        // 遍历格子列表
        foreach (CellObj item in celleffect)
        {
            // 播放特效并移除
            item.RemoveEffect();
        }
        // 遍历宝石列表
        foreach (JewelObj item in jeweldes)
        {
            // 销毁对象
            item.Destroy();
        }
    }

    /// <summary>
    /// 销毁一列
    /// </summary>
    /// <param name="x"></param>
    /// <param name="_y"></param>
    public void PDestroyCollumn(int x, int _y)
    {
        // 开始检查下落
        dropjewel();
        // 播放火箭音效
        SoundController.Sound.Fire();
        // 格子列表
        List<CellObj> celleffect = new List<CellObj>();
        // 宝石列表
        List<JewelObj> jeweldes = new List<JewelObj>();
        // 遍历纵坐标
        for (int y = 0; y < 9; y++)
        {
            // 除了该对象
            if (_y != y)
            {
                // 这个位置的格子添加到格子列表
                if (GribManager.cell.GribCellObj[x, y] != null && GribManager.cell.GribCellObj[x, y].cell.CellEffect > 0)
                    celleffect.Add(GribManager.cell.GribCellObj[x, y]);
                // 这个位置的宝石添加到宝石列表
                if (JewelSpawner.spawn.JewelGribScript[x, y] != null && JewelSpawner.spawn.JewelGribScript[x, y].jewel.JewelType != 99 && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 0)
                    jeweldes.Add(JewelSpawner.spawn.JewelGribScript[x, y]);
            }
        }
        // 遍历格子列表
        foreach (CellObj item in celleffect)
        {
            // 播放特效并移除
            item.RemoveEffect();
        }
        // 遍历宝石列表
        foreach (JewelObj item in jeweldes)
        {
            // 销毁对象
            item.Destroy();
        }
    }
    // 爆炸消除
    public void PBoom(int x, int y)
    {
        // 开始检查下落
        dropjewel();
        // 以该对象为中心遍历周围
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
            {
                // 除了该对象
                if (i != x || j != y)
                    // 没有越界，不为空，且不是星星宝石
                    if (i >= 0 && i < 7 && j >= 0 && j < 9 && JewelSpawner.spawn.JewelGribScript[i, j] != null && JewelSpawner.spawn.JewelGribScript[i, j].jewel.JewelType != 99)
                        // 销毁宝石
                        JewelSpawner.spawn.JewelGribScript[i, j].Destroy();
            }
    }
    // 消除相同类型对象
    public void PDestroyType(int type, Vector3 pos)
    {
        // 消除相同类型对象（协程）
        StartCoroutine(DestroyType(type, pos));
    }
    // 消除相同类型对象
    IEnumerator DestroyType(int type, Vector3 pos)
    {
        // 激活禁选框
        NoSelect.SetActive(true);
        // 开启滑落检测
        dropjewel();
        // 遍历宝石列表
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                JewelObj tmp = JewelSpawner.spawn.JewelGribScript[x, y];
                if (tmp != null && tmp.jewel.JewelType == type)
                {
                    // 播放特效
                    EffectSpawner.effect.MGE(pos, JewelSpawner.spawn.JewelGrib[x, y].transform.position);
                    // 销毁宝石对象
                    tmp.Destroy();

                }

            }
        }
        // 等0.2s
        yield return new WaitForSeconds(0.2f);
        // 隐藏禁选框
        NoSelect.SetActive(false);
    }

    /// <summary>
    /// 为本局游戏追加时间
    /// </summary>
    public void PBonusTime()
    {
        StartCoroutine(TimeInc());
    }
    // 随机消除一个宝石
    public void DestroyRandom()
    {
        //uu tien destroy ganh
        dropjewel();// 开启下落检查
        // 如果是街机模式
        if (PLayerInfo.MODE == 1)
        {
            // 如果没有星星宝石
            if (!isStar)
            {
                // 获取有特效的格子列表
                List<CellObj> listeff = getListCellEffect();
                // 如果有这样的格子
                if (listeff.Count > 0)
                {
                    // 从列表中随机获取一个格子
                    CellObj tmp = listeff[Random.Range(0, listeff.Count)];
                    // 播放并移除特效
                    tmp.RemoveEffect();
                    // 播放火球特效
                    EffectSpawner.effect.Thunder(GribManager.cell.GribCell[(int)tmp.cell.CellPosition.x, (int)tmp.cell.CellPosition.y].transform.position);
                }
                else
                {
                    // 从特殊格子中随机一个消除
                    destroynotempty();
                }

            }
            else
            {
                // 星星下方随机一个图块坐标
                Vector2 vtmp = posUnderStar();
                // 根据坐标获取宝石对象
                JewelObj tmp = JewelSpawner.spawn.JewelGribScript[(int)vtmp.x, (int)vtmp.y];
                // 如果该宝石对象不为空且不是星星宝石
                if (tmp != null && tmp != JewelStar)
                {
                    // 销毁该对象
                    tmp.Destroy();
                    // 播放火球特效
                    EffectSpawner.effect.Thunder(GribManager.cell.GribCell[(int)tmp.jewel.JewelPosition.x, (int)tmp.jewel.JewelPosition.y].transform.position);
                }
            }
        }
    }
    // 获取有特效的格子列表
    private List<CellObj> getListCellEffect()
    {
        // 创建一个空格子列表
        List<CellObj> tmp = new List<CellObj>();
        // 遍历所有格子
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                if (GribManager.cell.GribCellObj[x, y] != null && GribManager.cell.GribCellObj[x, y].cell.CellEffect > 0)
                {
                    // 将有特效格子添加到列表
                    tmp.Add(GribManager.cell.GribCellObj[x, y]);
                }
            }
        }
        // 返回列表
        return tmp;
    }
    // 获取所有非普通格子的列表
    private List<CellObj> getListNotEmpty()
    {
        // 格子列表
        List<CellObj> tmp = new List<CellObj>();
        // 遍历格子列表
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                // 格子不为空且是特殊格子
                if (GribManager.cell.GribCellObj[x, y] != null && GribManager.cell.GribCellObj[x, y].cell.CellType > 1)
                {
                    // 该位置宝石对象不为空
                    if (JewelSpawner.spawn.JewelGribScript[x, y] != null)
                        // 该位置格子添加到列表
                        tmp.Add(GribManager.cell.GribCellObj[x, y]);
                }
            }
        }
        // 返回列表
        return tmp;
    }
    // 星星下方随机一个宝石坐标
    private Vector2 posUnderStar()
    {
        // 二维坐标列表
        List<Vector2> under = new List<Vector2>();
        // 获取星星宝石现在位置
        int x = (int)JewelStar.jewel.JewelPosition.x;
        int y = (int)JewelStar.jewel.JewelPosition.y;
        // 遍历星星宝石正下方宝石
        for (int i = 0; i < y; i++)
        {
            // 该位置宝石对象不为空
            if (JewelSpawner.spawn.JewelGribScript[x, i] != null)
                // 添加到坐标列表
                under.Add(JewelSpawner.spawn.JewelGribScript[x, i].jewel.JewelPosition);
        }
        // 如果星星宝石下方有宝石
        if (under.Count > 0)
            // 返回随机其中一个的坐标
            return under[Random.Range(0, under.Count)];
        // 返回星星宝石坐标
        else return new Vector2(x, y);
    }
    // 从特殊格子中随机一个消除
    private void destroynotempty()
    {
        try
        {
            // 获取所有非普通格子的列表
            List<CellObj> listnotempty = getListNotEmpty();
            // 若列表不为空
            if (listnotempty.Count > 0)
            {
                // 从列表中随机一个格子的坐标
                Vector2 tmp = listnotempty[Random.Range(0, listnotempty.Count)].cell.CellPosition;
                // 根据做表在格子数组找到其对象
                if (JewelSpawner.spawn.JewelGribScript[(int)tmp.x, (int)tmp.y] != null)
                {
                    // 销毁该对象
                    JewelSpawner.spawn.JewelGribScript[(int)tmp.x, (int)tmp.y].Destroy();
                    // 播放火球特效
                    EffectSpawner.effect.Thunder(GribManager.cell.GribCell[(int)tmp.x, (int)tmp.y].transform.position);
                }
            }
        }
        catch
        {
        }
    }
    // 追加游戏时间
    IEnumerator TimeInc()
    {
        int dem = 0;    //？？
        // 一次追加的时长
        int t = 22;
        while (t > 0)
        {
            dem++;
            // 一帧加1s时长
            Timer.timer.GameTime += 1;
            // 拥有的时长不能大于270
            if (Timer.timer.GameTime >= 270f)
            {
                Timer.timer.GameTime = 270f;
                break;
            }
            t -= 1;
            // 等一帧
            yield return null;
            if (dem >= 270) break;

        }
    }

    /// 
    /// <summary>
    /// 随机在一个宝石身上产生一个技能特效
    /// </summary>
    public void AddBonusPower()
    {
        // 产生技能次数
        int dem = 0;
        while (true)
        {
            dem++;
            // 超过63次，就不在给宝石赋予技能
            if (dem >= 63)
                return;
            // 随机一个宝石坐标
            int x = Random.Range(0, 7);
            int y = Random.Range(0, 9);
            // 获取对应宝石对象
            JewelObj tmp = JewelSpawner.spawn.JewelGribScript[x, y];
            // 该对象不为空，不是五彩宝石，没有宝石技能，所在格子是普通格子
            if (tmp != null && tmp.jewel.JewelType != 8 && tmp.jewel.JewelPower == 0 && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 0)
            {
                //随机1种技能
                int r = Random.Range(2, 4);
                // 给宝石技能赋值
                tmp.jewel.JewelPower = r;
                // 生成火箭触发特效
                EffectSpawner.effect.ThunderRow(JewelSpawner.spawn.JewelGrib[x, y], r);
                return;
            }
        }
    }
    // 显示星星宝石
    public void ShowStar()
    {
        // 二维坐标列表
        List<Vector2> listpos = new List<Vector2>();
        // 从上向下遍历格子数组
        for (int y = 9 - 1; y >= 0; y--)
        {
            for (int x = 0; x < 7; x++)
            {
                // 该格子不为空
                if (GribManager.cell.GribCellObj[x, y] != null)
                    // 将格子坐标存入列表
                    listpos.Add(new Vector2(x, y));
            }
            // 若列表不为空跳出循环
            if (listpos.Count > 0)
                break;
        }
        // 从列表中随机一个格子的坐标
        Vector2 pos = listpos[Random.Range(0, listpos.Count)];
        // 生成星星宝石对象
        JewelSpawner.spawn.SpawnStar(pos);
        // 播放音效
        SoundController.Sound.StarIn();
    }
    // 判断是否赢了
    public void WinChecker()
    {
        // 当前最小y坐标
        int Min = 0;
        // 从下往上遍历格子数组
        for (int y = 0; y < 9; y++)
        {
            // 取得与星星宝石同列宝石对象
            if (GribManager.cell.GribCellObj[(int)JewelStar.jewel.JewelPosition.x, y] != null)
            {
                // 将当前y坐标赋予Min并跳出循环
                Min = y;
                break;
            }
        }
        // 如果星星宝石的y坐标 = 最小y坐标
        if ((int)JewelStar.jewel.JewelPosition.y == Min)
        {
            // 执行赢了的特效和动作
            Timer.timer.Win();
            // 销毁星星宝石
            Destroy(JewelStar.gameObject);
        }
    }
    // 显示选中框
    void EnableSelector(Vector3 pos)
    {
        // 设置选中框位置
        Selector.transform.position = pos;
        // 显示选中框
        Selector.SetActive(true);
    }
}
