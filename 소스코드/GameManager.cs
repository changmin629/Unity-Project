using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using UnityEngine.SceneManagement; // 씬 관리를 위한 네임스페이스

// GameManager 클래스는 게임의 전반적인 흐름, UI 업데이트, 스테이지 관리 등을 담당합니다.
public class GameManager : MonoBehaviour
{
    // 카메라 및 주요 게임 오브젝트 참조
    public GameObject menuCam;    // 메뉴 화면 카메라
    public GameObject gameCam;    // 게임 플레이 카메라
    public Player player;         // 플레이어 스크립트 참조
    public Boss boss;             // 보스 스크립트 참조 (보스 스테이지에서 사용)
    public GameObject itemShop;   // 아이템 상점 오브젝트
    public GameObject weaponShop; // 무기 상점 오브젝트
    public GameObject startZone;  // 스테이지 시작 구역 오브젝트

    // 게임 진행 관련 변수
    public int stage;             // 현재 스테이지 번호
    public float playTime;        // 현재 게임 플레이 시간
    public bool isBattle;         // 현재 전투 중인지 여부

    // 적 카운트 (각 타입별 적의 남은 수)
    public int enemyCntA; // A타입 적의 수
    public int enemyCntB; // B타입 적의 수
    public int enemyCntC; // C타입 적의 수
    public int enemyCntD; // D타입(보스) 적의 수

    // 적 스폰 관련 변수
    public Transform[] enemyZones; // 적이 스폰될 구역들의 Transform 배열
    public GameObject[] enemies;   // 적 프리팹 배열 (A, B, C, D 타입 순서)
    public List<int> enemyList;    // 현재 스테이지에서 스폰될 적들의 타입 인덱스를 저장하는 리스트

    // UI 패널 참조
    public GameObject menuPanel;  // 메인 메뉴 패널
    public GameObject gamePanel;  // 게임 플레이 UI 패널
    public GameObject overPanel;  // 게임 오버 패널
    public GameObject pausePanel; // 일시정지 패널

    // UI 텍스트 및 이미지 참조
    public TextMeshProUGUI maxScoreTxt;     // 최고 점수 표시 텍스트 (메인 메뉴)
    public TextMeshProUGUI scoreTxt;        // 현재 점수 표시 텍스트 (게임 플레이 중)
    public TextMeshProUGUI stageTxt;        // 현재 스테이지 표시 텍스트
    public TextMeshProUGUI playTimeTxt;     // 플레이 시간 표시 텍스트
    public TextMeshProUGUI playerHealthTxt; // 플레이어 체력 표시 텍스트
    public TextMeshProUGUI playerAmmoTxt;   // 플레이어 총알/탄약 표시 텍스트
    public TextMeshProUGUI playerCoinTxt;   // 플레이어 코인 표시 텍스트
    public Image weapon1Img;      // 무기 1 UI 이미지 (활성화 여부 표시)
    public Image weapon2Img;      // 무기 2 UI 이미지
    public Image weapon3Img;      // 무기 3 UI 이미지
    public Image weaponRImg;      // 수류탄 UI 이미지 (보유 여부 표시)
    public TextMeshProUGUI enemyATxt; // A타입 적 남은 수 텍스트
    public TextMeshProUGUI enemyBTxt; // B타입 적 남은 수 텍스트
    public TextMeshProUGUI enemyCTxt; // C타입 적 남은 수 텍스트

    // 보스 체력바 UI
    public RectTransform bossHealthGroup; // 보스 체력바 전체 그룹의 RectTransform
    public RectTransform bossHealthBar;   // 보스 체력바의 RectTransform (실제 체력 변화를 시각화)

    // 게임 오버/일시정지 화면의 점수 텍스트
    public TextMeshProUGUI curScoreText;    // 게임 오버 시 현재 점수
    public TextMeshProUGUI bestText;        // 게임 오버 시 최고 점수 여부 표시
    public TextMeshProUGUI PausebestText;   // 일시정지 시 최고 점수
    public TextMeshProUGUI PauseText;       // 일시정지 시 현재 점수

