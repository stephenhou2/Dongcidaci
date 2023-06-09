using UnityEngine;

public static class GamePlayDefine
{
    /// <summary>
    /// 攻击指令的节拍检测容差
    /// 向前和向后各
    /// </summary>
    public const float AttackMeterCheckTolerance = 0.3f;

    /// <summary>
    /// 冲刺指令的节拍检测容差
    /// 向前和向后各
    /// </summary>
    public const float DashMeterCheckTolerance = 0.3f;

    /// <summary>
    /// 处于输入检测阶段，空状态的最长时间
    /// </summary>
    public const float EmptyStatusMaxTime = 0.3f;

    /// <summary>
    /// 攻击指令执行的节拍进度判定
    /// </summary>
    public const float AttackMeterProgressWait = 0.5f;

    /// <summary>
    /// 冲刺指令执行的节拍进度判定
    /// </summary>
    public const float DashMeterProgressWait = 0.5f;

    /// <summary>
    /// 冲刺时间占一拍时间的百分比
    /// </summary>
    public const float DashMeterTime = 0.8f;

    /// <summary>
    /// 空参数
    /// </summary>
    public static Vector3 InputDirection_NONE = Vector3.zero;

}
