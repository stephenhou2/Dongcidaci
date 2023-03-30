using GameEngine;
using System.Collections.Generic;

/// <summary>
/// 如果需要临时性的使用update功能，可以注册到CommonUpdateCenter中
/// 注册进CommonUpdateCenter得updater，更新顺序和注册顺序无关
/// </summary>
public class CommonUpdateCenter : ModuleManager<CommonUpdateCenter>
{
    private List<IGameUpdate> mToUpdates;
    private SimpleQueue<int> mValidIndexs;

    // 容量不足时单次补充的容量大小
    private int Capacity = 1024;

    public override void Initialize()
    {
        mToUpdates = new List<IGameUpdate>(Capacity);
        mValidIndexs = new SimpleQueue<int>();
    }

    public int RegisterUpdater(IGameUpdate updater)
    {
        // 如果没有可用的空位，则一次性补出Capacity个空位出来
        if (mValidIndexs.Count <= 0)
        {
            int curCapacity = mToUpdates.Count;

            for (int i = 0; i < Capacity; i++)
            {
                int indexAdd = curCapacity + i;
                mValidIndexs.Enqueue(indexAdd);
                mToUpdates.Add(null);
            }
        }

        // 把updater放在首个出队的空位上
        int index = mValidIndexs.Dequeue();
        mToUpdates[index] = updater;
        return index;
    }

    public void UnregisterUpdater(int index)
    {
        mToUpdates[index] = null;
        mValidIndexs.Enqueue(index);
    }

    public override void OnUpdate(float deltaTime)
    {
        for(int i=0;i<mToUpdates.Count;i++)
        {
            if(mToUpdates[i] != null)
            {
                mToUpdates[i].OnUpdate(deltaTime);
            }
        }
    }

    public override void Dispose()
    {
        mToUpdates = null;
        mValidIndexs = null;
    }
}
