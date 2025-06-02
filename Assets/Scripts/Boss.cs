using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위한 네임스페이스

// Boss 클래스는 Enemy 클래스를 상속받아 보스 몬스터의 특화된 행동을 정의합니다.
public class Boss : Enemy
{
    // 보스 미사일 관련 변수
    public GameObject missile;    // 보스가 발사할 미사일 프리팹
    public Transform missilePortA; // 미사일 발사 지점 A
    public Transform missilePortB; // 미사일 발사 지점 B (현재 코드에서는 A만 사용)
    public bool isLook;           // 보스가 플레이어를 바라볼지 (true) 또는 특정 위치로 이동할지 (false) 결정

    // 보스 행동 관련 내부 변수
    Vector3 lookVec;  // 보스가 바라볼 방향을 계산하는 데 사용되는 벡터
    Vector3 tauntVec; // 보스가 도발 행동 시 이동할 목표 위치

    // 게임 오브젝트가 처음 활성화될 때 호출됩니다. (시작 시 한 번)
    void Awake()
    {
        // 필요한 컴포넌트들을 가져와 할당합니다. (Enemy 클래스에서 정의된 변수들)
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true; // 시작 시 NavMeshAgent 이동을 정지시킵니다.

        StartCoroutine(Think()); // 보스 행동을 결정하는 Think 코루틴을 시작합니다.
    }

    // 매 프레임 업데이트됩니다.
    void Update()
    {
        // 보스가 사망 상태이면 모든 코루틴을 중지하고 더 이상 업데이트하지 않습니다.
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        // isLook 상태에 따라 보스의 회전 또는 이동을 결정합니다.
        if (isLook)
        {
            // 수평 및 수직 입력 값을 가져와 lookVec를 계산합니다.
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            // 플레이어(Target)의 위치에 lookVec를 더한 지점을 바라보도록 회전합니다.
            transform.LookAt(Target.position + lookVec);
        }
        else // isLook이 false이면 (주로 Taunt 시)
        {
            nav.SetDestination(tauntVec); // tauntVec으로 NavMeshAgent의 목표 지점을 설정하여 이동합니다.
        }
    }

    // 보스의 다음 행동을 결정하는 코루틴
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기

        int ranAction = Random.Range(0, 5); // 0부터 4까지의 랜덤 정수를 생성 (5가지 행동)
        switch (ranAction)
        {
            case 0:
            case 1:
            case 2: // 0, 1, 2일 경우 미사일 공격
                StartCoroutine(MissileShot());
                break;
            case 3: // 3일 경우 락샷 공격
                StartCoroutine(RockShot());
                break;
            case 4: // 4일 경우 도발 행동
                StartCoroutine(Taunt());
                break;
        }
    }

    // 미사일 공격 코루틴
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot"); // "doShot" 애니메이션 트리거를 활성화합니다.
        yield return new WaitForSeconds(0.2f); // 0.2초 대기

        // 미사일 프리팹과 발사 지점이 유효한 경우에만 실행
        if (missile != null && missilePortA != null)
        {
            // 첫 번째 미사일 생성 및 설정
            GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
            BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
            bossMissileA.target = Target; // 미사일의 타겟을 플레이어로 설정

            yield return new WaitForSeconds(0.3f); // 0.3초 대기

            // 두 번째 미사일 생성 및 설정 (현재는 missilePortA에서 두 번 발사됨)
            GameObject instantMissileB = Instantiate(missile, missilePortA.position, missilePortA.rotation);
            BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
            bossMissileB.target = Target; // 미사일의 타겟을 플레이어로 설정
        }

        yield return new WaitForSeconds(2f); // 2초 대기 (공격 쿨타임)
        StartCoroutine(Think()); // 다음 행동을 결정하기 위해 Think 코루틴 다시 시작
    }

    // 락샷 공격 코루틴 (커다란 바위 발사 등)
    IEnumerator RockShot()
    {
        isLook = false; // 플레이어를 바라보지 않도록 설정 (고정된 공격 애니메이션 동안)
        anim.SetTrigger("doBigShot"); // "doBigShot" 애니메이션 트리거를 활성화합니다.

        if (bullet != null) // 탄환 프리팹이 유효하면
            Instantiate(bullet, transform.position, transform.rotation); // 보스 위치에서 탄환 생성

        yield return new WaitForSeconds(3f); // 3초 대기 (공격 애니메이션 및 발사 시간)
        isLook = true; // 다시 플레이어를 바라보도록 설정

        StartCoroutine(Think()); // 다음 행동을 결정하기 위해 Think 코루틴 다시 시작
    }

    // 도발 행동 코루틴 (플레이어에게 돌진 후 근접 공격 시도 등)
    IEnumerator Taunt()
    {
        // 도발 목표 위치를 플레이어 위치와 lookVec를 합산하여 설정 (플레이어 근처로 이동)
        tauntVec = Target.position + lookVec;

        isLook = false;         // 플레이어를 바라보지 않도록 설정
        nav.isStopped = false; // NavMeshAgent 이동을 활성화하여 tauntVec으로 이동 시작

        // BoxCollider를 안전하게 비활성화 시도 (도발 중 충돌 방지 또는 특정 효과)
        if (boxCollider != null)
        {
            try { boxCollider.enabled = false; } catch { } // 예외 처리 추가 (안전성 증대)
        }

        anim.SetTrigger("doTaunt"); // "doTaunt" 애니메이션 트리거를 활성화합니다.

        yield return new WaitForSeconds(1.5f); // 1.5초 대기

        if (meleeArea != null) // 근접 공격 영역이 존재하면
            meleeArea.enabled = true; // 근접 공격 영역 활성화 (플레이어에게 데미지 줄 수 있음)

        yield return new WaitForSeconds(0.5f); // 0.5초 대기

        if (meleeArea != null) // 근접 공격 영역이 존재하면
            meleeArea.enabled = false; // 근접 공격 영역 비활성화

        yield return new WaitForSeconds(1f); // 1초 대기

        isLook = true;         // 다시 플레이어를 바라보도록 설정
        nav.isStopped = true;  // NavMeshAgent 이동 정지

        // BoxCollider를 안전하게 다시 활성화 시도
        if (boxCollider != null)
        {
            try { boxCollider.enabled = true; } catch { } // 예외 처리 추가
        }

        StartCoroutine(Think()); // 다음 행동을 결정하기 위해 Think 코루틴 다시 시작
    }

    // 이 오브젝트가 파괴될 때 호출됩니다.
    void OnDestroy()
    {
        StopAllCoroutines(); // 오브젝트 파괴 시 실행 중인 모든 코루틴을 중지합니다.
    }
}