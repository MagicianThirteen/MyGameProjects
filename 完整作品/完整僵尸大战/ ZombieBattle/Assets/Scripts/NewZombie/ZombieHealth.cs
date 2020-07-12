using UnityEngine;
using System.Collections;

public class ZombieHealth :MonoBehaviour{
    //僵尸生命值脚本，主要记下中枪与否，中枪方向，死没死
    public int currentHP = 10;
    public int maxHP = 10;
    public int killScore = 5;
    [HideInInspector]//为了给别的状态转换提供判断依据，比如随机
    //游荡状态如果受到伤害，会进入搜索状态
    public bool getDamage = false;
    [HideInInspector]
    public Vector3 damageDirection = Vector3.zero;
    public AudioClip enemyHurtAudio;
    //通过标签知道是否活着
    public bool IsAlive
    {
        get
        {
            
            return currentHP > 0;
        }
    }
    //首先是受到伤害怎么办
    public void TakeDamage(int damage,Vector3 shootPosition)
    {
        if (!IsAlive)
            return;
        currentHP -= damage;
        if (currentHP <= 0) currentHP = 0;
        if (IsAlive)
        {
            getDamage = true;//记录中枪的状态
            damageDirection = shootPosition - transform.position;//记录中枪方向
            damageDirection.Normalize();

        }
        else
        {
            
            if (GameManager.gm != null)
            {
                GameManager.gm.AddScore(killScore);
            }
            
        }

        //击杀的音效
        if (enemyHurtAudio != null)
        {
            //参数音效，位置
            AudioSource.PlayClipAtPoint(enemyHurtAudio,transform.position);
        }
    }

}
