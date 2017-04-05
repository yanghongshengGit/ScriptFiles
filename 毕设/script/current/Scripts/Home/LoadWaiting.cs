using UnityEngine;
using System.Collections;

// 加载场景脚本
public class LoadWaiting : MonoBehaviour
{
    // 进度条
    public UnityEngine.UI.Image loadbar;    // Image loading fake

    /// <summary>
    /// fill image by second and go to Home scene
    /// </summary>
    /// <returns></returns>
    IEnumerator Start() // 协程
    {
        for (int i = 0; i < 120; i++)
        {
            loadbar.fillAmount += 1 / 120f;         // 设置进度
            yield return new WaitForEndOfFrame();   // 等一帧
        }
        Application.LoadLevel("HomeScene");         // 载入主场景
    }
}
