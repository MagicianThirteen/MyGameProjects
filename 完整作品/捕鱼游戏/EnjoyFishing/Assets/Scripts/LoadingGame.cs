using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 加载游戏场景，平滑的加载到底的方式加载
/// </summary>
public class LoadingGame : MonoBehaviour
{

    private Slider loadSlider;
    private float displayProgress = 0;//负责显示的数值
    private float toProgress = 0;//实际要追的数值
    public float lerpSpeed = 1000f;
    private AsyncOperation op;
    // Start is called before the first frame update
    void Start()
    {
        loadSlider = GetComponent<Slider>();
        op = SceneManager.LoadSceneAsync(2);
    }

    // Update is called once per frame
    void Update()
    {
       
        toProgress = (int)op.progress * 100;
        Debug.Log(toProgress);
        if (displayProgress != toProgress)
        {
            displayProgress = Mathf.Lerp(displayProgress, toProgress,Time.deltaTime*lerpSpeed);
           // Debug.Log(displayProgress);
            loadSlider.value = displayProgress / 100;
        }
    }

    
    
}
