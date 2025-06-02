using UnityEngine;

// PoisonGas Ŭ������ ������ ������ ������ �����մϴ�.
// �� ������ ���� �÷��̾�� ���������� ���ظ� �����ϴ�.
public class PoisonGas : MonoBehaviour
{
    public int duration = 5;      // ������ ������ ������ �ð� (�� ����)
    public int damagePerSecond = 1; // �ʴ� �÷��̾�� ���� ������

    private float damageInterval = 0.5f; // �������� ������ ���� (0.5�ʸ��� ������)
    private float nextDamageTime = 0f;   // ���� �������� ���� �ð�

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    private void Start()
    {
        // duration ������ ������ �ð�(��: 5��) �Ŀ� ������ ���� ������Ʈ�� �ı��մϴ�.
        Destroy(gameObject, duration);
    }

    // �ٸ� Collider�� �� ������Ʈ�� Ʈ���� ���� �ȿ� �ӹ����� ���� ��� ȣ��˴ϴ�.
    private void OnTriggerStay(Collider other)
    {
        // Ʈ���� �ȿ� �ִ� ������Ʈ���� Player ������Ʈ�� �����ɴϴ�.
        Player hp = other.GetComponent<Player>();

        // Player ������Ʈ�� �����ϰ�, ���� �ð��� ���� ������ �ð����� ũ�ų� ������
        if (hp != null && Time.time >= nextDamageTime)
        {
            // �÷��̾�� damagePerSecond ��ŭ�� ���ظ� �����ϴ�.
            hp.TakeDamage(damagePerSecond);
            // �̸� ���� ������ ���ݸ��� �������� ����˴ϴ�.
            nextDamageTime = Time.time + damageInterval;
        }
    }
}