    // 레벨업 UI 요소
    public GameObject levelUpGroup;        // 레벨업 패널 전체 그룹
    public Button itemUpgradeButtonA;      // 아이템 업그레이드 버튼 A
    public Button itemUpgradeButtonB;      // 아이템 업그레이드 버튼 B
    public Button itemUpgradeButtonC;      // 아이템 업그레이드 버튼 C

    // 경험치 바 UI
    public RectTransform expGroup; // 경험치 바 그룹 (전체)
    public Image expBar;           // 경험치 바 이미지 (실제 채워지는 부분)

    // 게임 오브젝트가 처음 활성화될 때 호출됩니다. (시작 시 한 번)
    void Awake()
    {
        enemyList = new List<int>(); // 적 리스트 초기화

        // "MaxScore" 키가 PlayerPrefs에 없으면 0으로 초기 설정합니다.
        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);

        // 저장된 최고 점수를 불러와 UI에 표시합니다. (천 단위 구분 기호 적용)
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    // 게임 시작 버튼 클릭 시 호출되는 메서드
    public void GameStart()
    {
        Re(); // 게임 데이터 초기화
        menuCam.SetActive(false); // 메뉴 카메라 비활성화
        gameCam.SetActive(true);  // 게임 카메라 활성화

        menuPanel.SetActive(false); // 메뉴 패널 비활성화
        gamePanel.SetActive(true);  // 게임 UI 패널 활성화

        player.gameObject.SetActive(true); // 플레이어 오브젝트 활성화
    }

    // 게임 종료 버튼 클릭 시 호출되는 메서드
    public void GameQuit()
    {
        // 에디터 환경과 빌드된 환경에 따라 다르게 작동합니다.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 플레이 모드 종료
#else
        Application.Quit(); // 빌드된 게임 종료
#endif
    }

    // 게임 이어하기 버튼 클릭 시 호출되는 메서드
    public void GameContinue()
    {
        Load(); // 저장된 게임 데이터 로드
        // "Score" 키가 없으면 (저장된 게임이 없으면) 이어하기를 중단합니다.
        if (!PlayerPrefs.HasKey("Score"))
            return;

        menuCam.SetActive(false); // 메뉴 카메라 비활성화
        gameCam.SetActive(true);  // 게임 카메라 활성화

        menuPanel.SetActive(false); // 메뉴 패널 비활성화
        gamePanel.SetActive(true);  // 게임 UI 패널 활성화

        player.gameObject.SetActive(true); // 플레이어 오브젝트 활성화
    }

    // 게임 데이터를 초기화하는 메서드 (새 게임 시작 시 호출)
    void Re()
    {
        player.gameObject.SetActive(false); // 플레이어 오브젝트 비활성화 (초기화 전)
        int maxScore = PlayerPrefs.GetInt("MaxScore"); // 현재 최고 점수를 임시 저장
        PlayerPrefs.DeleteAll(); // 모든 PlayerPrefs 데이터 삭제 (초기화)
        PlayerPrefs.SetInt("MaxScore", maxScore); // 삭제된 최고 점수를 다시 설정 (유지)

        // 게임 관련 변수 초기화
        stage = 1;
        playTime = 0;
        player.hasGrenades = 0;
        player.level = 1;
        player.currentExperience = 0;
        player.maxExperience = 100;
        player.ammo = 0;
        player.coin = 5500; // 초기 코인 설정
        player.health = 100;
        player.maxHealth = 100;
        player.score = 0;
        player.hasWeapons = new bool[] { false, false, false }; // 모든 무기 미보유로 초기화
        player.hasWeapons = LoadWeapons(3); // 무기 보유 상태 로드 (PlayerPrefs에서)
    }

