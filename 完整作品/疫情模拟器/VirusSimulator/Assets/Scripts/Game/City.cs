using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 城市类用来管人和管医院
/// </summary>
public class City : Space
{
    public static bool isBegin = false;
    public static City instance;
    //城市里的人
    public List<Person> persons = new List<Person>();
    //城市里的医院(就一个，统一管理所有的床位）
    public Hospital hospital;
    public int nowInfectionNum = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        centerX = -100;
        centerY = 0;
        w = 500;
        h = 500;
    }
    void Start()
    {
        GameObject obj = new GameObject("hospital");
        hospital = obj.AddComponent<Hospital>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //创建人的行为
    public void CreatePerson(int num)
    {
        StartCoroutine(BeginCreate(num));
    }

    public IEnumerator BeginCreate(int num)
    {
        isBegin = false;
        TipsPanel.instance.ShowMe("数据生成中…");
               yield return 0;//这里的0都是代表从下一帧执行
        //医院数据初始化
        hospital.InitInfo(Virus.HOSPITAL_BED_NUM);
        nowInfectionNum = 0;
        //把容器里的人销毁，再创建
        for (int i = 0; i < persons.Count; i++)
        {
            Destroy(persons[i].gameObject);//没有gameobject删的是脚本
            if (i % 200 == 0)//每删200个人等一等
                yield return 0;
        }
        //删了游戏物体，列表也要清空
        persons.Clear();


        for (int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("Person"));
            obj.transform.SetParent(this.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            //创建好的人还要存入容器
            Person p = obj.GetComponent<Person>();
            //人一创建出来是健康状态而且在城市
            p.ChangeType(Person.E_Person_Type.Healthy);
            p.ChangeSpace(this);
            //创建出来的前n个人默认得病
            if (nowInfectionNum <= Virus.INFECTION_NUM)
            {
                p.ChangeType(Person.E_Person_Type.Burst);
                nowInfectionNum++;
            }
            //还要添加到容器里
            persons.Add(p);

            if (i % 200 == 0)//每创建200个人等一等
                yield return 0;
        }
       

        TipsPanel.instance.ShowMe("模拟将在3s后开始…");
        yield return new WaitForSeconds(1f);
        TipsPanel.instance.ShowMe("模拟将在2s后开始…");
        yield return new WaitForSeconds(1f);
        TipsPanel.instance.ShowMe("模拟将在1s后开始…");
        yield return new WaitForSeconds(1f);
        TipsPanel.instance.ShowMe("开始");
        yield return new WaitForSeconds(1f);
        //实时显示面板初始化
        InfoPanel.instance.InitPanel(num);
        TipsPanel.instance.HideMe();
        isBegin = true; 

    } 

   
}
