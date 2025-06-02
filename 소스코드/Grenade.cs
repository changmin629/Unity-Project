using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Grenade Ŭ������ ����ź ������Ʈ�� ������ �����մϴ�.
// ���� �ð� �� �����Ͽ� �ֺ� ���鿡�� ���ظ� �����ϴ�.
public class Grenade : MonoBehaviour
{
    public GameObject meshObj;   // ����ź�� ���� ��(�޽�) ������Ʈ
    public GameObject effectObj; // ���� ȿ�� ������Ʈ
    public Rigidbody rigid;      // ����ź�� ������ �������� ������ Rigidbody ������Ʈ

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    void Start()
    {
        // ���� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(Explosion());
    }

    // ����ź�� ���� ������ �����ϴ� �ڷ�ƾ
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f); // 3�� ��� (����ź�� ������ �� ���߱����� �ð�)

        // ����ź�� �̵� �� ȸ���� ����ϴ�.
        rigid.linearVelocity = Vector3.zero;  // ���� �ӵ� 0���� ����
        rigid.angularVelocity = Vector3.zero; // ���ӵ� 0���� ����

        meshObj.SetActive(false); // ����ź �޽�(��)�� ��Ȱ��ȭ�Ͽ� ����ϴ�.
        effectObj.SetActive(true); // ���� ȿ�� ������Ʈ�� Ȱ��ȭ�Ͽ� ���� �ִϸ��̼� ���� ����մϴ�.

        // ���� ����ź ��ġ�� �߽����� �ݰ� 15f ���� ���� "Enemy" ���̾ �ִ� ��� ������Ʈ�� �����մϴ�.
        // SphereCastAll�� �������� ����ĳ��Ʈ�� �߻��ϸ�, ��� ��Ʈ�� ��ȯ�մϴ�.
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        // ������ �� �� ������Ʈ�� ���� ó���մϴ�.
        foreach (RaycastHit hitObj in rayHits)
        {
            // ������ ������Ʈ�� Transform���� Enemy ������Ʈ�� ������ HitByGrenade �޼��带 ȣ���մϴ�.
            // �̶� ����ź�� ��ġ�� �μ��� �Ѱ� ���� ���� ������ ���� �ٸ� ������ ���̵��� �� �� �ֽ��ϴ�.
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        // ���� ȿ���� ����� �� 5�� �ڿ� ����ź ���� ������Ʈ ��ü�� �ı��մϴ�.
        Destroy(gameObject, 5);
    }
}
