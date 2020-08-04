using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 星星旋转闪耀特效
/// </summary>
public class Shine : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotateSpeed = 100.0f;
    private Image image;
    public bool isShow=true;
    //颜色变透明的速度
    public float speed=4;

    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        if (isShow)
        {
            image.color -= new Color(1, 1, 1, speed * Time.deltaTime);
            if (image.color.a <= 0.2)
            {
                isShow = false;
            }
        }
        else
        {
            image.color += new Color(1, 1, 1, speed * Time.deltaTime);
            if (image.color.a > 0.9)
            {
                isShow = true;
            }
        }
    }
}
