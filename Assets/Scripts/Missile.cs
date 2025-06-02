using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Missile 클래스는 특정 유형의 미사일의 기본적인 동작을 정의합니다.
// 이 스크립트는 BossMissile과는 다르게, 특정 타겟을 추적하는 기능 없이 단순한 움직임을 가집니다.
public class Missile : MonoBehaviour
{
    // 매 프레임 업데이트됩니다.
    void Update()
    {
        // 미사일 오브젝트를 X축을 기준으로 초당 30도씩 회전시킵니다.
        // 이는 미사일이 날아가는 동안 회전하는 시각적 효과를 부여합니다.
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}