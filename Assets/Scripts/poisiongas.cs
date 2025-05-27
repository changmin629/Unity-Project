using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public int duration = 5; // ���� �ð�
    public int damagePerSecond = 1;

    private float damageInterval = 0.5f;
    private float nextDamageTime = 0f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        Player hp = other.GetComponent<Player>();
        if (hp != null && Time.time >= nextDamageTime)
        {
            hp.TakeDamage(damagePerSecond);
            nextDamageTime = Time.time + damageInterval;
        }
    }
}
