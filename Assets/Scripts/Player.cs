using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons; // 플레이어가 가질 수 있는 무기 오브젝트 배열
    public bool[] hasWeapons; // 플레이어가 특정 무기를 가지고 있는지 여부
    public GameObject[] grenades; // 플레이어가 가진 수류탄 오브젝트 배열
    public int hasGrenades; // 현재 가진 수류탄 개수
    public GameObject grenadeObj; // 수류탄 프리팹
    public Camera followCamera; // 따라다니는 카메라
    public GameManager manager; // GameManager 참조


    public AudioSource jumpSound; // 점프 사운드
    public AudioSource dodgeSound; // 회피 사운드
    public AudioSource interactionSound; // 상호작용 사운드
    public AudioSource swapSound; // 무기 교체 사운드

    public int level = 1; // 플레이어 레벨
    public int currentExperience = 0; // 현재 경험치
    public int maxExperience = 100; // 레벨업에 필요한 경험치 기본값

    public int ammo; // 현재 총알 수
    public int coin; // 현재 코인 수
    public int health; // 현재 체력
    public int score; // 현재 점수

    public int maxAmmo; // 최대 총알 수
    public int maxCoin; // 최대 코인 수
    public int maxHealth; // 최대 체력
    public int maxHasGrenades; // 최대 수류탄 개수

    float hAxis; // 수평 입력
    float vAxis; // 수직 입력

    bool wDown; // 걷기 키 입력
    bool jDown; // 점프 키 입력
    bool fDown; // 공격 키 입력
    bool gDown; // 수류탄 키 입력
    bool rDown; // 재장전 키 입력
    bool iDown; // 상호작용 키 입력
    bool sDown1; // 무기 1 선택 키 입력
    bool sDown2; // 무기 2 선택 키 입력
    bool sDown3; // 무기 3 선택 키 입력
    bool escDown; // 일시정지 키 입력

    bool isJump; // 점프 중인지 여부
    bool isDodge; // 회피 중인지 여부
    bool isSwap; // 무기 교체 중인지 여부
    bool isReload; // 재장전 중인지 여부
    bool isFireReady = true; // 공격 준비 완료 여부
    bool isBorder; // 벽에 닿았는지 여부
    bool isDamage; // 데미지 받는 중인지 여부
    bool isShop; // 상점 이용 중인지 여부
    bool isDead = false; // 사망 여부
    bool isPause = false; // 일시정지 여부

    Vector3 moveVec; // 이동 벡터
    Vector3 dodgeVec; // 회피 벡터

    Rigidbody rigid; // Rigidbody 컴포넌트
    Animator anim; // Animator 컴포넌트
    MeshRenderer[] meshs; // MeshRenderer 컴포넌트 배열

    GameObject nearObject; // 근처 오브젝트
    public Weapon equipWeapon; // 장착된 무기
    int equipWeaponIndex = -1; // 장착된 무기 인덱스
    float fireDelay; // 공격 딜레이

    public Weapon equippedWeapon; // 현재 장착된 무기 (Weapon 스크립트 참조)

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        anim = GetComponentInChildren<Animator>(); // 자식 오브젝트에서 Animator 컴포넌트 가져오기
        meshs = GetComponentsInChildren<MeshRenderer>(); // 자식 오브젝트에서 MeshRenderer 컴포넌트 배열 가져오기
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; // 회전 고정
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green); // 벽 감지 
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall")); // 벽 감지
    }
    private void FixedUpdate()
    {
        FreezeRotation(); // 물리 업데이트에서 회전 고정
        StopToWall(); // 물리 업데이트에서 벽 감지
    }
    void Update()
    {
        GetInput(); // 입력 가져오기
        Move(); // 이동
        Turn(); // 회전
        Jump(); // 점프
        Grenade(); // 수류탄 사용
        Attack(); // 공격
        Reload(); // 재장전
        Dodge(); // 회피
        Swap(); // 무기 교체
        Interation(); // 상호작용
        Pause(); // 일시정지
        if (Input.GetButtonDown("Fire1") && equippedWeapon != null) // 공격 버튼 누르고 무기가 장착되어 있으면
        {
            equippedWeapon.Use(); // 무기 사용
        }
    }
    // 경험치를 추가하는 함수
    public void AddExperience(int exp)
    {
        currentExperience += exp;

        while (currentExperience >= maxExperience)
        {
            currentExperience -= maxExperience;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExperience = 0;
        maxExperience = level * 100;

        // 게임 일시 중지 및 레벨업 옵션 표시
        Time.timeScale = 0f; // 게임 일시 중지
        manager.ShowLevelUpPanel(); // GameManager를 호출하여 레벨업 UI 표시
    }

    public void ApplyUpgrade(int selection)
    {
        switch (selection)
        {
            case 0: // 망치 (Melee Weapon)
                // 현재 망치를 장착하고 있는 경우
                if (equippedWeapon != null && equippedWeapon.type == Weapon.Type.Melee)
                {
                    equippedWeapon.damage += 10;
                    Debug.Log("Equipped Hammer damage increased to: " + equippedWeapon.damage);
                }
                else 
                {
                    GameObject hammerPrefab = weapons[0];
                    if (hammerPrefab != null)
                    {
                        Weapon hammerWeapon = hammerPrefab.GetComponent<Weapon>();
                        if (hammerWeapon != null)
                        {
                            hammerWeapon.damage += 10;
                            Debug.Log("Hammer prefab damage increased to: " + hammerWeapon.damage);
                        }
                    }
                }
                break;

            case 1: // 핸드건 (Range Weapon)
                // 현재 핸드건을 장착하고 있는 경우
                if (equippedWeapon != null && equippedWeapon.type == Weapon.Type.Range && equippedWeapon.name.Contains("HandGun"))
                {
                    Bullet bulletComponent = equippedWeapon.bullet.GetComponent<Bullet>();
                    if (bulletComponent != null)
                    {
                        bulletComponent.damage += 10;
                        Debug.Log("Equipped Hand Gun bullet damage increased to: " + bulletComponent.damage);
                    }
                }
                else
                {
                    // weapons[1]이 핸드건 프리팹이라고 가정
                    GameObject handGunPrefab = weapons[1];
                    if (handGunPrefab != null)
                    {
                        Weapon handGunWeapon = handGunPrefab.GetComponent<Weapon>();
                        if (handGunWeapon != null && handGunWeapon.bullet != null)
                        {
                            Bullet handGunBullet = handGunWeapon.bullet.GetComponent<Bullet>();
                            if (handGunBullet != null)
                            {
                                handGunBullet.damage += 10;
                                Debug.Log("Hand Gun prefab bullet damage increased to: " + handGunBullet.damage);
                            }
                        }
                    }
                }
                break;

            case 2: // 머신건 (Range Weapon)
                // 현재 머신건을 장착하고 있는 경우
                if (equippedWeapon != null && equippedWeapon.type == Weapon.Type.Range && equippedWeapon.name.Contains("MachineGun"))
                {
                    Bullet bulletComponent = equippedWeapon.bullet.GetComponent<Bullet>();
                    if (bulletComponent != null)
                    {
                        bulletComponent.damage += 3;
                        Debug.Log("Equipped Machine Gun bullet damage increased to: " + bulletComponent.damage);
                    }
                }
                else
                {
                    // weapons[2]이 머신건 프리팹이라고 가정
                    GameObject machineGunPrefab = weapons[2];
                    if (machineGunPrefab != null)
                    {
                        Weapon machineGunWeapon = machineGunPrefab.GetComponent<Weapon>();
                        if (machineGunWeapon != null && machineGunWeapon.bullet != null)
                        {
                            Bullet machineGunBullet = machineGunWeapon.bullet.GetComponent<Bullet>();
                            if (machineGunBullet != null)
                            {
                                machineGunBullet.damage += 3;
                                Debug.Log("Machine Gun prefab bullet damage increased to: " + machineGunBullet.damage);
                            }
                        }
                    }
                }
                break;
        }
    }

    public void Save()
    {
        isPause = false; // 일시정지 해제
        PlayerPrefs.SetInt("HasGrenades", hasGrenades); // 수류탄 개수 저장
        PlayerPrefs.SetInt("Level", level); // 레벨 저장
        PlayerPrefs.SetInt("CurrentExperience", currentExperience); // 현재 경험치 저장
        PlayerPrefs.SetInt("MaxExperience", maxExperience); // 최대 경험치 저장
        PlayerPrefs.SetInt("Ammo", ammo); // 총알 저장
        PlayerPrefs.SetInt("Coin", coin); // 코인 저장
        PlayerPrefs.SetInt("Health", health); // 체력 저장
        PlayerPrefs.SetInt("MaxHealth", maxHealth); // 최대 체력 저장
        PlayerPrefs.SetInt("Score", score); // 점수 저장
        SaveWeapons(hasWeapons); // 무기 보유 여부 저장
    }

    public void SaveWeapons(bool[] hasWeapons)
    {
        string weaponData = ""; // 무기 데이터를 문자열로 변환
        foreach (bool has in hasWeapons)
        {
            weaponData += has ? "1" : "0"; // true면 '1', false면 '0' 추가
        }
        PlayerPrefs.SetString("HasWeapons", weaponData); // 무기 데이터 저장
        PlayerPrefs.Save(); // 저장
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // 수평 입력 가져오기
        vAxis = Input.GetAxisRaw("Vertical"); // 수직 입력 가져오기
        wDown = Input.GetButton("Walk"); // 걷기 버튼 입력 가져오기
        jDown = Input.GetButtonDown("Jump"); // 점프 버튼 입력 가져오기
        fDown = Input.GetButton("Fire1"); // 공격 버튼 입력 가져오기
        gDown = Input.GetButton("Fire2"); // 수류탄 버튼 입력 가져오기
        rDown = Input.GetButtonDown("Reload"); // 재장전 버튼 입력 가져오기
        iDown = Input.GetButtonDown("Interation"); // 상호작용 버튼 입력 가져오기
        sDown1 = Input.GetButtonDown("Swap1"); // 무기 1 교체 버튼 입력 가져오기
        sDown2 = Input.GetButtonDown("Swap2"); // 무기 2 교체 버튼 입력 가져오기
        sDown3 = Input.GetButtonDown("Swap3"); // 무기 3 교체 버튼 입력 가져오기
        escDown = Input.GetButtonDown("Pause"); // 일시정지 버튼 입력 가져오기
    }

    void Pause()
    {
        if (!escDown || manager.isBattle) // 일시정지 키가 눌리지 않았거나 전투 중이면 리턴
            return;
        if (isPause) // 일시정지 중이면
        {
            isPause = false; // 일시정지 해제
            manager.pausePanel.SetActive(false); // 일시정지 패널 비활성화
        }
        else if (!isPause) // 일시정지 중이 아니면
        {
            isPause = true; // 일시정지 설정
            manager.pausePanel.SetActive(true); // 일시정지 패널 활성화
        }
    }


    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 이동 벡터 계산

        if (isDodge) // 회피 중이면
            moveVec = dodgeVec; // 회피 벡터로 이동

        if (isSwap || !isFireReady || isReload || isDead || isPause) // 무기 교체, 공격 불가, 재장전, 사망, 일시정지 중이면
            moveVec = Vector3.zero; // 이동 정지

        if (!isBorder) // 벽에 닿지 않았으면
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; // 이동

        anim.SetBool("isRun", moveVec != Vector3.zero); // 이동 중이면 isRun 애니메이션 파라미터 true
        anim.SetBool("isWalk", wDown); // 걷기 중이면 isWalk 애니메이션 파라미터 true
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 이동 방향으로 플레이어 회전

        if (fDown && !isDead && !isPause) // 공격 버튼 누르고 사망, 일시정지 중이 아니면
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 레이 생성
            RaycastHit rayHit; // 레이캐스트 결과
            if (Physics.Raycast(ray, out rayHit, 100)) // 레이캐스트 성공 시
            {
                Vector3 nextVec = rayHit.point - transform.position; // 마우스 위치와 플레이어 위치의 차이 계산
                nextVec.y = 0; // Y축 고정
                transform.LookAt(transform.position + nextVec); // 마우스 방향으로 플레이어 회전
            }
        }

    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead && !isPause) // 점프 키 누르고, 이동 중이 아니고, 점프/회피/무기교체/사망/일시정지 중이 아니면
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); // 위로 힘 가하기
            anim.SetBool("isJump", true); // isJump 애니메이션 파라미터 true
            anim.SetTrigger("doJump"); // doJump 애니메이션 트리거
            isJump = true; // 점프 중 설정

            jumpSound.Play(); // 점프 사운드 재생
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0) // 수류탄이 없으면 리턴
            return;

        if (gDown && !isReload && !isSwap && !isDead && !isPause) // 수류탄 키 누르고 재장전/무기교체/사망/일시정지 중이 아니면
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 레이 생성
            RaycastHit rayHit; // 레이캐스트 결과
            if (Physics.Raycast(ray, out rayHit, 100)) // 레이캐스트 성공 시
            {
                Vector3 nextVec = rayHit.point - transform.position; // 마우스 위치와 플레이어 위치의 차이 계산
                nextVec.y = 10; // Y축 높이 설정

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation); // 수류탄 생성
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>(); // 수류탄 Rigidbody 가져오기
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse); // 수류탄에 힘 가하기
                rigidGrenade.freezeRotation = true; // 회전 고정
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse); // 수류탄 회전

                hasGrenades--; // 수류탄 개수 감소
                grenades[hasGrenades].SetActive(false); // 수류탄 UI 비활성화

            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null) // 장착된 무기가 없으면 리턴
            return;
        fireDelay += Time.deltaTime; // 공격 딜레이 증가
        isFireReady = equipWeapon.rate < fireDelay; // 공격 준비 완료 여부 판단

        if (fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead && !isPause) // 공격 버튼 누르고 공격 준비 완료, 회피/무기교체/상점/사망/일시정지 중이 아니면
        {
            equipWeapon.Use(); // 무기 사용
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); // 무기 타입에 따라 애니메이션 트리거
            fireDelay = 0; // 공격 딜레이 초기화
        }
    }

    void Reload()
    {
        if (equipWeapon == null) return; // 장착된 무기가 없으면 리턴

        if (equipWeapon.type == Weapon.Type.Melee) return; // 근접 무기면 리턴

        if (equipWeapon.curAmmo == equipWeapon.maxAmmo) return; // 현재 총알이 최대 총알과 같으면 리턴

        if (ammo == 0) return; // 총알이 없으면 리턴

        if (isReload) return; // 재장전 중이면 리턴

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead && !isPause) // 재장전 키 누르고 점프/회피/무기교체/공격 준비 완료, 상점/사망/일시정지 중이 아니면
        {
            anim.SetTrigger("doReload"); // doReload 애니메이션 트리거
            isReload = true; // 재장전 중 설정
            Debug.Log(isReload); // 재장전 중 여부 로그 출력
            Invoke("ReloadOut", 2f); // 2초 후 ReloadOut 함수 호출
        }
    }

    void ReloadOut()
    {
        int reAmmo = equipWeapon.maxAmmo; // 재장전할 총알 양
        int needAmmo = reAmmo - equipWeapon.curAmmo; // 필요한 총알 양
        if (needAmmo > ammo) // 필요한 총알이 가진 총알보다 많으면
        {
            equipWeapon.curAmmo += ammo; // 가진 총알 모두 장전
            ammo = 0; // 가진 총알 0으로 설정
        }
        else // 필요한 총알이 가진 총알보다 적거나 같으면
        {
            equipWeapon.curAmmo = equipWeapon.maxAmmo; // 최대 총알만큼 장전
            ammo -= needAmmo; // 필요한 총알만큼 감소
        }
        isReload = false; // 재장전 중 해제
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead && !isPause) // 점프 키 누르고, 이동 중이고, 점프/회피/무기교체/상점/사망/일시정지 중이 아니면
        {
            dodgeVec = moveVec; // 회피 벡터 설정
            speed *= 2; // 속도 2배 증가
            anim.SetTrigger("doDodge"); // doDodge 애니메이션 트리거
            isDodge = true; // 회피 중 설정

            dodgeSound.Play(); // 회피 사운드 재생

            Invoke("DodgeOut", 0.6f); // 0.6초 후 DodgeOut 함수 호출
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f; // 속도 0.5배 감소 (원래 속도로)
        isDodge = false; // 회피 중 해제
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) // 무기 1 선택 키 누르고 무기 1 없거나 이미 장착 중이면 리턴
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) // 무기 2 선택 키 누르고 무기 2 없거나 이미 장착 중이면 리턴
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) // 무기 3 선택 키 누르고 무기 3 없거나 이미 장착 중이면 리턴
            return;

        int weaponIndex = -1; // 선택된 무기 인덱스
        if (sDown1) weaponIndex = 0; // 무기 1 선택
        if (sDown2) weaponIndex = 1; // 무기 2 선택
        if (sDown3) weaponIndex = 2; // 무기 3 선택

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead && !isPause) // 무기 선택 키 누르고 점프/회피/상점/사망/일시정지 중이 아니면
        {
            if (equipWeapon != null) // 장착된 무기가 있으면
                equipWeapon.gameObject.SetActive(false); // 비활성화

            equipWeaponIndex = weaponIndex; // 장착된 무기 인덱스 설정
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>(); // 새로운 무기 장착
            equipWeapon.gameObject.SetActive(true); // 활성화

            anim.SetTrigger("doSwap"); // doSwap 애니메이션 트리거

            isSwap = true; // 무기 교체 중 설정

            Invoke("SwapOut", 0.4f); // 0.4초 후 SwapOut 함수 호출
            swapSound.Play(); // 무기 교체 사운드 재생
        }

    }

    void SwapOut()
    {
        isSwap = false; // 무기 교체 중 해제
    }

    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !isDead && !isPause) // 상호작용 키 누르고 근처 오브젝트 있고 점프/회피/사망/일시정지 중이 아니면
        {
            if (nearObject.tag == "Weapon") // 근처 오브젝트가 무기이면
            {
                Item item = nearObject.GetComponent<Item>(); // Item 컴포넌트 가져오기
                int weaponIndex = item.value; // 무기 인덱스 가져오기
                hasWeapons[weaponIndex] = true; // 무기 보유 설정

                Destroy(nearObject); // 오브젝트 파괴
            }
            else if (nearObject.tag == "shop") // 근처 오브젝트가 상점이면
            {
                Shop shop = nearObject.GetComponent<Shop>(); // Shop 컴포넌트 가져오기
                shop.Enter(this); // 상점 열기
                isShop = true; // 상점 이용 중 설정
            }
            interactionSound.Play(); // 상호작용 사운드 재생
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") // 바닥과 충돌하면
        {
            anim.SetBool("isJump", false); // isJump 애니메이션 파라미터 false
            isJump = false; // 점프 중 해제
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item") // 아이템과 충돌하면
        {
            Item item = other.GetComponent<Item>(); // Item 컴포넌트 가져오기
            switch (item.type) // 아이템 타입에 따라
            {
                case Item.Type.Ammo: // 총알 아이템
                    ammo += item.value; // 총알 증가
                    if (ammo > maxAmmo) // 최대 총알보다 많으면
                        ammo = maxAmmo; // 최대 총알로 설정
                    break;
                case Item.Type.Coin: // 코인 아이템
                    coin += item.value; // 코인 증가
                    if (coin > maxCoin) // 최대 코인보다 많으면
                        coin = maxCoin; // 최대 코인으로 설정
                    break;
                case Item.Type.Heart: // 체력 아이템
                    health += item.value; // 체력 증가
                    if (health > maxHealth) // 최대 체력보다 많으면
                        health = maxHealth; // 최대 체력으로 설정
                    break;
                case Item.Type.Grenade: // 수류탄 아이템
                    grenades[hasGrenades].SetActive(true); // 수류탄 UI 활성화
                    hasGrenades += item.value; // 수류탄 개수 증가
                    if (hasGrenades > maxHasGrenades) // 최대 수류탄보다 많으면
                        hasGrenades = maxHasGrenades; // 최대 수류탄으로 설정
                    break;
            }
            Destroy(other.gameObject); // 아이템 파괴
        }
        else if (other.tag == "EnemyBullet") // 적 총알과 충돌하면
        {
            if (!isDamage) // 데미지 받는 중이 아니면
            {
                Bullet enemyBullet = other.GetComponent<Bullet>(); // Bullet 컴포넌트 가져오기
                health -= enemyBullet.damage; // 체력 감소

                bool isBossAtk = other.name == "Boss Melee Area"; // 보스 공격 여부
                StartCoroutine(OnDamage(isBossAtk)); // OnDamage 코루틴 시작
            }
            if (other.GetComponent<Rigidbody>() != null) // Rigidbody 컴포넌트가 있으면
                Destroy(other.gameObject);  // 총알 파괴

        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true; // 데미지 받는 중 설정

        foreach (MeshRenderer mesh in meshs) // 모든 MeshRenderer에 대해
        {
            mesh.material.color = Color.red; // 색상 빨간색으로 변경
            Debug.Log("���� ���� �õ�: " + mesh.gameObject.name); // 로그 출력
        }
        if (isBossAtk) // 보스 공격이면
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse); // 뒤로 밀리는 힘 가하기
        }

        if (health <= 0 && !isDead) // 체력이 0 이하이고 사망 상태가 아니면
            OnDie(); // 사망 처리

        yield return new WaitForSeconds(1f); // 1초 대기

        isDamage = false; // 데미지 받는 중 해제

        foreach (MeshRenderer mesh in meshs) // 모든 MeshRenderer에 대해
        {
            mesh.material.color = Color.white; // 색상 흰색으로 변경
            Debug.Log("���� ����: " + mesh.gameObject.name); // 로그 출력
        }

        if (isBossAtk) // 보스 공격이면
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // 속도 0으로 설정
        }
    }

    void OnDie()
    {
        anim.SetTrigger("doDie"); // doDie 애니메이션 트리거
        isDead = true; // 사망 상태 설정
        manager.GameOver(); // 게임 오버 처리
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "shop") // 무기 또는 상점 태그와 충돌하면
            nearObject = other.gameObject; // 근처 오브젝트 설정
        if (nearObject != null) // 근처 오브젝트가 있으면
            Debug.Log(nearObject.name); // 오브젝트 이름 로그 출력

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon") // 무기 태그와 충돌 종료 시
            nearObject = null; // 근처 오브젝트 해제
        else if (other.tag == "shop") // 상점 태그와 충돌 종료 시
        {
            Shop shop = nearObject.GetComponent<Shop>(); // Shop 컴포넌트 가져오기
            shop.Exit(); // 상점 닫기
            isShop = false; // 상점 이용 중 해제
            nearObject = null; // 근처 오브젝트 해제
        }
    }
    public void EquipWeapon(Weapon newWeapon)
    {
        equippedWeapon = newWeapon; // 무기 장착
    }

    public void TakeDamage(int amount)
    {
        health -= amount; // 체력 감소
        if (health <= 0) // 체력이 0 이하면
        {
            OnDie(); // 사망 처리
        }
    }
}