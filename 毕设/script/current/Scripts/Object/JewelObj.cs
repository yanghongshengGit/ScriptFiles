using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 宝石游戏对象 组件 (挂在 Jewel游戏对象下)
/// 职责:自身提供多种方法，用于宝石生命周期的自管理
/// </summary>
public class JewelObj : MonoBehaviour
{
	// 宝石数据对象
    public Jewel jewel;
	// 宝石图片对象
    public SpriteRenderer render;
	// 延时
    private const float DELAY = 0.2f;
	// 是否检查过
    public bool Checked;
	// 是否移动
    public bool isMove;

    //delete jewel
    public void Destroy()
    {
        //从宝石数组中移除自身
        RemoveFromList((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
        StartCoroutine(_Destroy());
    }

    /// <summary>
    /// 技能释放 power of jewel
    /// </summary>
    /// <param name="power"></param>
    void PowerProcess(int power)
    {
        switch (power)
        {
            case 1:		// 爆炸
                GameController.action.PBoom((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
                EffectSpawner.effect.boom(this.gameObject.transform.position);
                break;
			case 2:		// 行火箭
                EffectSpawner.effect.FireArrow(transform.position, false);
                GameController.action.PDestroyRow((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
                break;
			case 3:		// 列爆炸
                EffectSpawner.effect.FireArrow(transform.position, true);
                GameController.action.PDestroyCollumn((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
                break;
			case 4:		// 奖励时间
                GameController.action.PBonusTime();
                break;
        }
    }

    //move jewel and destroy
    public void ReGroup(Vector2 pos)
    {
        StartCoroutine(_ReGroup(pos));
    }

    IEnumerator _ReGroup(Vector2 pos)
    {
        RemoveFromList((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
        yield return new WaitForSeconds(DELAY - 0.015f);
        Ulti.MoveTo(this.gameObject, pos, DELAY);

        StartCoroutine(_Destroy());
    }

    /// <summary>
    /// 销毁自身
    /// </summary>
    /// <returns></returns>
    IEnumerator _Destroy()
    {
		// 消除
        GribManager.cell.GribCellObj[(int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y].CelltypeProcess();
		// 播放消除特效
        GameController.action.CellRemoveEffect((int)jewel.JewelPosition.x, (int)jewel.JewelPosition.y);
		// 延时
        yield return new WaitForSeconds(DELAY);
		// 如果宝石有技能
        if (jewel.JewelPower > 0)
        {
			// 播放特效
            PowerProcess(jewel.JewelPower);
        }
		// 更新下落延时
        GameController.action.drop.DELAY = GameController.DROP_DELAY;
		// 根据当前宝石显示对象，播放销毁动画
        JewelCrash();

		// 等一帧
        yield return new WaitForEndOfFrame();
		// 增加分数
        EffectSpawner.effect.ScoreInc(this.gameObject.transform.position);

		// 等一帧
        yield return new WaitForEndOfFrame();
		// 增加特效计数
        EffectSpawner.effect.ContinueCombo();

		// 等一帧
        yield return new WaitForEndOfFrame();
		// 更新提示时间
        Supporter.sp.RefreshTime();
		// 停止协程
        StopAllCoroutines();
		// 销毁宝石对象
        Destroy(gameObject);
    }

    /// <summary>
    /// 根据当前宝石显示对象，播放销毁动画
    /// </summary>
    void JewelCrash()
    {
        int x = (int)jewel.JewelPosition.x;
        int y = (int)jewel.JewelPosition.y;

        EffectSpawner.effect.JewelCrashArray[x, y].transform.position = new Vector3(transform.position.x, transform.position.y, -0.2f);
        EffectSpawner.effect.JewelCrashArray[x, y].SetActive(false);
        EffectSpawner.effect.JewelCrashArray[x, y].SetActive(true);
    }

    /// <summary>
    /// 重新调整JewelPosition的位置，并播放下落动画
    /// 此方法有点类似于排序，主要是处理当Map中有消除的方块后产生空位，此时需要将空位上方的方块移动到空位。
    /// （说白了就是冒泡排序，地图中消失的方块移动到后面）
    /// 将数组中Y轴的往下移动。全部移动完后，所有地图中空的位置均为后面的值，例如6、7、8这几个位置是空的）
    /// </summary>
    public void getNewPosition()
    {
        int newpos = (int)jewel.JewelPosition.y;    // 新的y坐标
        int x = (int)jewel.JewelPosition.x;         // x坐标
        int oldpos = (int)jewel.JewelPosition.y;    // 初始的y坐标

        for (int y = newpos - 1; y >= 0; y--)       // 遍历从现在往下的jewelObj
        {
            // 如果该位置不是固定为空并且该位置没有冰崩特效并且该位置没有宝石对象
            if (GribManager.cell.Map[x, y] != 0 && GribManager.cell.GribCellObj[x, y].cell.CellEffect != 4 && JewelSpawner.spawn.JewelGribScript[x, y] == null)
                newpos = y;
            // 如果该位置不是固定为空并且该位置有冰崩特效
            else if (GribManager.cell.Map[x, y] != 0 && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 4)
            {
                break;
            }
        }
        // 清空原坐标对象
        JewelSpawner.spawn.JewelGribScript[x, (int)jewel.JewelPosition.y] = null;
        JewelSpawner.spawn.JewelGrib[x, (int)jewel.JewelPosition.y] = null;
        // 生成新坐标
        jewel.JewelPosition = new Vector2(x, newpos);
        JewelSpawner.spawn.JewelGribScript[x, newpos] = this;
        JewelSpawner.spawn.JewelGrib[x, newpos] = this.gameObject;

        if (oldpos != newpos)
            // 开始下落协程
            StartCoroutine(Ulti.IEDrop(this.gameObject, jewel.JewelPosition, GameController.DROP_SPEED));
    }

    /// <summary>
    /// 获取行消除List
    /// </summary>
    /// <param name="Pos"></param>
    /// <param name="type"></param>
    /// <param name="bonus"></param>
    /// <returns></returns>
    public List<JewelObj> GetRow(Vector2 Pos, int type, JewelObj bonus)
    {
        // 获取左边类型相同的对象列表
        List<JewelObj> tmp1 = GetLeft(Pos, type);
        // 获取右边类型相同的对象列表
        List<JewelObj> tmp2 = GetRight(Pos, type);
        // 如果相同的类型数>=3
        if (tmp1.Count + tmp2.Count > 1)
        {
            // 返回合并两个list的list
            return Ulti.ListPlus(tmp1, tmp2, bonus);
        }

        else
            // 返回空表
            return new List<JewelObj>();
    }

    /// <summary>
    /// 获取列消除List
    /// </summary>
    /// <param name="Pos"></param>
    /// <param name="type"></param>
    /// <param name="bonus"></param>
    /// <returns></returns>
    public List<JewelObj> GetCollumn(Vector2 Pos, int type, JewelObj bonus)
    {
		// 获取上边类型相同的对象列表
        List<JewelObj> tmp1 = GetTop(Pos, type);
		// 获取下边类型相同的对象列表
        List<JewelObj> tmp2 = GetBot(Pos, type);
		// 如果相同的类型数>=3
        if (tmp1.Count + tmp2.Count > 1)
        {
			// 返回合并两个list的list
            return Ulti.ListPlus(tmp1, tmp2, bonus);
        }
        else
            return new List<JewelObj>();
    }

    /// <summary>
    /// 播放移动无效的动画
    /// </summary>
    /// <param name="Obj"></param>
    public void SetBackAnimation(GameObject Obj)
    {
        if (!Supporter.sp.isNomove)	// 可以移动
        {
            Vector2 ObjPos = Obj.GetComponent<JewelObj>().jewel.JewelPosition;
            Animation anim = transform.GetChild(0).GetComponent<Animation>();
            anim.enabled = true;

            if (ObjPos.x == jewel.JewelPosition.x)
            {
                if (ObjPos.y > jewel.JewelPosition.y)
                {
                    anim.Play("MoveBack_Up");
                }
                else
                {
                    anim.Play("MoveBack_Down");
                }
            }
            else
            {
                if (ObjPos.x > jewel.JewelPosition.x)
                {
                    anim.Play("MoveBack_Right");
                }
                else
                {
                    anim.Play("MoveBack_Left");
                }
            }
        }
    }

	/// <summary>
	///  获取本对象左边相同类型的对象列表
	/// </summary>
	/// <returns>The left.</returns>
	/// <param name="Pos">Position.</param>
	/// <param name="type">Type.</param>
    List<JewelObj> GetLeft(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
        for (int x = (int)Pos.x - 1; x >= 0; x--)
        {

            if (x != jewel.JewelPosition.x && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y] != null && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y].jewel.JewelType == type && GribManager.cell.GribCellObj[x, (int)Pos.y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y]);
            else
                return tmp;
        }
        return tmp;
    }
	/// <summary>
	/// 获取本对象右边相同类型的对象列表
	/// </summary>
	/// <returns>The right.</returns>
	/// <param name="Pos">Position.</param>
	/// <param name="type">Type.</param>
    List<JewelObj> GetRight(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
        for (int x = (int)Pos.x + 1; x < 7; x++)
        {
            if (x != jewel.JewelPosition.x && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y] != null && JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y].jewel.JewelType == type && GribManager.cell.GribCellObj[x, (int)Pos.y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[x, (int)Pos.y]);
            else
                return tmp;
        }
        return tmp;
    }

    /// <summary>
    /// 检测上方是否有相同的方块
    /// </summary>
    /// <param name="Pos"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    List<JewelObj> GetTop(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
        for (int y = (int)Pos.y + 1; y < 9; y++)
        {
            if (y != jewel.JewelPosition.y && 
                JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y] != null && 
                JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y].jewel.JewelType == type && 
                GribManager.cell.GribCellObj[(int)Pos.x, y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y]);
            else
                return tmp;
        }

        return tmp;
    }
    
    /// <summary>
    /// 检测下方是否有相同的方块
    /// </summary>
    /// <param name="Pos"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    List<JewelObj> GetBot(Vector2 Pos, int type)
    {
        List<JewelObj> tmp = new List<JewelObj>();
        for (int y = (int)Pos.y - 1; y >= 0; y--)
        {
            if (y != jewel.JewelPosition.y && JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y] != null && JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y].jewel.JewelType == type && GribManager.cell.GribCellObj[(int)Pos.x, y].cell.CellEffect == 0)
                tmp.Add(JewelSpawner.spawn.JewelGribScript[(int)Pos.x, y]);
            else
                return tmp;
        }

        return tmp;
    }

    /// <summary>
    /// 从宝石数组中移除自身
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void RemoveFromList(int x, int y)
    {
        //移除脚本
        JewelSpawner.spawn.JewelGribScript[x, y] = null;
        //移除显示对象
        JewelSpawner.spawn.JewelGrib[x, y] = null;

        //碰撞器
        GetComponent<Collider2D>().enabled = false;
    }

    // 获取本对象周围可以消除的列表的大小
    public int getListcount()
    {
        // 合并行，列消除列表
        List<JewelObj> list = Ulti.ListPlus(GetRow(jewel.JewelPosition, jewel.JewelType, null),
                                            GetCollumn(jewel.JewelPosition, jewel.JewelType, null),
                                            this);
        // 返回列表大小
        return list.Count;
    }
    // 获取本对象周围可以消除的列表
    public List<JewelObj> getList()
    {
        // 合并行，列消除列表
        List<JewelObj> list = Ulti.ListPlus(GetRow(jewel.JewelPosition, jewel.JewelType, null),
                                            GetCollumn(jewel.JewelPosition, jewel.JewelType, null),
                                            this);
        // 返回列表
        return list;
    }
	// 检查消除
    public void RuleChecker()
    {

        if (jewel.JewelType != 99)
        {
			// 获取该位置消除list
            List<JewelObj> list = Ulti.ListPlus(GetRow(jewel.JewelPosition, jewel.JewelType, null),
                                                      GetCollumn(jewel.JewelPosition, jewel.JewelType, null),
                                                      this);
			// 如果消除数>=3
            if (list.Count >= 3)
            {
				// 消除过程
                listProcess(list);
                Checked = true;
            }
        }
        else
        {
			// 检查是否赢了
            GameController.action.WinChecker();
        }


    }
	/// <summary>
	/// 消除列表中对象
	/// </summary>
	/// <param name="list">List.</param>
    void listProcess(List<JewelObj> list)
    {
		// 一个宝石对象周围可以消除的数量
        List<int> _listint = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].Checked)	// 如果该对象还没被检查过
				// 获取该对象周围可以消除的数量，存入_listint
                _listint.Add(list[i].getListcount());
            else
				// 存入本对象周围可以消除的数量
                _listint.Add(list.Count);
        }
		// 找出最大的消除数
        int max = Mathf.Max(_listint.ToArray());
		// 获取最大消除数的list索引
        int idx = _listint.IndexOf(max);
		// 消除宝石 参数（宝石列表，本对象）
        GameController.action.JewelProcess(list[idx].getList(), this.gameObject);
    }

