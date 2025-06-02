using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Enemy 클래스는 게임 내 모든 적 캐릭터의 기본적인 동작과 상태를 관리합니다.
public class Enemy : MonoBehaviour
{
    // 적의 타입을 정의하는 열거형 (Enum). A, B, C는 일반 적, D는 보스 적을 나타낼 수 있습니다.
    public enum Type { A, B, C, D };
    public Type enemyType; // 현재 적의 타입

    // 적의 체력 및 게임 관련 스탯
    public int maxHealth;  // 최대 체력
    public int curHealth;  // 현재 체력
    public int score;      // 적 처치 시 획득할 점수

    // 다른 스크립트 또는 오브젝트에 대한 참조
    public GameManager manager; // 게임 관리자 스크립트 (적 처치 수 등을 업데이트)
    public Transform Target;    // 플레이어(타겟)의 Transform (추적 대상)
    public BoxCollider meleeArea; // 근접 공격 영역 (근접 공격 시 활성화)
    public GameObject bullet;     // 원거리 공격 시 발사할 총알 프리팹
    public GameObject[] coins;    // 적 처치 시 드롭할 코인 프리팹 배열
    public GameObject poisonGas;  // 적 처치 시 생성될 독가스 프리팹 (Type.A 적)

    // 사운드 관련 변수
    public AudioClip attackSound;     // 적이 공격할 때 나는 소리
    public AudioClip hitSound;        // 적이 맞을 때 나는 소리
    public AudioClip deathSound;      // 적이 죽을 때 나는 소리
    private AudioSource audioSource;  // 사운드 재생을 위한 AudioSource 컴포넌트

    // 적의 현재 상태를 나타내는 플래그
    public bool isChase;  // 플레이어를 추적 중인지 여부
    public bool isAttack; // 공격 중인지 여부
    public bool isDead;   // 죽었는지 여부

    // 컴포넌트 참조
    public Rigidbody rigid;       // 물리 제어를 위한 Rigidbody 컴포넌트
    public BoxCollider boxCollider; // 충돌 감지를 위한 BoxCollider 컴포넌트
    public MeshRenderer[] meshs;   // 적의 메쉬 렌더러 배열 (피격 시 색상 변경 등)
    public NavMeshAgent nav;       // 길 찾기 및 이동을 위한 NavMeshAgent 컴포넌트
    public Animator anim;         // 애니메이션 제어를 위한 Animator 컴포넌트

    public int experiencePoints; // 적 처치 시 플레이어가 획득할 경험치 

    // 오브젝트가 생성될 때 가장 먼저 호출되는 초기화 메서드
    void Awake()
    {
        // 필요한 컴포넌트들을 가져와 변수에 할당합니다.
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>(); // 자식 오브젝트의 메쉬 렌더러 포함
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>(); // 자식 오브젝트의 애니메이터 포함

        // Type.D (보스)가 아닌 경우 2초 후에 ChaseStart 메서드를 호출하여 추적을 시작합니다.
        if (enemyType != Type.D)
            Invoke("ChaseStart", 2);

        // AudioSource 컴포넌트를 가져오거나 없으면 추가합니다.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // 추적을 시작하는 메서드
    void ChaseStart()
    {
        isChase = true;                 // 추적 상태를 true로 설정
        anim.SetBool("isWalk", true);   // 걷기 애니메이션 재생
    }

    // 매 프레임 업데이트되는 메서드
    void Update()
    {
        // NavMeshAgent가 활성화되어 있고 Type.D (보스)가 아닌 경우 플레이어를 추적합니다.
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(Target.position); // 플레이어의 위치를 목적지로 설정
            nav.isStopped = !isChase;            // isChase 상태에 따라 이동을 멈추거나 계속합니다.
        }
    }

    // 물리 업데이트에서 호출되어 속도를 고정하는 메서드
    void FreezeVelocity()
    {
        // 추적 중일 때만 선형 및 각속도를 0으로 설정하여 미끄러짐 방지
        if (isChase)
        {
            rigid.linearVelocity = Vector3.zero;  // 선형 속도 (이동) 고정
            rigid.angularVelocity = Vector3.zero; // 각속도 (회전) 고정
        }
    }

