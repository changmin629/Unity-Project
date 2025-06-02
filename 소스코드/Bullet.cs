using UnityEngine;

// Bullet Ŭ������ ���� ������ �߻�Ǵ� ��� �Ѿ�(����ü)�� �⺻���� ������ �����մϴ�.
// �̻���, ���� �� �پ��� ����ü�� �� Ŭ������ ��ӹ޾� ����� �� �ֽ��ϴ�.
public class Bullet : MonoBehaviour
{
    public int damage;    // �Ѿ��� ���ݷ� (������ ���� ������)
    public bool isMelee;  // �� �Ѿ��� ���� ����(��: Į �ֵθ���)�� ���� ������ ���� 
    public bool isRock;   // �� �Ѿ��� ���� ��(����)���� ���� 

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    void Start()
    {
        // �Ѿ��� ������ �� 5�� �ڿ� �ڵ����� ���� ������Ʈ�� �ı��մϴ�.
        // �̴� ȭ�鿡 ���ʿ��ϰ� �����ִ� �Ѿ� ���� �ٿ� ������ ����ȭ�ϴ� �� ������ �ݴϴ�.
        Destroy(gameObject, 5f);
    }

    // �ٸ� Collider�� ������ �浹�� �߻����� �� ȣ��˴ϴ�.
    void OnCollisionEnter(Collision collision)
    {
        // ���� �� �Ѿ��� '����(isRock)'�� �ƴϰ�, �浹�� ������Ʈ�� �±װ� "Floor" (�ٴ�)�̶��
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            // �浹 �� 3�� �ڿ� ���� ������Ʈ�� �ı��մϴ�.
            Destroy(gameObject, 3f);
        }
    }

    // �ٸ� Collider�� Ʈ���� ������ �������� �� ȣ��˴ϴ�.
    void OnTriggerEnter(Collider other)
    {
        // ���� �� �Ѿ��� '���� ����(isMelee)'�� ���� ���� �ƴϰ�, �浹�� ������Ʈ�� �±װ� "Wall" (��)�̶��
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            // ��� ���� ������Ʈ�� �ı��մϴ�.
            // �̴� �Ѿ��� ���� �հ� �������� �ʵ��� �Ͽ� ���ǰ��� ���Դϴ�.
            Destroy(gameObject);
        }
    }
}
