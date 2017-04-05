﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ulti : MonoBehaviour
{

    /// <summary>
    /// 将两个List合并
    /// </summary>
    /// <param name="l1"></param>
    /// <param name="l2"></param>
    /// <param name="bonus">触发消除的方块</param>
    /// <returns></returns>
    public static List<JewelObj> ListPlus(List<JewelObj> l1, List<JewelObj> l2, JewelObj bonus)
    {
		// 创建一个新的宝石对象表
        List<JewelObj> tmp = new List<JewelObj>();
		// 添加左边对象
        for (int i = l1.Count - 1; i >= 0; i--)
        {
            tmp.Add(l1[i]);
        }
		// 添加自己
        if (bonus != null)
            tmp.Add(bonus);
		// 添加右边对象
        for (int i = 0; i < l2.Count; i++)
        {
            tmp.Add(l2[i]);
        }
		// 返回合并的list
        return tmp;
    }

    /// <summary>
    /// 移动显示对象
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="NewPos"></param>
    /// <param name="duration"></param>
    public static void MoveTo(GameObject obj, Vector2 NewPos, float duration)
    {
        obj.SetActive(true);
        Animation anim = obj.GetComponent<Animation>();
        //if (anim.GetClipCount() > 1)
        //{
        //    Destroy(anim.GetClip("Moveto"));
        //}
        anim.enabled = true;
        AnimationClip animclip = new AnimationClip();
#if UNITY_5
            animclip.legacy = true;
#endif
        AnimationCurve curvex = AnimationCurve.Linear(0, obj.transform.localPosition.x, duration, NewPos.x);
        AnimationCurve curvey = AnimationCurve.Linear(0, obj.transform.localPosition.y, duration, NewPos.y);
        AnimationCurve curvenable = AnimationCurve.Linear(0, 1, duration, 0);

        animclip.SetCurve("", typeof(Transform), "localPosition.x", curvex);
        animclip.SetCurve("", typeof(Transform), "localPosition.y", curvey);
        animclip.SetCurve("", typeof(Animation), "m_Enabled", curvenable);

        anim.AddClip(animclip, "Moveto");
        anim.Play("Moveto");
        Destroy(animclip, duration);
    }

    /// <summary>
    /// 补间动画 移动游戏物体到某位置.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="NewPos"></param>
    /// <param name="duration"></param>
    /// <param name="z"></param>
    public static void MoveTo(GameObject obj, Vector2 NewPos, float duration, float z)
    {
        Animation anim = obj.GetComponent<Animation>();
        //if (anim.GetClipCount() > 1)
        //{
        //    anim.RemoveClip("Moveto");
        //}
        anim.enabled = true;
        AnimationClip animclip = new AnimationClip();
#if UNITY_5
            animclip.legacy = true;
#endif
        AnimationCurve curvex = AnimationCurve.Linear(0, obj.transform.localPosition.x, duration, NewPos.x);
        AnimationCurve curvey = AnimationCurve.Linear(0, obj.transform.localPosition.y, duration, NewPos.y);
        AnimationCurve curvez = AnimationCurve.Linear(0, z, duration, z);
        AnimationCurve curvenable = AnimationCurve.Linear(0, 1, duration, 0);

        animclip.SetCurve("", typeof(Transform), "localPosition.x", curvex);
        animclip.SetCurve("", typeof(Transform), "localPosition.y", curvey);
        animclip.SetCurve("", typeof(Transform), "localPosition.z", curvez);
        animclip.SetCurve("", typeof(Animation), "m_Enabled", curvenable);

        anim.AddClip(animclip, "Moveto");
        anim.Play("Moveto");
        Destroy(animclip, duration);
    }
    public static void MoveTo(GameObject obj, Vector2 startpos, Vector2 NewPos, float duration, float z)
    {
        Animation anim = obj.GetComponent<Animation>();
        //if (anim.GetClipCount() > 1)
        //{
        //    anim.RemoveClip("Moveto");
        //}
        anim.enabled = true;
        AnimationClip animclip = new AnimationClip();
#if UNITY_5
                animclip.legacy = true;
#endif
        AnimationCurve curvex = AnimationCurve.Linear(0, startpos.x, duration, NewPos.x);
        AnimationCurve curvey = AnimationCurve.Linear(0, startpos.y, duration, NewPos.y);
        AnimationCurve curvez = AnimationCurve.Linear(0, z, duration, z);
        AnimationCurve curvenable = AnimationCurve.Linear(0, 1, duration, 0);

        animclip.SetCurve("", typeof(Transform), "localPosition.x", curvex);
        animclip.SetCurve("", typeof(Transform), "localPosition.y", curvey);
        animclip.SetCurve("", typeof(Transform), "localPosition.z", curvez);
        animclip.SetCurve("", typeof(Animation), "m_Enabled", curvenable);

        anim.AddClip(animclip, "Moveto");
        anim.Play("Moveto");
        Destroy(animclip, duration);
    }

    /// <summary>
    /// 使用协程播放下落动画
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="NewPos"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static IEnumerator IEDrop(GameObject obj, Vector2 NewPos, float speed)
    {
		// 获取对象上挂载脚本对象jewelobj
        JewelObj script = obj.GetComponent<JewelObj>();
		// 获取对象上二维碰撞组件
        Collider2D coll = obj.GetComponent<Collider2D>();

        if (obj != null)	// 存在判断
        {
			// 获取对象上Transform组件
            Transform _tranform = obj.transform;
			// 禁用碰撞组件
            coll.enabled = false;
			// 设置脚本对象成员 可以移动
            script.isMove = true;
            //!!!! 注意显示游戏对象即：obj中transform的localPosition与逻辑地图中的NewPos是一样，均使用的是左下角0,0然后往上一格则为0,1
            //这就解决了显示对象数据向逻辑数据同步的问题，即显示对象按逻辑对象的x,y进行移动到指定位置。
            while (_tranform != null && _tranform.localPosition.y - NewPos.y > 0.1f)
            {
				// Time.smoothDeltaTime上一帧所花的时间
                _tranform.localPosition -= new Vector3(0, Time.smoothDeltaTime * speed);
				// 停留一帧
                yield return null;
            }
            if (_tranform != null)
            {
                //？？这块的数据和AS3中的闭包数据很像,即协程执行完比后,他的数据类似AS3中的闭包方法,即数据还是之前的数据.
				// 赋给对象准确的位置
                _tranform.localPosition = new Vector3(NewPos.x, NewPos.y);
				// 播放宝石的抖动动画
                script.Bounce();
				// 检查消除
                script.RuleChecker();
				// 停留0.2s
                yield return new WaitForSeconds(0.2f);

                //开启碰撞器组件
                if (coll != null)
                {
                    coll.enabled = true;
                    script.isMove = false;
                }
            }
        }
    }

}