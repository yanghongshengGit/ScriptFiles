using System.Collections.Generic;
using System;
using UnityEngine;

namespace DataManager
{
    //public class MyPoint: MonoBehaviour
    //{
    //    public MyPoint(int X = 0,int Y = 0)
    //    {
    //        x = X;
    //        y = Y;
    //    }
    //    //vector3 转 mypoint

    //    //mypint 转 vector3

    //    private int x
    //    {
    //        get;
    //        set;
    //    }
    //    private int y
    //    {
    //        get;
    //        set;
    //    }
    //}
    public struct FillPiece
    {
        public int row;
        public int col;
        public int type;
    }
    //单例模式 数据处理
    public class DataDeal : MonoBehaviour
    {
        private DataDeal()
        {
            coreArray = new int[DataDefine.RowNum, DataDefine.ColNum];
            deleteArray = new bool[DataDefine.RowNum, DataDefine.ColNum];
            InitDeleteArray();
        }
        private static DataDeal dataDealer;
        public static DataDeal GetInstance()
        {
            if (dataDealer == null)
            {
                dataDealer = new DataDeal();
            }
            return dataDealer;
        }

        private int[,] coreArray;                               //存图块类型的数组
        private bool[,] deleteArray;                            //消除的图块

        public bool[,] outDeleteArray()
        {
            return deleteArray;
        }
        //public int[,] outCoreArray()
        //{
        //    return coreArray;
        //}

        private void InitDeleteArray()
        {
            for(int i = 0; i < DataDefine.RowNum; i++)
            {
                for(int j = 0; j < DataDefine.ColNum; j++)
                {
                    deleteArray[i, j] = false;
                }
            }
        }

        public void InputArray(int[,] array)                    //输入数组
        {
            for (int i = 0; i < DataDefine.RowNum; i++)
                for (int j = 0; j < DataDefine.ColNum; j++)
                    coreArray[i, j] = array[i, j];
			InitDeleteArray ();
        }
        public void ScanArray()                                 //扫描数组
        {
            for (int i = 0; i < DataDefine.RowNum; i++)
                for (int j = 0; j < DataDefine.ColNum; j++)
                {
                    CheckPiece(i, j);
                }
        }
        private void CheckPiece(int x, int y)                   //检查该图块的周围类型
        {
            //右边
            int count = 1;
            int temX = x;
            int temY = y;
            while(temX + 1 < DataDefine.ColNum)
            {
                if(coreArray[y, temX] == coreArray[y, temX + 1])
                {
                    count++;
                    temX++;
                }
                else
                {
                    break;
                }
            }
            if(count >= 3)
            {
                for (int i = 0; i < count; i++)
                    deleteArray[y, x + i] = true;
            }
            //下边
            count = 1;
            while (temY + 1 < DataDefine.RowNum)
            {
                if (coreArray[temY, x] == coreArray[temY+1, x])
                {
                    count++;
                    temY++;
                }
                else
                {
                    break;
                }
            }
            if (count >= 3)
            {
                for (int i = 0; i < count; i++)
                    deleteArray[y+i, x] = true;
            }
        }
        public void GetSkillList(ref List<SkillData> skillList) //获取技能释放列表
        {
            int jinNum  = 0;
            int muNum   = 0;
            int shuiNum = 0;
            int huoNum  = 0;
            int tuNum   = 0;
            for (int i = 0; i < DataDefine.RowNum; i++)
                for (int j = 0; j < DataDefine.ColNum; j++)
                {
                    if(deleteArray[i, j] == true)
                    {
                        //获得消除图块的数量
                        switch ((BaseElement)coreArray[i,j])
                        {
                            case BaseElement.Jin: jinNum++; break;
                            case BaseElement.Mu:  muNum++; break;
                            case BaseElement.Shui:shuiNum++; break;
                            case BaseElement.Huo: huoNum++; break;
                            case BaseElement.Tu:  tuNum++; break;
                            default: break;
                        }
                        //将要消除的位置赋为空
                        coreArray[i, j] = (int)BaseElement.None;
                    }
                }
            if (jinNum == 3)
                skillList.Add(SkillData.Jin_3);
            else if (jinNum == 4)
                skillList.Add(SkillData.Jin_4);
            else if (jinNum >= 5)
                skillList.Add(SkillData.Jin_5);
            if (muNum == 3)
                skillList.Add(SkillData.Mu_3);
            else if (muNum == 4)
                skillList.Add(SkillData.Mu_4);
            else if (muNum >= 5)
                skillList.Add(SkillData.Mu_5);
            if (shuiNum == 3)
                skillList.Add(SkillData.Shui_3);
            else if (shuiNum == 4)
                skillList.Add(SkillData.Shui_4);
            else if (shuiNum >= 5)
                skillList.Add(SkillData.Shui_5);
            if (huoNum == 3)
                skillList.Add(SkillData.Huo_3);
            else if (huoNum == 4)
                skillList.Add(SkillData.Huo_4);
            else if (huoNum >= 5)
                skillList.Add(SkillData.Huo_5);
            if (tuNum == 3)
                skillList.Add(SkillData.Tu_3);
            else if (tuNum == 4)
                skillList.Add(SkillData.Tu_4);
            else if (tuNum >= 5)
                skillList.Add(SkillData.Tu_5);
        }
        public int[,] Fall()                                    //下落
        {
            int[,] fallDistanceArray = new int[DataDefine.RowNum, DataDefine.ColNum];
            for (int i = 0; i < DataDefine.RowNum; i++)
            {
                for (int j = 0; j < DataDefine.ColNum; j++)
                {
                    if(coreArray[i, j] == (int)BaseElement.None)
                    {
                        fallDistanceArray[i, j] = 0;
                    }
                    else
                    {
                        fallDistanceArray[i, j] = CheckColNone(i, j);
                    }
                }
            }

            fallUpdateArray(fallDistanceArray);

            return fallDistanceArray;
        }
        private int CheckColNone(int row, int col)              //检查该元素下面空几格
        {
            int count = 0;
            int temRow = row;
            while(temRow < DataDefine.RowNum)
            {
                if (coreArray[temRow, col] == (int)BaseElement.None)
                    count++;
                temRow++;
            }

            return count;
        }
        private void fallUpdateArray(int[,] fallDistanceArray)  //下落更新数组
        {
            for (int i = DataDefine.RowNum - 1; i >= 0; i--)
            {
                for (int j = 0; j < DataDefine.ColNum; j++)
                {
                    int fallDistance = fallDistanceArray[i, j];
                    //交换上下
                    int tempType = coreArray[i, j];
                    coreArray[i, j] = coreArray[i + fallDistance, j];
                    coreArray[i + fallDistance, j] = tempType;
                }
            }
        }
        public void Fill(ref List<FillPiece> fillList)                               //填补空白
        {
            System.Random rand = new System.Random();
            for (int i = 0; i < DataDefine.RowNum; i++)
            {
                for (int j = 0; j < DataDefine.ColNum; j++)
                {
                    if(coreArray[i, j] == (int)BaseElement.None)
                    {
                        coreArray[i, j] = rand.Next(1, 6);
                        FillPiece fillpiece = new FillPiece();
                        fillpiece.row = i;
                        fillpiece.col = j;
                        fillpiece.type = coreArray[i, j];
                        fillList.Add(fillpiece);
                    }
                }
            }
        }
    }
}
