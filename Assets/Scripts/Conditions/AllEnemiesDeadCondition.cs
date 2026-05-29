using UnityEngine;

public class AllEnemiesDeadCondition : RoomCondition
{
    [SerializeField] private string _failedHint = string.Empty;
    [SerializeField] private EnemySpawner _enemySpawner;

    private bool _allEnemiesDead = false;

    public override bool IsMet => _allEnemiesDead;

    public override string FailedHint => _failedHint;

    private void Awake()
    {
        _enemySpawner.OnAllEnemiesDead += () => _allEnemiesDead = true;
    }

    private void OnDestroy()
    {
        _enemySpawner.OnAllEnemiesDead -= () => _allEnemiesDead = true;
    }
}
