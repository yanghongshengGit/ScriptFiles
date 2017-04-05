using UnityEngine;
using System.Collections;

namespace DataManager
{
    //定义图块
    public enum BaseElement
    {
        None = 0,       //无
        Jin = 1,        //金
        Mu,             //木
        Shui,           //水
        Huo,            //火
        Tu              //土
    }
    //定义技能数据
    public enum SkillData
    {
        Jin_3,      //锐金利剑
        Jin_4,      //金钟罩
        Jin_5,      //黄金领域
        Mu_3,       //刺血荆棘
        Mu_4,       //回春符
        Mu_5,       //禁咒：万物复苏
        Shui_3,     //水愈术
        Shui_4,     //冰冻结界
        Shui_5,     //禁咒：水幕天华
        Huo_3,      //白炽火球
        Huo_4,      //大火球
        Huo_5,      //禁咒：赤炎红莲
        Tu_3,       //土刺
        Tu_4,       //岩钢咒
        Tu_5        //禁咒：大地守护
    }
    //定义包头
    public enum NetPackage
    {
        NP_Login = 1,   //登录
        NP_Register,    //注册
        NP_Match,       //匹配玩家
        NP_Exit,        //退出
        NP_PersonInfo,  //获取个人信息
        NP_Friend,      //获取好友列表
        NP_AddFriend,   //填加好友
        NP_AnswerAdd,   //填加好友消息回复
        NP_Competition, //挑战消息
        NP_ChatRecord,  //获取聊天记录
        NP_SendMessage, //发送聊天消息
        NP_Operator,    //操作消息
        NP_UpdatePerson,//更新人物信息
        NP_UpLoadArray  //上传初始数组
    }
    public class DataDefine : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //定义数据常量
        public const int MaxGrade = 50;     //最大等级
        public const int BasicHP = 100;     //基础血量
        public const int AddHP = 5;         //升级增加血量
        public const int RowNum = 9;        //图块行数
        public const int ColNum = 9;        //图块列数
    }
}
