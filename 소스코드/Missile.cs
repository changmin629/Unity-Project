using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Missile Ŭ������ Ư�� ������ �̻����� �⺻���� ������ �����մϴ�.
// �� ��ũ��Ʈ�� BossMissile���� �ٸ���, Ư�� Ÿ���� �����ϴ� ��� ���� �ܼ��� �������� �����ϴ�.
public class Missile : MonoBehaviour
{
    // �� ������ ������Ʈ�˴ϴ�.
    void Update()
    {
        // �̻��� ������Ʈ�� X���� �������� �ʴ� 30���� ȸ����ŵ�ϴ�.
        // �̴� �̻����� ���ư��� ���� ȸ���ϴ� �ð��� ȿ���� �ο��մϴ�.
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}