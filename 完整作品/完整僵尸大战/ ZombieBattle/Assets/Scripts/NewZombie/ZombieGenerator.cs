using UnityEngine;
using System.Collections;

public class ZombieGenerator : MonoBehaviour
{
    private GameObject[] instances;
    public GameObject zombiePrefab;
    public int maximumInstanceCount=10;
    public static Vector3 defaultPosition=new Vector3(33,-6,-8);
    public Transform[] zombieSpawnTransform;//生成地数组
    public float maxGenerateTimeInterval = 10f;
    public float minGenerateTimeInterval = 3f;
    private float timer = 0.0f;//有时间间隔就需要计时器
    private float nextGenerationTime = 0.0f;//即不要固定的时间间隔，在最大和最小间隔之间

    private void Start()
    {
        //初始化对象池
        instances=new GameObject[maximumInstanceCount];
        for(int i = 0; i < maximumInstanceCount;i++)
        {
            GameObject zombie = Instantiate(zombiePrefab,
                defaultPosition,Quaternion.identity) as GameObject;
            zombie.SetActive(false);
            instances[i] = zombie;
            
        }

    }
    //从对象池获取一个禁用状态的僵尸对象
    private GameObject GetNextAvailiableInstance()
    {
        for(var i = 0; i < maximumInstanceCount; i++)
        {
            if (!instances[i].activeSelf)
            {
                return instances[i];
            }
        }
        return null;
    }

    //在指定位置生成一个僵尸,激活，添加组件zombieai，初始化别的属性
    //注意这里只是生成一个，在持续生成处调用
    private bool generate(Vector3 position)
    {
        GameObject zombie = GetNextAvailiableInstance();
        if (zombie != null)
        {
            zombie.SetActive(true);
            zombie.GetComponent<ZombieAI>().Born(position);
            return true;
        }
        return false;
    }

    private void Update()
    {
        //条件：只有在玩的状态才生成
        if (GameManager.gm.gameState !=
            GameManager.GameState.Playing)
            return;
        if (timer > nextGenerationTime)
        {
            //选择一个出生点，生成一个僵尸，下一次生成僵尸的随机时间间隔，
            //计时器清零
            int i = Random.Range(0, zombieSpawnTransform.Length);
            generate(zombieSpawnTransform[i].position);
            nextGenerationTime = Random.Range(minGenerateTimeInterval,
                maxGenerateTimeInterval);
            timer = 0;
            
        }
        timer += Time.deltaTime;

        
    }

}
