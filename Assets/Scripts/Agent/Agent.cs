using UnityEngine;

public abstract class Agent : IEntity, IMeterHandler
{
    /// <summary>
    /// 角色id
    /// </summary>
    protected uint mAgentId;
    /// <summary>
    /// 实体id
    /// </summary>
    protected int mEntityId;
    /// <summary>
    /// 角色的游戏体
    /// </summary>
    protected GameObject mAgentGo;

    // 相机追随的虚拟目标(防止动画自身位置)
    //protected VirtualCamTarget mVirtualCamTarget;

    /// <summary>
    /// 动画播放控制器
    /// </summary>
    public AgentAnimPlayer AnimPlayer;

    /// <summary>
    /// 动画状态机
    /// </summary>
    protected AgentStatusMachine StatusMachine;

    /// <summary>
    /// 移动控制
    /// </summary>
    public AgentMoveControl MoveControl;

    /// <summary>
    /// 角色状态信息表
    /// </summary>
    public AgentStatusGraph StatusGraph;

    public uint GetAgentId()
    {
        return mAgentId;
    }

    public int GetEntityId()
    {
        return mEntityId;
    }
    // 角色移动速度
    protected float mSpeed;
    // 角色的冲刺速度
    protected float mDashDistance;
    // 角色朝向
    protected Vector3 mTowards;
    // 角色位置
    protected Vector3 mPosition;

    public Vector3 GetPosition()
    {
        return mPosition;
    }

    public void SetPosition(Vector3 position)
    {
        mPosition = position;
        if(mAgentGo != null)
        {
            mAgentGo.transform.position = position;
        }
    }

    public float GetSpeed()
    {
        return mSpeed;
    }

    public void SetSpeed(float speed)
    {
        mSpeed = speed;
        Log.Logic(LogLevel.Info, "set speed:{0}", speed);
    }

    public float GetDashDistance()
    {
        return mDashDistance;
    }

    public void SetDashDistance(float dashDistance)
    {
        this.mDashDistance = dashDistance;
    }

    public Vector3 GetTowards()
    {
        return mTowards.normalized;
    }

    public void SetTowards(Vector3 towards)
    {
        mTowards = towards;
        if(mAgentGo != null)
        {
            mAgentGo.transform.rotation = Quaternion.LookRotation(towards);
            //Log.Logic(LogLevel.Info, "SetTowards-----{0}", towards);
        }
    }


    /// <summary>
    /// 加载角色配置信息
    /// </summary>
    /// <param name="agentId"></param>
    protected abstract void LoadAgentCfg(uint agentId);

    /// <summary>
    /// 加载角色物件
    /// </summary>
    protected abstract void LoadAgentGo();

    /// <summary>
    /// 自定义的初始化
    /// </summary>
    protected abstract void CustomInitialize();

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize()
    {
        LoadAgentCfg(mAgentId);
        LoadAgentGo();
        CustomInitialize();
        StatusGraph = DataCenter.Ins.AgentStatusGraphCenter.GetAgentStatusGraph(mAgentId);
        StatusMachine = new AgentStatusMachine();
        StatusMachine.Initialize(this);
        MeterManager.Ins.RegisterMeterHandler(this);
    }

    public virtual void Dispose()
    {
        EntityManager.Ins.RemoveEntity(this);
        mAgentGo = null;
        AnimPlayer = null;
    }

    public Agent(uint agentId)
    {
        mAgentId = agentId;
        mEntityId = EntityManager.Ins.AddEntity(this);
        AnimPlayer = new AgentAnimPlayer();
    }

    public void OnMeter(int meterIndex)
    {
        StatusMachine.OnMeter(meterIndex);
    }
    public void OnCommand(AgentInputCommand cmd)
    {
        if (StatusMachine != null)
        {
            StatusMachine.OnCommand(cmd);
        }

        AgentInputCommandPool.Ins.PushAgentInputCommand(cmd);
    }

    /// <summary>
    /// update
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void OnUpdate(float deltaTime)
    {
        MoveControl.OnUpdate(deltaTime);
        StatusMachine.OnUpdate(deltaTime);
    }
}
