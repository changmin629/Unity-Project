using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Orbit Ŭ������ �� ������Ʈ�� Ư�� ��ǥ(target)�� �߽����� ȸ��(�˵� �)�ϵ��� ����ϴ�.
public class Orbit : MonoBehaviour
{
    public Transform target;     // �� ������Ʈ�� �˵� ��� �� �߽� ��ǥ Transform
    public float orbitSpeed;     // �˵� ��� �ӵ� (�ʴ� ����)
    Vector3 offSet;              // ��ǥ�κ����� ������� ��ġ ������

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    void Start()
    {
        // �ʱ� �������� ����մϴ�.
        // �� ������Ʈ�� ���� ��ġ���� ��ǥ�� ��ġ�� ���� ��ǥ�κ����� �ʱ� ��� ���͸� �����մϴ�.
        offSet = transform.position - target.position;
    }

    // �� ������ ������Ʈ�˴ϴ�.
    void Update()
    {
        // 1. ��ǥ ��ġ�� �������� ���Ͽ� �� ������Ʈ�� ��ġ�� �����մϴ�.
        // �� �ܰ踦 ���� ������Ʈ�� ��ǥ�� ����ٴϸ� �׻� ������ ������� �Ÿ��� �����Ϸ��� �մϴ�.
        transform.position = target.position + offSet;

        // 2. ��ǥ�� ��ġ�� �߽����� Vector3.up (Y��) �������� orbitSpeed��ŭ ȸ����ŵ�ϴ�.
        // Time.deltaTime�� ���Ͽ� ������ �ӵ��� �������� �ε巯�� ȸ���� ����ϴ�.
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // 3. ȸ�� �� ����� ��ġ�� ������� ���ο� �������� �ٽ� ����մϴ�.
        offSet = transform.position - target.position;
    }
}