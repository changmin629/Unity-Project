using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// StartZone Ŭ������ ������ �������� ���� ������ ��Ÿ���ϴ�.
// �÷��̾ �� ������ �����ϸ� ���� �������� ���� ������ ȣ���մϴ�.
public class StartZone : MonoBehaviour
{
    public GameManager manager; // GameManager ��ũ��Ʈ�� ���� 

    // �ٸ� Collider�� �� ������Ʈ�� Ʈ���� ������ �������� �� ȣ��˴ϴ�.
    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ���� ������Ʈ�� �±װ� "Player"�� ���
        if (other.gameObject.tag == "Player")
        {
            // GameManager�� StageStart() �޼��带 ȣ���Ͽ� �������� ���� ������ �����մϴ�.
            manager.StageStart();
        }
    }
}