    // 저장된 게임 데이터를 로드하는 메서드
    void Load()
    {
        // PlayerPrefs에서 각 게임 데이터를 불러와 변수에 할당합니다.
        stage = PlayerPrefs.GetInt("Stage");
        playTime = PlayerPrefs.GetFloat("PlayTime");
        player.hasGrenades = PlayerPrefs.GetInt("HasGrenades");
        player.level = PlayerPrefs.GetInt("Level");
        player.currentExperience = PlayerPrefs.GetInt("CurrentExperience");
        player.maxExperience = PlayerPrefs.GetInt("MaxExperience");
        player.ammo = PlayerPrefs.GetInt("Ammo");
        player.coin = PlayerPrefs.GetInt("Coin");
        player.health = PlayerPrefs.GetInt("Health");
        player.maxHealth = PlayerPrefs.GetInt("MaxHealth");
        player.score = PlayerPrefs.GetInt("Score");
        player.hasWeapons = LoadWeapons(3); // 무기 보유 상태 로드
    }

    // 무기 보유 상태를 문자열로 저장된 PlayerPrefs에서 불러와 bool 배열로 변환하는 메서드
    public bool[] LoadWeapons(int weaponCount)
    {
        // "HasWeapons" 키로 저장된 문자열을 불러옵니다. 없으면 '0'으로 채워진 문자열을 기본값으로 사용합니다.
        string weaponData = PlayerPrefs.GetString("HasWeapons", new string('0', weaponCount));
        bool[] hasWeapons = new bool[weaponCount]; // 무기 개수만큼 bool 배열 생성

        // 문자열을 순회하며 '1'이면 true, '0'이면 false로 배열에 저장합니다.
        for (int i = 0; i < weaponCount && i < weaponData.Length; i++)
        {
            hasWeapons[i] = weaponData[i] == '1';
        }
        return hasWeapons; // 완성된 bool 배열 반환
    }

    // 레벨업 패널을 보여주는 메서드
    public void ShowLevelUpPanel()
    {
        levelUpGroup.SetActive(true); // 레벨업 패널 활성화

        RectTransform levelUpRect = levelUpGroup.GetComponent<RectTransform>();
        if (levelUpRect != null)
        {
            levelUpRect.anchoredPosition = Vector3.zero; // 레벨업 패널을 화면 중앙으로 이동
        }

        // 업그레이드 버튼에 클릭 리스너 할당 (기존 리스너는 제거하여 중복 방지)
        itemUpgradeButtonA.onClick.RemoveAllListeners();
        itemUpgradeButtonA.onClick.AddListener(() => OnUpgradeSelected(0)); // 버튼 A 클릭 시 0번 선택

        itemUpgradeButtonB.onClick.RemoveAllListeners();
        itemUpgradeButtonB.onClick.AddListener(() => OnUpgradeSelected(1)); // 버튼 B 클릭 시 1번 선택

        itemUpgradeButtonC.onClick.RemoveAllListeners();
        itemUpgradeButtonC.onClick.AddListener(() => OnUpgradeSelected(2)); // 버튼 C 클릭 시 2번 선택
    }

    // 업그레이드 선택 시 호출되는 메서드
    public void OnUpgradeSelected(int selection)
    {
        player.ApplyUpgrade(selection); // 플레이어의 ApplyUpgrade 메서드를 호출하여 선택된 업그레이드 적용

        levelUpGroup.SetActive(false); // 레벨업 패널 비활성화
        Time.timeScale = 1f;           // 게임 시간 재개 (일시정지 상태에서 호출될 경우)
    }

    // 게임 오버 시 호출되는 메서드
    public void GameOver()
    {
        gamePanel.SetActive(false); // 게임 UI 패널 비활성화
        overPanel.SetActive(true);  // 게임 오버 패널 활성화
        curScoreText.text = scoreTxt.text; // 현재 점수를 게임 오버 패널에 표시

        bestText.gameObject.SetActive(false); // "BEST" 텍스트 초기에는 비활성화

        int maxScore = PlayerPrefs.GetInt("MaxScore", 0); // 저장된 최고 점수 불러오기
        if (player.score > maxScore) // 현재 점수가 최고 점수보다 높으면
        {
            bestText.gameObject.SetActive(true); // "BEST" 텍스트 활성화
            PlayerPrefs.SetInt("MaxScore", player.score); // 최고 점수 업데이트
        }
    }

