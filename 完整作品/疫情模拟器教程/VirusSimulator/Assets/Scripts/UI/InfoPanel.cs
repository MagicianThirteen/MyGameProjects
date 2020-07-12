using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 当前数据面板：主要显示更新数据，所以要给外界提供一个更新的方法
/// </summary>
public class InfoPanel : MonoBehaviour
{
    public Text txtJKNum;
    public Text txtQFNum;
    public Text txtBFNum;
    public Text txtCWNum;

    private int nowJKNum;
    private int nowQFNum;
    private int nowBFNum;
    private int nowBedNum;

    public static InfoPanel instance;
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 初始化面板
    /// </summary>
    public void InitPanel(int num)
    {
        SetJkNum(num - Virus.INFECTION_NUM);//健康的=城市数-初始感染人数
        SetBFNum(Virus.INFECTION_NUM);
        SetQFNum(0);
        SetCWNum(Virus.HOSPITAL_BED_NUM);
    }

    //健康人数
    public void SetJkNum(int num)
    {
        txtJKNum.text = num + "人";
        nowJKNum = num;
    }
    //潜伏人数
    public void SetQFNum(int num)
    {
        txtQFNum.text = num + "人";
        nowQFNum = num;
    }
    //爆发人数
    public void SetBFNum(int num)
    {
        txtBFNum.text = num + "人";
        nowBFNum = num;
    }
    //床位人数
    public void SetCWNum(int num)
    {
        txtCWNum.text = num +"/"+Virus.HOSPITAL_BED_NUM;
        nowBedNum = num;
    }

    //提供增加减少的方法
    //实际这些数据跟人的状态改变有关
    public void ChangeJKNum(bool isAdd = false)
    {
        nowJKNum += isAdd ? +1 : -1;
        SetJkNum(nowJKNum);
    }

    public void ChangeQFNum(bool isAdd = true)
    {
        nowQFNum += isAdd ? +1 : -1;
        SetQFNum(nowQFNum);
    }

    public void ChangeBFNum(bool isAdd = true)
    {
        nowBFNum += isAdd ? +1 : -1;
        SetBFNum(nowBFNum);
    }

    public void ChangeBedNum(bool isAdd = false)
    {
        nowBedNum += isAdd ? +1 : -1;
        SetCWNum(nowBedNum);
    }

}
