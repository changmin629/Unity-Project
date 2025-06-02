using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BossRock Ŭ������ Bullet Ŭ������ ��ӹ޾� ���� ���Ͱ� �߻��ϴ� ������ Ư�� ������ �����մϴ�.
public class BossRock : Bullet
{
    Rigidbody rigid;    // ������ ������ �������� ������ Rigidbody ������Ʈ
    float angularPower = 2; // ������ ȸ���� (�ʱⰪ 2)
    float scaleValue = 0.1f;  // ������ ũ�� (�ʱⰪ 0.1)
    bool isShoot;       // ������ �߻�� �غ� �Ǿ����� ���θ� ��Ÿ���� �÷���

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� ȣ��˴ϴ�. (���� �� �� ��)
    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // �� ������Ʈ�� �پ��ִ� Rigidbody ������Ʈ�� ������ �Ҵ��մϴ�.
        StartCoroutine(GainPowerTimer()); // Ư�� �ð� �� �߻� �غ� �ϷḦ �˸��� �ڷ�ƾ ����
        StartCoroutine(GainPower());      // �߻� ������ ������ ���� �����ϰ� ũ�⸦ Ű��� �ڷ�ƾ ����
    }

    // ���� �ð� �� ������ �߻� �غ� �Ǿ����� �˸��� Ÿ�̸� �ڷ�ƾ
    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f); // 2.2�� ���
        isShoot = true; // 2.2�ʰ� ������ isShoot �÷��׸� true�� �����Ͽ� �߻� �غ� �Ϸ�
    }

    // ������ ���� �����ϰ� ũ�⸦ Ű��� �ڷ�ƾ
    IEnumerator GainPower()
    {
        // isShoot�� true�� �� ������ �ݺ��մϴ�.
        while (!isShoot)
        {
            angularPower += 0.05f; // ȸ������ ���������� ������ŵ�ϴ�.
            scaleValue += 0.003f;  // ũ�� ���� ���������� ������ŵ�ϴ�.
            transform.localScale = Vector3.one * scaleValue; // ������ ���� �������� scaleValue�� ���� �����Ͽ� ũ�⸦ Ű��ϴ�.
            // ������ ȸ����(��ũ)�� ���Ͽ� ȸ����ŵ�ϴ�. (transform.right �������� angularPower��ŭ ����)
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null; // ���� �����ӱ��� ���
        }
    }
}