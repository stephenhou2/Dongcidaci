public class MonsterBaseMeterHandler : IMeterHandler
{
    private int meterOffset;
    private Monster mMonster;
    public MonsterBaseMeterHandler(Monster monster)
    {
        mMonster = monster;
    }

    public int GetMeterOffset()
    {
        return meterOffset;
    }

    public void OnMeter(int meterIndex)
    {
        if (mMonster == null)
            return;
    }
}
