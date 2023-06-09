using UnityEngine;
public class LoopMoveWithMeter : BehaviourWithMeter
{
    /// <summary>
    /// 原始位置
    /// </summary>
    private Vector3 mOriPos;

    /// <summary>
    /// 偏移的位移
    /// </summary>
    public Vector3 MoveOffset;

    /// <summary>
    /// 移动的时间
    /// </summary>
    public float MoveDuration;

    protected override void Initialize()
    {
        base.Initialize();
        mOriPos = this.transform.position;
    }

    public override void OnMeter(int meterIndex)
    {
        meterTriggered = CheckTrigger(meterIndex);
        if (!meterTriggered)
            return;

        mOriPos = this.transform.position;
        timeRecord = 0;
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!UpdateEnable || !meterTriggered)
            return;

        if (timeRecord >= MoveDuration)
            return;

        float progress = timeRecord / MoveDuration;
        if(progress >= 0.5f)
        {
            progress = 1f - progress;
        }

        progress *= 2;

        // 计算移动的距离
        Vector3 offset = Vector3.Lerp(Vector3.zero, MoveOffset, progress);
        this.transform.position = mOriPos + offset;
        timeRecord += deltaTime;
        if (timeRecord >= MoveDuration)
        {
            this.transform.position = mOriPos;
        }
    }
}
