using UnityEngine;
using System.Collections;

/// <summary>
/// 格子游戏对象 组件
/// </summary>
public class CellObj : MonoBehaviour
{
	// 格子类型
    public int CellCode;
	// 格子数据对象
    public Cell cell;


    /// <summary>
    /// change to sprite by index
    /// </summary>
    public void SetSpriteEvent()			// 在消除后，格子背景下降一个档
    {
        SetSprite(cell.CellType - 1);
    }

    /// <summary>
    /// set sprite for cell when change index
    /// </summary>
    /// <param name="type"></param>
    public void SetSprite(int type)
    {
		// 更换图片
        this.GetComponent<SpriteRenderer>().sprite = GribManager.cell.CellSprite[type];
		// 设置特效
        setChilEffectSprite(cell.CellEffect);

    }

    /// <summary>
    /// remove effect of cell
    /// </summary>
    public void RemoveEffect()
    {
		// 如果有特效
        if (cell.CellEffect > 0)
        {
			// 隐藏特效图
            transform.GetChild(0).gameObject.SetActive(false);
            if (cell.CellEffect == 5)
            {
				// 播放冰崩特效
                EffectSpawner.effect.IceCrash(cell.CellPosition);
                SoundController.Sound.IceCrash();
            }
            else if (cell.CellEffect == 4)
            {
				// 播放解锁特效
                EffectSpawner.effect.LockCrash(cell.CellPosition);
                SoundController.Sound.LockCrash();
            }
            cell.CellEffect = 0;
			// 如果该位置有宝石对象
            if (JewelSpawner.spawn.JewelGribScript[(int)cell.CellPosition.x, (int)cell.CellPosition.y] != null)
				// 检查消除
                JewelSpawner.spawn.JewelGribScript[(int)cell.CellPosition.x, (int)cell.CellPosition.y].RuleChecker();
        }
    }
	// 设置特效
    void setChilEffectSprite(int celleffect)
    {
        if (celleffect > 0)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GribManager.cell.CellSprite[celleffect];
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

    }
	// 格子降档
    public void CelltypeProcess()
    {
        if (cell.CellType > 1)
        {
            cell.CellType--;		// 类型减一
            runAnim();				// 播放图片切换动画
            if (cell.CellType == 1)
            {
                GameController.action.CellNotEmpty--;			// 格子类型不是1的数量减一
                if (GameController.action.CellNotEmpty == 0)	// 如果该数量已经为0
                    GameController.action.isShowStar = true;	// 就显示特殊星形
            }

        }
    }
	// 播放图片切换动画
    void runAnim()
    {
        Animation anim = GetComponent<Animation>();
        anim.enabled = true;
        anim.Play("CellChangeSprite");
    }

}