    // 타겟을 감지하고 공격을 시작할지 결정하는 메서드
    void Targerting()
    {
        // 죽지 않았고 Type.D (보스)가 아닌 경우에만 실행
        if (!isDead && enemyType != Type.D)
        {
            float targetRadious = 1.5f; // 스피어캐스트의 반지름
            float targetRange = 3f;     // 스피어캐스트의 사거리

            // 적 타입에 따라 감지 범위와 공격 사거리 설정
            switch (enemyType)
            {
                case Type.A: // 근접형 적
                    targetRadious = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B: // 돌진형 적
                    targetRadious = 1f;
                    targetRange = 12f;
                    break;
                case Type.C: // 원거리형 적
                    targetRadious = 0.5f;
                    targetRange = 25f;
                    break;
            }

            // 플레이어 레이어에 있는 오브젝트를 스피어캐스트로 감지
            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,     // 시작 위치
                targetRadious,                                // 반지름
                transform.forward,                            // 발사 방향 (적 앞쪽)
                targetRange,                                  // 사거리
                LayerMask.GetMask("Player"));                 // "Player" 레이어만 감지

            // 플레이어가 감지되었고 공격 중이 아니라면 공격 코루틴 시작
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    // 공격 코루틴
    IEnumerator Attack()
    {
        isChase = false;                    // 공격 중에는 추적 중지
        isAttack = true;                    // 공격 상태로 설정
        anim.SetBool("isAttack", true);     // 공격 애니메이션 재생

        // 공격 사운드가 있으면 재생
        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);

        // 적 타입에 따른 공격 방식
        switch (enemyType)
        {
            case Type.A: // 근접 공격
                yield return new WaitForSeconds(0.2f); // 짧게 대기 후
                meleeArea.enabled = true;              // 근접 공격 영역 활성화 (대미지 판정)

                yield return new WaitForSeconds(1f);   // 1초 대기 후
                meleeArea.enabled = false;             // 근접 공격 영역 비활성화

                yield return new WaitForSeconds(1f);   // 다음 공격까지 대기
                break;

            case Type.B: // 돌진 공격
                yield return new WaitForSeconds(0.1f);                                  // 짧게 대기 후
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);              // 앞으로 강한 힘을 가하여 돌진
                meleeArea.enabled = true;                                               // 근접 공격 영역 활성화

                yield return new WaitForSeconds(0.5f);                                  // 0.5초 대기 후
                rigid.linearVelocity = Vector3.zero;                                    // 돌진 후 속도 고정
                meleeArea.enabled = false;                                              // 근접 공격 영역 비활성화

                yield return new WaitForSeconds(2f);                                    // 다음 공격까지 대기
                break;

            case Type.C: // 원거리 공격
                yield return new WaitForSeconds(0.5f);                                                              // 0.5초 대기 후
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);           // 총알 프리팹 생성
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();                                  // 생성된 총알의 Rigidbody 가져오기
                rigidBullet.linearVelocity = transform.forward * 20;                                            // 총알을 적 앞쪽으로 발사

