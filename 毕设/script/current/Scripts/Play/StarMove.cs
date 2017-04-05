using UnityEngine;
using System.Collections;

/// <summary>
/// 星星移动 组件 （挂在star游戏对象下）
/// </summary>
public class StarMove : MonoBehaviour
{
	//数据
    float X = -1;			//坐标x
	float Y = -1;			//坐标y
	bool actived = false;	//是否启动

	//协程
    IEnumerator Start()
    {
        X = 0;
        Y = 0;
        yield return new WaitForSeconds(1f);	//等1s后继续
        actived = true;
		yield return new WaitForSeconds(0.75f);	//等0.75s后继续
        GameObject.Find("JewelStar").transform.GetChild(0).gameObject.SetActive(true);

//		Debug.Log (GameObject.Find ("JewelStar"));
//		Debug.Log (gameObject);

        Destroy(gameObject);
    }

    void Update()
    {

        if (actived)
        {
            X = JewelSpawner.spawn.JewelGrib[(int)GameController.action.JewelStar.jewel.JewelPosition.x, (int)GameController.action.JewelStar.jewel.JewelPosition.y].transform.position.x;
            Y = JewelSpawner.spawn.JewelGrib[(int)GameController.action.JewelStar.jewel.JewelPosition.x, (int)GameController.action.JewelStar.jewel.JewelPosition.y].transform.position.y;
        }

        if (X != -1 && X != transform.localPosition.x)
            MoveToX(X);
        if (Y != -1 && Y != transform.localPosition.y)
            MoveToY(Y);

    }
    void MoveToX(float x)
    {
        if (Mathf.Abs(x - transform.localPosition.x) > 0.15)
        {
            if (transform.localPosition.x > x)
                transform.localPosition -= new Vector3(Time.smoothDeltaTime * 6f, 0, 0);
            else if (transform.localPosition.x < x)
                transform.localPosition += new Vector3(Time.smoothDeltaTime * 6f, 0, 0);
        }
        else
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            X = -1;
        }
    }
    void MoveToY(float y)
    {
        if (Mathf.Abs(y - transform.localPosition.y) > 0.15)
        {
            if (transform.localPosition.y > y)
                transform.localPosition -= new Vector3(0, Time.smoothDeltaTime * 8f, 0);
            else if (transform.localPosition.y < y)
                transform.localPosition += new Vector3(0, Time.smoothDeltaTime * 8f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            Y = -1;

        }
    }
}
