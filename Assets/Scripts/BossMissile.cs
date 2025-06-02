using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위한 네임스페이스

// BossMissile 클래스는 Bullet 클래스를 상속받아 보스 미사일의 특정 행동을 정의합니다.
public class BossMissile : Bullet
{
    public Transform target; // 미사일이 추적할 목표 (주로 플레이어)의 Transform
    NavMeshAgent nav;        // 미사일의 이동을 제어할 NavMeshAgent 컴포넌트

    // 게임 오브젝트가 처음 활성화될 때 호출됩니다. (시작 시 한 번)
    void Awake()
    {
        // 이 오브젝트에 붙어있는 NavMeshAgent 컴포넌트를 가져와 nav 변수에 할당합니다.
        nav = GetComponent<NavMeshAgent>();
    }

    // 매 프레임 업데이트됩니다.
    void Update()
    {
        // 추적할 목표(target)가 유효한 경우에만 실행
        if (target != null)
        {
            // NavMeshAgent의 목표 지점을 target의 현재 위치로 설정하여 미사일이 target을 추적하도록 합니다.
            nav.SetDestination(target.position);
        }
    }
}
