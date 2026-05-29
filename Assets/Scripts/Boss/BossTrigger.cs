using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    private BossController _boss;

    public void Init(BossController boss) => _boss = boss;

    private void OnTriggerEnter(Collider other)
    {
        if (_boss == null || !other.CompareTag("Player")) return;
        _boss.ActivateBoss();
        gameObject.SetActive(false);
    }
}
