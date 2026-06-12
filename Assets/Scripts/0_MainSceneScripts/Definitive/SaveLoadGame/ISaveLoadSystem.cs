using UnityEngine;

public interface ISaveLoadSystem
{
    bool TryLoad(out GameMain_Data data);
    void Save(GameMain_Data data);
}
