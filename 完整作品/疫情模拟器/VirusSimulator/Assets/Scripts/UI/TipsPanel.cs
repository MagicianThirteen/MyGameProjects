using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 按下开始模拟键后，显示倒计时多少秒开始模拟。
/// </summary>
public class TipsPanel : MonoBehaviour
{
    public Text txtInfo;
    public static TipsPanel instance;
    
    void Awake()
    {
        instance = this;
        HideMe();
    }

    public void ShowMe(string info)
    {
        this.gameObject.SetActive(true);
        txtInfo.text = info;
    }
    public void HideMe()
    {
        this.gameObject.SetActive(false);
    }
}
