using UnityEngine;

public class KeyboardInputHandle_BeHit : AgentKeyboardInputHandle
{
    public KeyboardInputHandle_BeHit(Agent agt) : base(agt)
    {
    }

    public override string GetHandleName()
    {
        return InputDef.KeyboardInputHandle_BeHit;
    }

    public override void OnMeter(int meterIndex)
    {
        
    }

    public override void OnUpdate(float deltaTime)
    {

    }
}
