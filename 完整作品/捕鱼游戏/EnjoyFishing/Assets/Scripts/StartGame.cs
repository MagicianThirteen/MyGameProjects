using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// 加载游戏加载界面
/// </summary>
public class StartGame : MonoBehaviour
{
    private Button startBtn;
    // Start is called before the first frame update
    void Start()
    {
        startBtn = GetComponent<Button>();
        startBtn.onClick.AddListener(LoadGame);
    }

    public void LoadGame()
    {
        //加载，加载页面
        SceneManager.LoadScene(1);
    }

   
}
