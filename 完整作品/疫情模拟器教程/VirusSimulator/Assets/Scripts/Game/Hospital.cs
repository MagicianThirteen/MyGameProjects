using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : Space
{
    private int nowBedNum = 0;
    private void Awake()
    {
       
        centerX = 333;
        centerY = -147;
        w = 300;
        h = 300; 
    }
    public void InitInfo(int num)
    {
        //初始化医院床位数
        nowBedNum = num;
    }
    /// <summary>
    /// 加人要床位数够才能添加成功
    /// </summary>
    /// <returns></returns>
    public bool AddPerson()
    {
        if (nowBedNum > 0)
        {
            --nowBedNum;
            return true;
        }
        return false;
    }
}
