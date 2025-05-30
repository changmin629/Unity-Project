using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    void Start()
    {
        // �Ѿ��� ������ �� 5�� �ڿ� �ڵ� ����
        Destroy(gameObject, 5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3f); // �ٴڿ� ������ 3�� �� ����
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject); // ���� ������ ��� ����
        }
    }
}
