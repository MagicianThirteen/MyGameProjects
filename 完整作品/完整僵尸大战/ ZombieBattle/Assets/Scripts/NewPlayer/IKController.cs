using UnityEngine;
using System.Collections;

public class IKController : MonoBehaviour {
    //似乎设置ik都要设置相应的权重？weight？
    //AvatarIKGoal包括，左手，右手，左脚，右脚
    Animator animator;//动画控制器
    public bool isActive = true;//是否启用ik
    //以下分别是头部，左右手，躯干身体的ik标记物
    public Transform lookObj = null;
    public Transform leftHandObj = null;
    public Transform rightHandObj = null;
    public Transform bodyObj = null;

	void Start(){
        animator = GetComponent<Animator>();
	}

    private void OnAnimatorIK()//这里是固定函数
    {
        //条件：是否有动画控制器，是否手动开启ik
        if (animator&&isActive)
        {
            //头部ik标记物，先面向标记物，再设置权重
            if (lookObj != null)
            {
                //lookat本身包含了上下左右旋转，和位置
                animator.SetLookAtPosition(lookObj.position);
                animator.SetLookAtWeight(1.0f);
            }
            //躯干和头，手不同，不能随意上下摆动，只能旋转
            //所以设置bodyrotation属性就可以了
            if (bodyObj != null)
            {
                animator.bodyRotation = bodyObj.rotation;
            }
            //左手ik标记物，ik的position，rotation，以及对应的权重
            if (leftHandObj != null)
            {
                //参数解析：ik标记物需要与身体哪个部位对应，权重也要点名
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            }
            //右手同理
            if (rightHandObj != null)
            {
                //参数解析：ik标记物需要与身体哪个部位对应，权重也要点名
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

            }

        }
        else
        {
            //没有开启ik的话，就用正向动力学，即把所有的ik position
            //rotation 权重设置为0
            animator.SetLookAtWeight(0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        }
        
    }


}
