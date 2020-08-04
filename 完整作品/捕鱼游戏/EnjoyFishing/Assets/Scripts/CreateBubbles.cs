using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  每隔一段时间，产生泡泡
/// </summary>
public class CreateBubbles : MonoBehaviour
{
    public GameObject bubble;
    public Transform bg;
    private float timeval;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeval > 1)
        {
            for(int i = 0; i < 2; i++)
            {
                //一秒产生一个泡泡
                Invoke("InitBubbles", 1);
            }
            timeval = 0;
        }
        else
        {
            timeval += Time.deltaTime;
        }
    }

    private void InitBubbles()
    {
        Instantiate(bubble, transform.position, Quaternion.Euler(0, 0, Random.Range(-80, 0)), bg);
    }
}
