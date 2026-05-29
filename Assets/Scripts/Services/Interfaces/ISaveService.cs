public interface ISaveService : IService
{
    bool HasSave { get; }
    void Save();
    void Load();
    void PrepareTransit();
    void DeleteSave();
    bool IsSpawnerCleared(string id);
    void MarkSpawnerCleared(string id);
}
