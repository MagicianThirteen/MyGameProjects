using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float moveSpeed;
    public bool isGameBubble;//是否关闭泡泡效果
    // Start is called before the first frame update
    void Start()
    {
        //第一个是不要泡泡飞射出去效果，第二个是要泡泡飞射出去效果
        if (!isGameBubble)
        {
            
            moveSpeed = Random.Range(2f, 4f);
            Destroy(this.gameObject, Random.Range(0.5f,1f));
        }
        else
        {
            //这里7-10秒能走出屏幕
            moveSpeed = Random.Range(40f, 100f);
            Destroy(this.gameObject, Random.Range(7f, 10f));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime,Space.Self);
    }
}
