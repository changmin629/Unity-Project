using UnityEngine;

// Item Ŭ������ ���� ������ �÷��̾ ȹ���� �� �ִ� �������� �⺻���� ������ �����մϴ�.
public class Item : MonoBehaviour
{
    // �������� ������ �����ϴ� ������ (Enum)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;   // �� �������� ����
    public int value;   // �������� ��ġ (��: ź�� ����, ���� �ݾ�, ü�� ȸ���� ��)

    Rigidbody rigid;          // �������� ������ �������� ������ Rigidbody ������Ʈ
    SphereCollider sphereCollider; // �������� �浹�� ������ SphereCollider ������Ʈ

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� ȣ��˴ϴ�. (���� �� �� ��)
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();           // �� ������Ʈ�� �پ��ִ� Rigidbody ������Ʈ�� ������ �Ҵ��մϴ�.
        sphereCollider = GetComponent<SphereCollider>(); // �� ������Ʈ�� �پ��ִ� SphereCollider ������Ʈ�� ������ �Ҵ��մϴ�.
    }

    // �� ������ ������Ʈ�˴ϴ�.
    void Update()
    {
        // ������ ������Ʈ�� Y���� �������� �ʴ� 20���� ȸ����ŵ�ϴ�.
        // �̴� �������� ���� ��� �Ͽ� �÷��̾ ���� �ν��ϰ� ȹ���� �� �ֵ��� �����ϴ�.
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    // �ٸ� Collider�� ������ �浹�� �߻����� �� ȣ��˴ϴ�.
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �±װ� "Floor" (�ٴ�)�̶��
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;     // Rigidbody�� Ű�׸�ƽ���� �����Ͽ� ������ ����(�߷�, �浹 ��)�� ���� �ʰ� �մϴ�.
                                          // �̴� �������� �ٴڿ� ������ �� �������� �ʰ� ���ڸ��� �����ǵ��� �մϴ�.
            sphereCollider.enabled = false; // SphereCollider�� ��Ȱ��ȭ�Ͽ� �� �̻� �浹�� �������� �ʰ� �մϴ�.
                                            // �÷��̾ �� �������� �ֿ� �� Ʈ���Ÿ� ����ϱ� ������ �� �ֽ��ϴ�.
        }
    }
}
