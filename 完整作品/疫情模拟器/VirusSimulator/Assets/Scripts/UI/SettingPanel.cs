using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public static SettingPanel instance;

    public InputField inputNum;
    public InputField inputInitNum;
    public InputField inputRate;
    public InputField inputQFTime;
    public InputField inputCWNum;
    public InputField inputSZTime;
    public InputField inputYX;
    public Button btnBegin;

    private void Awake()
    {
        instance = this;
        btnBegin.onClick.AddListener(ClickBegin); 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClickBegin()
    {
        //重置病毒数据，然后创建人
        RestInfo();  
        City.instance.CreatePerson(int.Parse(inputNum.text));
    }

    private void RestInfo()
    {
        //面板设置的数据和virus类里的数据对应上
        Virus.INFECTION_NUM = int.Parse(inputInitNum.text);
        Virus.TRANSMISSION_RATE = int.Parse(inputRate.text);
        Virus.HIDE_TIME = int.Parse(inputQFTime.text);
        Virus.HOSPITAL_BED_NUM = int.Parse(inputCWNum.text);
        Virus.HOSPITAL_TIME = int.Parse(inputSZTime.text);
        Virus.MOVE_INTENTION = int.Parse(inputYX.text);
    }
}
