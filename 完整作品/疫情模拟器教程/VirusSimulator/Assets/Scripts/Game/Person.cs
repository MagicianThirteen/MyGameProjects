using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Person : MonoBehaviour
{
    private Image img;
    Vector3 moveDir=Vector3.zero;
    //用来定时随机移动时间
    private int nowMoveTime = 0;
    //用来定时当前传染时间
    private int nowCheckInfectionTime = 0;
    //用来记时什么时候住院
    private int checkInHospital = 0;
    //记时潜伏时间
    private int nowHideTime = 0;
    //随机移动时间间隔50帧
    public static int RandomMoveTime =50;
    //检测感染时间间隔
    public static int CheckInfectionTime = 3;
    int speed = 5;
    List<Vector3> dirs = new List<Vector3>() {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
    };
    public enum E_Person_Type
    {
        Healthy,
        Hide,
        Burst,
        Hospital,
    }

    //当前人的状态
    E_Person_Type personType = E_Person_Type.Healthy;
    //当前人的空间
    Space mySpace;

    //人改变状态
    public void ChangeType(E_Person_Type type)
    {

        //协程可能会造成重复改变 
        if (personType == type)
            return;
        switch (type)
        {
            case E_Person_Type.Healthy:
                
                img.color = Color.white;
                 break;
            case E_Person_Type.Hide:
                //从健康变潜伏
                if (personType == E_Person_Type.Healthy)
                    InfoPanel.instance.ChangeJKNum();
                InfoPanel.instance.ChangeQFNum();
                img.color = Color.yellow;
                break;
            case E_Person_Type.Burst:
                //从健康变爆发
                if (personType == E_Person_Type.Healthy)
                    InfoPanel.instance.ChangeJKNum();
                //从潜伏变爆发
                else if (personType == E_Person_Type.Hide)
                    InfoPanel.instance.ChangeQFNum(false);
                InfoPanel.instance.ChangeBFNum();
                img.color = Color.red;
                break;
            case E_Person_Type.Hospital:
                InfoPanel.instance.ChangeBFNum(false);
                InfoPanel.instance.ChangeBedNum(); 

                img.color = Color.green;
                break;
               
        }
        personType = type;
    }
    //人改变空间，是城市还是医院
    public void ChangeSpace(Space space)
    {
        mySpace = space;
        //随机一个位置
        this.transform.localPosition =
            new Vector3(space.centerX - space.w / 2 + Random.Range(0, space.w),
                        space.centerY - space.h / 2 + Random.Range(0, space.h),
                        0);
        //随机一个移动方向
        RandomMove(); 
    }

    //随机移动
    public void RandomMove()
    {
        //把上一次取出来的方向放回列表中
        if (moveDir != Vector3.zero)
            dirs.Add(moveDir);
        //进行范围判断，不能超过上下左右的边界，超过就往反方向移动
        if (this.transform.localPosition.x <= mySpace.centerX - mySpace.w / 2)
        {
            moveDir = Vector3.right;
            dirs.Remove(moveDir);
        }else if(this.transform.localPosition.x >= mySpace.centerX + mySpace.w / 2)
        {
            moveDir = Vector3.left;
            dirs.Remove(moveDir);
        }else if (this.transform.localPosition.y <= mySpace.centerY - mySpace.h / 2)
        {
            moveDir = Vector3.up;
            dirs.Remove(moveDir);

        }
        else if(this.transform.localPosition.y >= mySpace.centerY + mySpace.h / 2)
        {
            moveDir = Vector3.down;
            dirs.Remove(moveDir);
        }
        else
        {
            //都没超出边界再随机选择方向
            int index = Random.Range(0, dirs.Count);
            moveDir = dirs[index];
            dirs.RemoveAt(index);
        }
       
    }

    //传染
    //用协程不要每一帧里面都做1000次循环，会有卡顿
    public IEnumerator Infection()
    {
        //传染的条件是：是城市才传染，传染给健康且距离较小的
        City city = mySpace as City;
        if(mySpace is City)
        {
            
            //传播概率会随着流动意向的降低而降低
            float rate = (Virus.TRANSMISSION_RATE / 100f)*(Virus.MOVE_INTENTION/100f);
            for(int i = 0; i < city.persons.Count; i++)
            {
                Person person = city.persons[i];
                //传染的条件是人要健康的，距离小于5个像素的，随机出的概率要比传播率小
                if(person.personType==E_Person_Type.Healthy&&
                    Vector3.Distance(this.transform.localPosition, person.transform.localPosition) < 20&&
                    Random.Range(0,1f)<rate)
                {
                    //先是潜伏后是传染
                    if (Virus.HIDE_TIME > 0)
                    {
                        city.persons[i].ChangeType(E_Person_Type.Hide);
                        //给的是别人的潜伏时间
                        city.persons[i].nowHideTime = Virus.HIDE_TIME;
                    }
                    else 
                    {
                        city.persons[i].ChangeType(E_Person_Type.Burst);
                    }
                    
                }

                //避免第一波没传染完又传染下一波
                if (i % (city.persons.Count / CheckInfectionTime) == 0)
                {
                    yield return 0;
                }
            }
        }

        

    }
    private void Awake()
    {
        img = this.GetComponent<Image>();
    }

    //定时更新人数

    // Update is called once per frame
    void Update()
    {
        //移动方向之前要等提示信息消失才可以开始移动，要先判断是否可以检测再测
        if (!City.isBegin)
            return;
         
        this.transform.Translate(moveDir * Time.deltaTime*speed);
        //定时检测改变方向
        nowMoveTime ++;//这里不能写成nowMoveTime+=Time.deltaTime, 因为这样加着会很慢。
        if (nowMoveTime > RandomMoveTime)
        {
            nowMoveTime = 0;
            RandomMove();
        }
        //定时检测传染，当人的状态是发病或者隐藏才传染人
        if (personType == Person.E_Person_Type.Burst || personType == Person.E_Person_Type.Hide)
        {
            nowCheckInfectionTime++;
            if (nowCheckInfectionTime > CheckInfectionTime)
            {
                nowCheckInfectionTime = 0;
                StartCoroutine(Infection());
            }
        }
        //处于爆发期的病人我们要检测什么时候送它进医院，医院有个收治响应时间
        if (personType == E_Person_Type.Burst)
        {
            checkInHospital++;
            if (checkInHospital >= Virus.HOSPITAL_TIME)
            {
                checkInHospital = 0;
                //住院成功，改变状态，放到医院去 
                //checkInHospital = 0;
                if((mySpace as City).hospital.AddPerson())
                {
                   
                    ChangeType(E_Person_Type.Hospital);
                    ChangeSpace((mySpace as City).hospital);
                    
                    
                }

            }
        }else if (personType == E_Person_Type.Hide)
        {
            //如果不是爆发状态而是潜伏状态
            nowHideTime--;
            if (nowHideTime <= 0)
            {
                ChangeType(E_Person_Type.Burst);
            }
        }
    }
}