    // 게임 오버 후 메인 메뉴로 돌아갈 때 호출되는 메서드
    public void GameReturn()
    {
        menuCam.SetActive(true);  // 메뉴 카메라 활성화
        gameCam.SetActive(false); // 게임 카메라 비활성화
        menuPanel.SetActive(true);  // 메뉴 패널 활성화
        gamePanel.SetActive(false); // 게임 UI 패널 비활성화
        pausePanel.SetActive(false); // 일시정지 패널 비활성화 (혹시 모를 경우)

        // 현재 게임 상태 저장
        PlayerPrefs.SetInt("Stage", stage);
        PlayerPrefs.SetFloat("PlayTime", playTime);
        player.Save(); // 플레이어 데이터 저장

        player.gameObject.SetActive(false); // 플레이어 오브젝트 비활성화
    }

    // 게임 재시작 버튼 클릭 시 호출되는 메서드 (씬 다시 로드)
    public void Restart()
    {
        SceneManager.LoadScene(0); // 현재 씬 (인덱스 0)을 다시 로드하여 게임 재시작
    }

    // 스테이지 시작 시 호출되는 메서드
    public void StageStart()
    {
        itemShop.SetActive(false);   // 아이템 상점 비활성화
        weaponShop.SetActive(false); // 무기 상점 비활성화
        startZone.SetActive(false);  // 시작 구역 비활성화

        // 모든 적 스폰 구역 활성화
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;           // 전투 상태로 설정
        StartCoroutine(InBattle()); // 전투 코루틴 시작
    }

    // 스테이지 종료 시 호출되는 메서드
    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f; // 플레이어를 특정 위치로 이동 (스테이지 중앙 등)

        itemShop.SetActive(true);   // 아이템 상점 활성화
        weaponShop.SetActive(true); // 무기 상점 활성화
        startZone.SetActive(true);  // 시작 구역 활성화

