using System.Collections.Generic;

public class AgentStatus_Run : AgentStatus
{
    public override string GetStatusName()
    {
        return AgentStatusDefine.RUN;
    }

    public override void OnAction(int action)
    {
        if(action == AgentActionDefine.IDLE)
        {
            ChangeStatus(AgentStatusDefine.IDLE, null);
        }

        if(action == AgentActionDefine.RUN)
        {
            return;
        }
        
    }

    public override void OnEnter(Dictionary<string, object> context)
    {
        AgentStatusInfo statusInfo = mAgent.StatusGraph.GetStatusInfo(GetStatusName());
        if (statusInfo == null)
            return;


    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate(float deltaTime)
    {
        
    }

}