    /// <summary>
    /// 播放宝石的抖动动画,即宝石移动完毕后停止时会抖动一下.
    /// </summary>
    public void Bounce()
    {
        if (GameController.action.GameState == (int)Timer.GameState.PLAYING && !Supporter.sp.isNomove)
        {
            Animation anim = render.GetComponent<Animation>();
            anim.enabled = true;
            anim.Play("bounce");
        }
    }

    public void JewelDisable()
    {
        Animation anim = render.GetComponent<Animation>();
        anim.enabled = true;
        anim.Play("Disable");
    }

    public void JewelEnable()
    {
        Animation anim = render.GetComponent<Animation>();
        anim.enabled = true;
        anim.Play("Enable");
    }

    /// <summary>
    /// 播放左右摇晃动画,用于提示玩家此方块可以消除
    /// </summary>
    public void JewelSuggesttion()
    {
        Animation anim = render.GetComponent<Animation>();
        anim.enabled = true;
        anim.Play("Suggesttion");
    }

    /// <summary>
    /// 停止左右摇晃动画
    /// </summary>
    public void JewelStopSuggesttion()
    {
        Animation anim = render.GetComponent<Animation>();
        if (anim.IsPlaying("Suggesttion"))
        {
            anim.Stop("Suggesttion");
            anim.enabled = false;
            transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}
