using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWeaponSwitcher : MonoBehaviour {

    public Transform[] weaponList;
    private IKController iKController;
    private int currentIdx = 0;
    private int weaponNum = 0;

    private void Start()
    {
        iKController = transform.GetComponent<IKController>();
        weaponNum = weaponList.Length;
        currentIdx = 0;
        changeNextWeapon();//第一次启用就要使用一次
    }

    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            changeNextWeapon();
        }
    }

    public void changeNextWeapon()
    {
        if (weaponNum <=1)
        {
            return;
        }

        //设置好序号
        int newIdx = (currentIdx + 1) % weaponNum;
        //设置好当前玩家的ik标记物
        Transform newWeapon = weaponList[newIdx];
        Transform rightHand = newWeapon.Find("RightHandObj");
        Transform leftHand = newWeapon.Find("LeftHandObj");
        Transform gunBarrelEnd = newWeapon.Find("GunBarrelEnd");
        iKController.leftHandObj = leftHand;
        iKController.rightHandObj = rightHand;
        iKController.lookObj = gunBarrelEnd;
        //设置好显示
        newWeapon.gameObject.SetActive(true);
        weaponList[currentIdx].gameObject.SetActive(false);
        //更新好序号
        currentIdx = newIdx;
    }
}
