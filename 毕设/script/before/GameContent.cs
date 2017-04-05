using UnityEngine;
using System.Collections;
using DataManager;
using System;

namespace UIManager
{
    public class GameContent : MonoBehaviour
    {
		const double elementWidth = 0.3;
		const double elementHight = 0.3;
		const double interval = 0.2;

		const int startPositionX = -6;
		const int startPositionY = 4;

        // Use this for initialization
        void Start()
        {
            //生成随机数组
            showDataArray = new int[DataDefine.RowNum, DataDefine.ColNum];
            System.Random rand = new System.Random();
            for (int i = 0; i < DataDefine.RowNum; i++)
            {
                for (int j = 0; j < DataDefine.ColNum; j++)
                {
                    showDataArray[i, j] = rand.Next(1, 6);
                }
            }
            //显示图块
            Show();
        }

        // Update is called once per frame
        void Update()
        {

        }
        void Show()
        {
            for (int i = 0; i < DataDefine.RowNum; i++)
            {
                for (int j = 0; j < DataDefine.ColNum; j++)
                {
					GameObject sprite = null;
					CreateElement(showDataArray[i, j], ref sprite);
                    if (sprite == null) continue;
					float spriteX = (float)(j * (elementWidth + interval)+startPositionX);
					float spriteY = (float)(-i * (elementHight + interval)+startPositionY);
					Vector3 spritePosition = sprite.transform.position;
					spritePosition.Set (spriteX, spriteY, 0);
					//spritePosition = ContentUI.transform.InverseTransformPoint (spritePosition);
					sprite.transform.position = spritePosition;
                }
            }
        }
        public GameObject JinSprite;
        public GameObject MuSprite;
        public GameObject ShuiSprite;
        public GameObject HuoSprite;
        public GameObject TuSprite;
		void CreateElement(int type, ref GameObject sprite)
        {
            switch ((BaseElement)type)
            {
                case BaseElement.Jin: sprite = (GameObject)Instantiate(JinSprite); break;
                case BaseElement.Mu: sprite = (GameObject)Instantiate(MuSprite); break;
                case BaseElement.Shui: sprite = (GameObject)Instantiate(ShuiSprite); break;
                case BaseElement.Huo: sprite = (GameObject)Instantiate(HuoSprite); break;
                case BaseElement.Tu: sprite = (GameObject)Instantiate(TuSprite); break;
                default: break;
            }
        }

        private int[,] showDataArray;
    }
}