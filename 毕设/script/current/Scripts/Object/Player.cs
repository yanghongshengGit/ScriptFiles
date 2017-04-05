using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 玩家针对每一关卡的数据 实体类
/// </summary>
[System.Serializable]	// 序列化 将对象实例的状态存储到存储媒体的过程
public class Player		// 数据类，存玩家现在数据和状态
{
    public int Level;		// 等级
    public string Name;		// 名字
    public bool Locked;		// 该关卡是否上锁
	public int Stars;		// 现有的星数？
    public int HightScore;	// 现最高分
    public int Background;	// 背景模式？

    /// <summary>
    /// 生成字符串拼接,类似于JSON
    /// </summary>
    /// <returns></returns>
    public string ToSaveString()
    {
        string s = Locked + "," + Stars + "," + HightScore + "," + Background + ",";
        return s;
    }
}

/// <summary>
/// 玩家保存记录实体类
/// </summary>
public class PlayerUtils
{
    private string KEY_DATA = "DATA";		// 数据条目key
    private string data = "";				// 数据value
    private string[] dataSplit;				// 一条数据拆分出的数组
    private Player p;						// 存一条数据的对象

    /// <summary>
    /// 保存玩家数据,保存的过程有点类似于JSON
    /// </summary>
    /// <param name="Maps"></param>
    public void Save(List<Player> Maps)
    {
        //PlayerPrefs.DeleteKey(KEY_DATA);
        foreach (var item in Maps)
        {
            data += item.ToSaveString();
        }
		// unity3d提供了一个用于本地持久化保存与读取的类——PlayerPrefs
		// SetString(key, value)
        PlayerPrefs.SetString(KEY_DATA, data);
    }	// 将缓存的玩家信息存入文件

    /// <summary>
    /// 加载玩家数据
    /// Load data load by PlayerPrefs, set to buttons level on map scene 
    /// </summary>
    /// <returns></returns>
    public List<Player> Load()
    {
        List<Player> list = new List<Player>();

        string data = PlayerPrefs.GetString(KEY_DATA, "");

        dataSplit = data.Split(',');

        for (int i = 0; i < 297; i++)
        {
            p = new Player();
            p.Level = i + 1;
            p.Name = (i + 1).ToString();
            p.Locked = bool.Parse(dataSplit[i * 4]);
            p.Stars = int.Parse(dataSplit[i * 4 + 1]);
            p.HightScore = int.Parse(dataSplit[i * 4 + 2]);
            p.Background = int.Parse(dataSplit[i * 4 + 3]);
            list.Add(p);
        }

        return list;
    }
}

