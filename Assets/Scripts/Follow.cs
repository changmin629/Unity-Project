using UnityEngine;

// NewMonoBehaviourScript (�Ϲ������� Follow ������ ���� �� ����) Ŭ������
// Ư�� Transform(target)�� ����ٴϴ� ������Ʈ�� �������� �����մϴ�.
public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform target; // �� ������Ʈ�� ���� ��ǥ Transform (��: �÷��̾�, ī�޶� ���)
    public Vector3 offset;   // ��ǥ Transform���� ������� ��ġ ���� (������)

    // �� ������ ������Ʈ�˴ϴ�.
    void Update()
    {
        // �� ������Ʈ�� ��ġ�� ��ǥ(target)�� ��ġ�� �������� ���� ������ �����մϴ�.
        // �̸� ���� ������Ʈ�� �� ������ ��ǥ�� ����ٴϸ�, �����¸�ŭ ������ ��ġ�� �����˴ϴ�.
        transform.position = target.position + offset;
    }
}
