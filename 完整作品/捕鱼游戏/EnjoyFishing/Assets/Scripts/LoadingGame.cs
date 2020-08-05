using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 脚本作用：加载游戏场景，平滑的加载到底的方式加载
/// 脚本位置：挂在1号场景的，滑动条上
/// </summary>
public class LoadingGame : MonoBehaviour
{
	private AsyncOperation op;
	private Slider loadSlider;
	private int toProgress = 0;
	private int displayProgress = 0;
	
	void Start()
	{
		loadSlider = GetComponent<Slider>();
		StartCoroutine(LoadGame(2));
	}

	IEnumerator LoadGame(int index)
    {
		op = SceneManager.LoadSceneAsync(index);
		op.allowSceneActivation = false;
		yield return op;
    }

	void Update()
	{
        if (op == null)
        {
			return;
        }
		if (op.progress < 0.9f)
        {
			toProgress = (int)(op.progress*100);
			
        }
        else
        {
			toProgress = 100;
        }

        if (displayProgress < toProgress)
        {
			displayProgress++;
			
        }
		loadSlider.value = displayProgress / 100f;
		if (displayProgress == 100)
        {
			op.allowSceneActivation = true;
        }
		
	}	
}
