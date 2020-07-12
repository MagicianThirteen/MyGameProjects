using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSensor : MonoBehaviour
{
    public float SoundRange = 15.0f;
    public float SightRange = 25.0f;
    public float SightAngle = 60;
    private Transform zombieTransform;
    private Transform nearbyPlayer;
    private Transform zombieEye;

    public float senseTimer = 0.0f;
    public float SensorInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //脚本一开始就获得了这个物体的组件transform
        zombieTransform = transform;
        zombieEye = transform.Find("eye").transform;//通过标签找到眼睛位置

    }

    private void FixedUpdate()//通过计时器实现僵尸对玩家的持续感知
    {
        if (senseTimer >= SensorInterval)
        {
            SenseNearbyPlayer();
            senseTimer = 0.0f;
        }
        
            senseTimer += Time.deltaTime;
        //这里可以不要用else，fixedUpdate每隔段时间调用一次
        
    }

    void SenseNearbyPlayer()//通过视觉和听觉感知玩家
    {
        nearbyPlayer = null;//这里的好处是，一开始就把nearbyPlayer设置成了空，后面如果没感知到就可以不用else
        //模拟听觉
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float dist = Vector3.Distance(player.transform.position, zombieTransform.position);
        if (dist < SoundRange)//在听觉范围内缓存玩家
        {
            nearbyPlayer = player.transform;
        }

        //模拟视觉（视觉范围内，在判断僵尸眼睛前方到玩家的夹角，最后判断有无遮挡
        if (dist < SightRange)
        {
            //这里的方向只需要是僵尸到玩家的前方位置就好了
            Vector3 direction = player.transform.position - zombieTransform.position;
            float degree = Vector3.Angle(direction, zombieTransform.forward);
            if(degree<SightAngle/2 && degree > -SightAngle / 2)
            {
                //一条射线，起始位置是僵尸眼睛，方向是从僵尸位置到玩家位置
                Ray ray = new Ray();
                ray.origin = zombieEye.position;
                ray.direction = direction;
                RaycastHit hitInfo;
                //判断玩家与僵尸之间是否有遮挡物
                if(Physics.Raycast(ray,out hitInfo, SightRange))
                {
                    if (hitInfo.transform == player.transform)
                    {
                        nearbyPlayer = player.transform;
                    }
                        
                }
            }
        }


    }

    //保存僵尸感知结果，用以给别处调用，比如状态改变的依据
    public Transform getNearbyPlayer()
    {
        return nearbyPlayer;
    }
}
