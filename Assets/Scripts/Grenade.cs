using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Grenade 클래스는 수류탄 오브젝트의 동작을 정의합니다.
// 일정 시간 후 폭발하여 주변 적들에게 피해를 입힙니다.
public class Grenade : MonoBehaviour
{
    public GameObject meshObj;   // 수류탄의 실제 모델(메쉬) 오브젝트
    public GameObject effectObj; // 폭발 효과 오브젝트
    public Rigidbody rigid;      // 수류탄의 물리적 움직임을 제어할 Rigidbody 컴포넌트

    // 게임 오브젝트가 처음 활성화될 때 한 번 호출됩니다.
    void Start()
    {
        // 폭발 코루틴을 시작합니다.
        StartCoroutine(Explosion());
    }

    // 수류탄의 폭발 과정을 관리하는 코루틴
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f); // 3초 대기 (수류탄이 던져진 후 폭발까지의 시간)

        // 수류탄의 이동 및 회전을 멈춥니다.
        rigid.linearVelocity = Vector3.zero;  // 선형 속도 0으로 설정
        rigid.angularVelocity = Vector3.zero; // 각속도 0으로 설정

        meshObj.SetActive(false); // 수류탄 메쉬(모델)를 비활성화하여 숨깁니다.
        effectObj.SetActive(true); // 폭발 효과 오브젝트를 활성화하여 폭발 애니메이션 등을 재생합니다.

        // 현재 수류탄 위치를 중심으로 반경 15f 범위 내의 "Enemy" 레이어에 있는 모든 오브젝트를 감지합니다.
        // SphereCastAll은 구형으로 레이캐스트를 발사하며, 모든 히트를 반환합니다.
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        // 감지된 각 적 오브젝트에 대해 처리합니다.
        foreach (RaycastHit hitObj in rayHits)
        {
            // 감지된 오브젝트의 Transform에서 Enemy 컴포넌트를 가져와 HitByGrenade 메서드를 호출합니다.
            // 이때 수류탄의 위치를 인수로 넘겨 적이 폭발 지점에 따라 다른 반응을 보이도록 할 수 있습니다.
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        // 폭발 효과가 재생된 후 5초 뒤에 수류탄 게임 오브젝트 자체를 파괴합니다.
        Destroy(gameObject, 5);
    }
}
