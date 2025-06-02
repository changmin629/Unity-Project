using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent�� ����ϱ� ���� ���ӽ����̽�

// BossMissile Ŭ������ Bullet Ŭ������ ��ӹ޾� ���� �̻����� Ư�� �ൿ�� �����մϴ�.
public class BossMissile : Bullet
{
    public Transform target; // �̻����� ������ ��ǥ (�ַ� �÷��̾�)�� Transform
    NavMeshAgent nav;        // �̻����� �̵��� ������ NavMeshAgent ������Ʈ

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� ȣ��˴ϴ�. (���� �� �� ��)
    void Awake()
    {
        // �� ������Ʈ�� �پ��ִ� NavMeshAgent ������Ʈ�� ������ nav ������ �Ҵ��մϴ�.
        nav = GetComponent<NavMeshAgent>();
    }

    // �� ������ ������Ʈ�˴ϴ�.
    void Update()
    {
        // ������ ��ǥ(target)�� ��ȿ�� ��쿡�� ����
        if (target != null)
        {
            // NavMeshAgent�� ��ǥ ������ target�� ���� ��ġ�� �����Ͽ� �̻����� target�� �����ϵ��� �մϴ�.
            nav.SetDestination(target.position);
        }
    }
}
