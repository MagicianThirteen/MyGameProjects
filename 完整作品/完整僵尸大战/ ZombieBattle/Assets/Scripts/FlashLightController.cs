using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

//玩家手电控制脚本
public class FlashLightController : MonoBehaviour {

    private Light mylight;
    private void Start()
    {
        mylight = GetComponent<Light>();
    }
    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        {
            if (mylight.intensity < 0.1f)//开
            {
                mylight.intensity = 5.0f;
            }
            else
            {
                mylight.intensity = 0.0f;//关
            }
        }
    }
}