        // 모든 적 스폰 구역 비활성화
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false; // 전투 상태 해제
        stage++;          // 스테이지 번호 증가
    }

    // 전투를 관리하는 코루틴 (적 생성 로직 포함)
    IEnumerator InBattle()
    {
        // 스테이지 번호가 5의 배수이면 보스 스테이지
        if (stage % 5 == 0)
        {
            enemyCntD++; // D타입(보스) 적 카운트 증가
            // 보스 적 생성 (enemies 배열의 3번 인덱스는 보스 프리팹)
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>(); // 생성된 적의 Enemy 스크립트 가져오기
            enemy.Target = player.transform;                  // 적의 타겟을 플레이어로 설정
            enemy.manager = this;                             // 적에게 GameManager 참조 전달
            boss = instantEnemy.GetComponent<Boss>();         // 생성된 적의 Boss 스크립트 가져와 보스 변수에 할당
        }
        // 일반 스테이지
        else
        {
            // 현재 스테이지 번호만큼 적을 리스트에 추가
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3); // 0, 1, 2 중 랜덤 선택 (A, B, C 타입 적)
                enemyList.Add(ran);            // 적 타입 인덱스를 리스트에 추가

                // 각 타입별 적 카운트 증가
                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            // 적 리스트에 적이 남아있는 동안 스폰
            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4); // 0부터 3까지 랜덤 스폰 구역 선택
                // 리스트의 첫 번째 적을 스폰 구역에 생성
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>(); // 생성된 적의 Enemy 스크립트 가져오기
                enemy.Target = player.transform;                  // 적의 타겟을 플레이어로 설정
                enemy.manager = this;                             // 적에게 GameManager 참조 전달
                enemyList.RemoveAt(0);                            // 스폰된 적은 리스트에서 제거
                yield return new WaitForSeconds(4f);              // 4초 대기 후 다음 적 스폰
            }
        }

        // 모든 적 (A, B, C, D 타입)이 처치될 때까지 대기
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null; // 한 프레임 대기
        }

        yield return new WaitForSeconds(4f); // 모든 적 처치 후 4초 대기
        boss = null; // 보스 참조 초기화 (다음 스테이지를 위해)
        StageEnd();  // 스테이지 종료 메서드 호출
    }

    // 매 프레임 업데이트되는 메서드
    void Update()
    {
        // 전투 중일 때만 플레이 시간을 증가시킵니다.
        if (isBattle)
            playTime += Time.deltaTime;
    }

    // 모든 Update 함수가 호출된 후 매 프레임 호출되는 메서드 (주로 UI 업데이트에 사용)
    void LateUpdate()
    {
        // 좌측 상단 UI 업데이트
        int maxscore = PlayerPrefs.GetInt("MaxScore", 0); // 최고 점수 불러오기
        scoreTxt.text = string.Format("{0:n0}", player.score); // 현재 점수 UI 업데이트
        PausebestText.text = string.Format("Best:{0:n0}", maxscore); // 일시정지 화면 최고 점수
        PauseText.text = string.Format("Now:{0:n0}", player.score);   // 일시정지 화면 현재 점수
        if (player.score > maxscore) // 현재 점수가 최고 점수보다 높으면
        {
            PausebestText.text = string.Format("Best:{0:n0}", player.score); // 일시정지 화면 최고 점수 업데이트
        }

        // 우측 상단 UI 업데이트
        stageTxt.text = "STAGE " + stage; // 스테이지 번호 UI 업데이트
        int hour = (int)(playTime / 3600);           // 플레이 시간을 시로 변환
        int min = (int)((playTime - hour * 3600) / 60); // 플레이 시간을 분으로 변환
        int second = (int)(playTime % 60);           // 플레이 시간을 초로 변환
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second); // 플레이 시간 UI 업데이트 (00:00:00 형식)

        // 플레이어 (좌측 하단) UI 업데이트
        playerHealthTxt.text = player.health + " / " + player.maxHealth; // 플레이어 체력 UI 업데이트
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);     // 플레이어 코인 UI 업데이트
        // 플레이어가 장착한 무기에 따라 총알/탄약 UI 업데이트
        if (player.equipWeapon == null) // 장착된 무기가 없으면
            playerAmmoTxt.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee) // 근접 무기이면
            playerAmmoTxt.text = "- / " + player.ammo;
        else // 원거리 무기이면
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        // 무기 (중앙 하단) UI 업데이트 (플레이어가 특정 무기를 가지고 있는지에 따라 이미지 투명도 조절)
        weapon1Img.color = new Color(1, 1, 1, (player.hasWeapons[0] ? 1 : 0)); // 무기 1 보유 시 불투명, 아니면 투명
        weapon2Img.color = new Color(1, 1, 1, (player.hasWeapons[1] ? 1 : 0)); // 무기 2
        weapon3Img.color = new Color(1, 1, 1, (player.hasWeapons[2] ? 1 : 0)); // 무기 3
        weaponRImg.color = new Color(1, 1, 1, (player.hasGrenades > 0 ? 1 : 0)); // 수류탄 보유 시 불투명, 아니면 투명

        // 몬스터 숫자 (우측 하단) UI 업데이트
        enemyATxt.text = enemyCntA.ToString(); // A타입 적 수 UI 업데이트
        enemyBTxt.text = enemyCntB.ToString(); // B타입 적 수 UI 업데이트
        enemyCTxt.text = enemyCntC.ToString(); // C타입 적 수 UI 업데이트

        // 보스 체력 (중앙 상단) UI 업데이트
        if (boss != null) // 보스가 존재하면
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30; // 보스 체력바를 화면에 보이도록 이동

            // 보스 체력 비율 계산 (0과 1 사이로 강제 조정하여 바가 올바르게 표시되도록 함)
            float bossHpRatio = Mathf.Clamp((float)boss.curHealth / boss.maxHealth, 0f, 1f);

            // 보스 체력바의 가로 스케일을 체력 비율에 맞춰 조절
            bossHealthBar.localScale = new Vector3(bossHpRatio, 1, 1);
        }
        else // 보스가 없으면
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200; // 보스 체력바를 화면 밖으로 이동하여 숨김
        }

        // 플레이어 경험치 바 UI 업데이트
        if (player != null && expBar != null)
        {
            float expRatio = (float)player.currentExperience / player.maxExperience; // 경험치 비율 계산
            expBar.fillAmount = Mathf.Clamp01(expRatio); // 경험치 바 채우는 양 업데이트 (0과 1 사이로 제한)
        }
    }
}
