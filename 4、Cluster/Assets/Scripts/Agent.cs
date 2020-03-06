using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用力来驱动个体
public class Agent : MonoBehaviour
{
    public Vector3 accelerate;
    public Vector3 velocity;
    public Vector3 mPos;
    private Transform _transform;

    private Manager manager;//真正做的时候建议换成单例
    private Config config;//这里最好是对不同的个体做不同的配置，然后动态加载   此处为了方便修改属性直接看到变化，从简处理了。
    private float deltatime;
    
    public GameObject _target;


    private void Start()
    {
        manager = FindObjectOfType<Manager>();//此方法需慎用
        config = FindObjectOfType<Config>();
        _transform = transform;//缓存自身的位移组件
        mPos = _transform.position;
        deltatime = Time.deltaTime;
    }

    private void Update()
    {
        accelerate = Combine();
        accelerate = Vector3.ClampMagnitude(accelerate, config.maxAccelerate);
        velocity = velocity + accelerate * deltatime;
        velocity = Vector3.ClampMagnitude(velocity, config.maxVelocity);
        mPos = mPos + velocity * deltatime;
        ClampPos(ref mPos, -manager.bound, manager.bound);//限制不飞出去
        _transform.position = mPos;
        if (velocity.magnitude > 0)
            transform.LookAt(mPos + velocity);
        if (manager.useTargetForce)
        {
            if (_target != null && Vector3.Distance(_target.transform.position, mPos) < config.targetRadius)
                _target = null;
        }
    }
    private void ClampPos(ref Vector3 v, float min, float max)
    {
        v.x = ClampFloat(v.x, min, max);
        v.y = ClampFloat(v.y, min, max);
        v.z = ClampFloat(v.z, min, max);
    }

    private float ClampFloat(float value, float min, float max)
    {
        if (value > max)
            value = min;
        else if (value < min)
            value = max;
        return value;
    }

    Vector3 Cohesion()
    {
        Vector3 dir = Vector3.zero;
        //1、得到邻居
        List<Agent> agents = new List<Agent>();
        agents = manager.GetNeighbours(this, config.cohensionRadius);
        int listLen = agents.Count;
        int inFieldNum = 0;
        if (0 == listLen)
            return dir;
        //2、计算邻居中心点
        for (int i = 0; i < listLen; i++)
        {
            if (InField(agents[i].mPos, mPos))
            {
                dir += agents[i].mPos;
                ++inFieldNum;
            }
        }
        if (inFieldNum == 0)
            return dir;
        dir /= inFieldNum;
        //3、返回自身指向中心点的单位方向向量
        dir -= mPos;
        dir.Normalize();
        return dir;

    }

    Vector3 Seperation()
    {
        Vector3 dir = Vector3.zero;
        //1、得到邻居
        List<Agent> agents = new List<Agent>();
        agents = manager.GetNeighbours(this, config.cohensionRadius);
        int listLen = agents.Count;
        int inFieldNum = 0;
        if (0 == listLen)
            return dir;
        //2、遍历邻居，计算方向向量大小，如果大于0，则累加 (单位方向向量/方向向量的大小)
        for (int i = 0; i < listLen; i++)
        {
            Vector3 tempVector = mPos - agents[i].mPos;//反向的向量
            if (tempVector.magnitude > 0 && InField(agents[i].mPos, mPos))
            {
                dir += tempVector.normalized / tempVector.magnitude;
                ++inFieldNum;
            }
        }
        if (inFieldNum == 0)
            return dir;
        //3、做归一化
        dir.Normalize();
        return dir;
    }

    Vector3 Alignment()
    {
        Vector3 dir = Vector3.zero;
        //1、得到邻居
        List<Agent> agents = new List<Agent>();
        agents = manager.GetNeighbours(this, config.cohensionRadius);
        int listLen = agents.Count;
        int inFieldNum = 0;
        if (0 == listLen)
            return dir;
        //2、累加邻居的速度方向
        for (int i = 0; i < listLen; i++)
        {
            if (InField(agents[i].mPos, mPos))
            {
                dir += agents[i].velocity;
                ++inFieldNum;
            }
        }
        if (inFieldNum == 0)
            return dir;
        //3、做归一化
        dir.Normalize();
        return dir;
    }

    Vector3 TargetForce()
    {
        Vector3 dir = Vector3.zero;
        if (_target == null)
            return dir;
        dir = _target.transform.position - mPos;
        dir.Normalize();
        return dir;
    }

    Vector3 Combine()
    {
        Vector3 dir = Vector3.zero;
        if (manager.useTargetForce && _target != null)
            dir = Cohesion() * config.cohensionWeigth + Seperation() * config.seperateWeigth + Alignment() * config.alignmentWeigth + TargetForce() * config.targetWeigth;
        else
            dir = Cohesion() * config.cohensionWeigth + Seperation() * config.seperateWeigth + Alignment() * config.alignmentWeigth;
        return dir;
    }

    private bool InField(Vector3 target, Vector3 myPosition)
    {
        return Vector3.Angle(velocity, target - myPosition) <= config.maxField;
    }

}
