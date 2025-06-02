using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Orbit 클래스는 이 오브젝트가 특정 목표(target)를 중심으로 회전(궤도 운동)하도록 만듭니다.
public class Orbit : MonoBehaviour
{
    public Transform target;     // 이 오브젝트가 궤도 운동을 할 중심 목표 Transform
    public float orbitSpeed;     // 궤도 운동의 속도 (초당 각도)
    Vector3 offSet;              // 목표로부터의 상대적인 위치 오프셋

    // 게임 오브젝트가 처음 활성화될 때 한 번 호출됩니다.
    void Start()
    {
        // 초기 오프셋을 계산합니다.
        // 이 오브젝트의 현재 위치에서 목표의 위치를 빼서 목표로부터의 초기 상대 벡터를 저장합니다.
        offSet = transform.position - target.position;
    }

    // 매 프레임 업데이트됩니다.
    void Update()
    {
        // 1. 목표 위치에 오프셋을 더하여 이 오브젝트의 위치를 설정합니다.
        // 이 단계를 통해 오브젝트는 목표를 따라다니며 항상 동일한 상대적인 거리를 유지하려고 합니다.
        transform.position = target.position + offSet;

        // 2. 목표의 위치를 중심으로 Vector3.up (Y축) 방향으로 orbitSpeed만큼 회전시킵니다.
        // Time.deltaTime을 곱하여 프레임 속도에 독립적인 부드러운 회전을 만듭니다.
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // 3. 회전 후 변경된 위치를 기반으로 새로운 오프셋을 다시 계산합니다.
        offSet = transform.position - target.position;
    }
}