                yield return new WaitForSeconds(2f);                                                                // 다음 공격까지 대기
                break;
        }

        isChase = true;                     // 공격 완료 후 다시 추적 시작
        isAttack = false;                   // 공격 상태 해제
        anim.SetBool("isAttack", false);    // 공격 애니메이션 비활성화
    }

    // 일정한 물리 업데이트 간격으로 호출되는 메서드
    void FixedUpdate()
    {
        Targerting();     // 타겟 감지 및 공격 로직 호출
        FreezeVelocity(); // 속도 고정 로직 호출
    }

    // 다른 Collider와 충돌했을 때 호출되는 메서드 (isTrigger가 true인 Collider)
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 "Melee" (근접 공격)인 경우
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>(); // 충돌한 오브젝트에서 Weapon 컴포넌트 가져오기
            curHealth -= weapon.damage;                   // 무기의 대미지만큼 체력 감소
            Vector3 reactVec = transform.position - other.transform.position; // 피격 반발력 방향 계산 (적이 맞은 위치에서 공격자 위치를 뺀 벡터)

            StartCoroutine(OnDamage(reactVec, false)); // 피격 처리 코루틴 시작 (수류탄 아님)
        }
        // 충돌한 오브젝트의 태그가 "Bullet" (총알)인 경우
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>(); // 충돌한 오브젝트에서 Bullet 컴포넌트 가져오기
            curHealth -= bullet.damage;                   // 총알의 대미지만큼 체력 감소
            Vector3 reactVec = transform.position - other.transform.position; // 피격 반발력 방향 계산
            Destroy(other.gameObject);                    // 총알 오브젝트 파괴

            StartCoroutine(OnDamage(reactVec, false)); // 피격 처리 코루틴 시작 (수류탄 아님)
        }
    }

    // 수류탄에 의해 피격되었을 때 호출되는 public 메서드
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100; // 수류탄 대미지만큼 체력 감소 (고정 값 100)
        Vector3 reactVec = transform.position - explosionPos; // 피격 반발력 방향 계산 (적이 맞은 위치에서 폭발 위치를 뺀 벡터)
        StartCoroutine(OnDamage(reactVec, true)); // 피격 처리 코루틴 시작 (수류탄 맞음)
    }

    // 피격 처리 및 사망 로직을 담당하는 코루틴
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        // 이미 죽은 적이라면 피해 처리를 무시하고 코루틴 종료
        if (isDead)
            yield break;

        // 모든 메쉬 렌더러의 색상을 빨간색으로 변경하여 피격 효과 표현
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f); // 0.1초 대기

        // 현재 체력이 0보다 크면 (아직 살아있다면)
        if (curHealth > 0)
        {
            yield return new WaitForSeconds(0.1f); // 0.1초 대기

            // 다시 한번 죽었는지 확인 (짧은 시간 내에 여러 번 피격될 수 있으므로)
            if (isDead)
                yield break;
            // 모든 메쉬 렌더러의 색상을 흰색으로 되돌려 피격 효과 종료
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        // 현재 체력이 0 이하면 (죽었다면)
        else
        {
            isDead = true; // 죽음 상태로 설정

            // 모든 메쉬 렌더러의 색상을 회색으로 변경하여 사망 효과 표현
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14; // 레이어를 "DeadEnemy" 등으로 변경하여 더 이상 충돌 및 감지되지 않도록 합니다.

            isChase = false;         // 추적 중지
            nav.enabled = false;     // NavMeshAgent 비활성화 (이동 중지)
            anim.SetTrigger("doDie"); // 사망 애니메이션 트리거

            Player player = Target.GetComponent<Player>(); // 플레이어 스크립트 참조 가져오기

            // 플레이어 스크립트가 있다면 경험치 및 점수 처리
            if (player != null)
            {
                // 적의 타입에 따라 획득 경험치를 다르게 설정
                int exp = 0;
                switch (enemyType)
                {
                    case Type.A:
                        exp = 10;
                        break;
                    case Type.B:
                        exp = 20;
                        break;
                    case Type.C:
                        exp = 40;
                        break;
                    case Type.D: // 보스 적
                        exp = 100;
                        break;
                }
                player.AddExperience(exp); // 플레이어에게 경험치 추가
            }

            player.score += score; // 플레이어의 점수에 이 적의 점수 추가

            // 0에서 2 사이의 무작위 코인 중 하나를 적의 위치에 생성하여 드롭
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            // 적 타입에 따라 게임 관리자의 적 카운트를 감소시키고 추가 효과 (예: 독가스) 생성
            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    Instantiate(poisonGas, transform.position, Quaternion.identity); // 독가스 생성
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;
            }

            // 수류탄에 맞아서 죽은 경우 더 강한 물리적 반발력 적용
            if (isGrenade)
            {
                reactVec = reactVec.normalized; // 반발력 벡터 정규화
                reactVec += Vector3.up * 3;     // 위쪽으로 추가적인 힘 적용 (높이 띄우기)

                rigid.freezeRotation = false;                 // 회전 고정 해제
                rigid.AddForce(reactVec * 5, ForceMode.Impulse); // 강력한 힘으로 튕겨나가게 함
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse); // 회전력도 추가
            }
            // 일반 공격에 맞아서 죽은 경우
            else
            {
                reactVec = reactVec.normalized; // 반발력 벡터 정규화
                reactVec += Vector3.up;         // 위쪽으로 약간의 힘 적용

                rigid.AddForce(reactVec * 5, ForceMode.Impulse); // 힘으로 튕겨나가게 함
            }

            Destroy(gameObject, 2f); // 2초 후 적 오브젝트 파괴
        }
    }
}