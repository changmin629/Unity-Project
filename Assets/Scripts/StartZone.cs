using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// StartZone 클래스는 게임의 스테이지 시작 지점을 나타냅니다.
// 플레이어가 이 영역에 진입하면 게임 스테이지 시작 로직을 호출합니다.
public class StartZone : MonoBehaviour
{
    public GameManager manager; // GameManager 스크립트의 참조 

    // 다른 Collider가 이 오브젝트의 트리거 영역에 진입했을 때 호출됩니다.
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 게임 오브젝트의 태그가 "Player"인 경우
        if (other.gameObject.tag == "Player")
        {
            // GameManager의 StageStart() 메서드를 호출하여 스테이지 시작 로직을 실행합니다.
            manager.StageStart();
        }
    }
}