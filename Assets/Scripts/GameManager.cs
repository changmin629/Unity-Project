using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�
using UnityEngine.SceneManagement; // �� ������ ���� ���ӽ����̽�

// GameManager Ŭ������ ������ �������� �帧, UI ������Ʈ, �������� ���� ���� ����մϴ�.
public class GameManager : MonoBehaviour
{
    // ī�޶� �� �ֿ� ���� ������Ʈ ����
    public GameObject menuCam;    // �޴� ȭ�� ī�޶�
    public GameObject gameCam;    // ���� �÷��� ī�޶�
    public Player player;         // �÷��̾� ��ũ��Ʈ ����
    public Boss boss;             // ���� ��ũ��Ʈ ���� (���� ������������ ���)
    public GameObject itemShop;   // ������ ���� ������Ʈ
    public GameObject weaponShop; // ���� ���� ������Ʈ
    public GameObject startZone;  // �������� ���� ���� ������Ʈ

    // ���� ���� ���� ����
    public int stage;             // ���� �������� ��ȣ
    public float playTime;        // ���� ���� �÷��� �ð�
    public bool isBattle;         // ���� ���� ������ ����

    // �� ī��Ʈ (�� Ÿ�Ժ� ���� ���� ��)
    public int enemyCntA; // AŸ�� ���� ��
    public int enemyCntB; // BŸ�� ���� ��
    public int enemyCntC; // CŸ�� ���� ��
    public int enemyCntD; // DŸ��(����) ���� ��

    // �� ���� ���� ����
    public Transform[] enemyZones; // ���� ������ �������� Transform �迭
    public GameObject[] enemies;   // �� ������ �迭 (A, B, C, D Ÿ�� ����)
    public List<int> enemyList;    // ���� ������������ ������ ������ Ÿ�� �ε����� �����ϴ� ����Ʈ

    // UI �г� ����
    public GameObject menuPanel;  // ���� �޴� �г�
    public GameObject gamePanel;  // ���� �÷��� UI �г�
    public GameObject overPanel;  // ���� ���� �г�
    public GameObject pausePanel; // �Ͻ����� �г�

    // UI �ؽ�Ʈ �� �̹��� ����
    public TextMeshProUGUI maxScoreTxt;     // �ְ� ���� ǥ�� �ؽ�Ʈ (���� �޴�)
    public TextMeshProUGUI scoreTxt;        // ���� ���� ǥ�� �ؽ�Ʈ (���� �÷��� ��)
    public TextMeshProUGUI stageTxt;        // ���� �������� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI playTimeTxt;     // �÷��� �ð� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI playerHealthTxt; // �÷��̾� ü�� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI playerAmmoTxt;   // �÷��̾� �Ѿ�/ź�� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI playerCoinTxt;   // �÷��̾� ���� ǥ�� �ؽ�Ʈ
    public Image weapon1Img;      // ���� 1 UI �̹��� (Ȱ��ȭ ���� ǥ��)
    public Image weapon2Img;      // ���� 2 UI �̹���
    public Image weapon3Img;      // ���� 3 UI �̹���
    public Image weaponRImg;      // ����ź UI �̹��� (���� ���� ǥ��)
    public TextMeshProUGUI enemyATxt; // AŸ�� �� ���� �� �ؽ�Ʈ
    public TextMeshProUGUI enemyBTxt; // BŸ�� �� ���� �� �ؽ�Ʈ
    public TextMeshProUGUI enemyCTxt; // CŸ�� �� ���� �� �ؽ�Ʈ

    // ���� ü�¹� UI
    public RectTransform bossHealthGroup; // ���� ü�¹� ��ü �׷��� RectTransform
    public RectTransform bossHealthBar;   // ���� ü�¹��� RectTransform (���� ü�� ��ȭ�� �ð�ȭ)

    // ���� ����/�Ͻ����� ȭ���� ���� �ؽ�Ʈ
    public TextMeshProUGUI curScoreText;    // ���� ���� �� ���� ����
    public TextMeshProUGUI bestText;        // ���� ���� �� �ְ� ���� ���� ǥ��
    public TextMeshProUGUI PausebestText;   // �Ͻ����� �� �ְ� ����
    public TextMeshProUGUI PauseText;       // �Ͻ����� �� ���� ����

    // ������ UI ���
    public GameObject levelUpGroup;        // ������ �г� ��ü �׷�
    public Button itemUpgradeButtonA;      // ������ ���׷��̵� ��ư A
    public Button itemUpgradeButtonB;      // ������ ���׷��̵� ��ư B
    public Button itemUpgradeButtonC;      // ������ ���׷��̵� ��ư C

    // ����ġ �� UI
    public RectTransform expGroup; // ����ġ �� �׷� (��ü)
    public Image expBar;           // ����ġ �� �̹��� (���� ä������ �κ�)

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� ȣ��˴ϴ�. (���� �� �� ��)
    void Awake()
    {
        enemyList = new List<int>(); // �� ����Ʈ �ʱ�ȭ

        // "MaxScore" Ű�� PlayerPrefs�� ������ 0���� �ʱ� �����մϴ�.
        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);

        // ����� �ְ� ������ �ҷ��� UI�� ǥ���մϴ�. (õ ���� ���� ��ȣ ����)
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void GameStart()
    {
        Re(); // ���� ������ �ʱ�ȭ
        menuCam.SetActive(false); // �޴� ī�޶� ��Ȱ��ȭ
        gameCam.SetActive(true);  // ���� ī�޶� Ȱ��ȭ

        menuPanel.SetActive(false); // �޴� �г� ��Ȱ��ȭ
        gamePanel.SetActive(true);  // ���� UI �г� Ȱ��ȭ

        player.gameObject.SetActive(true); // �÷��̾� ������Ʈ Ȱ��ȭ
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void GameQuit()
    {
        // ������ ȯ��� ����� ȯ�濡 ���� �ٸ��� �۵��մϴ�.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� �÷��� ��� ����
#else
        Application.Quit(); // ����� ���� ����
#endif
    }

    // ���� �̾��ϱ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void GameContinue()
    {
        Load(); // ����� ���� ������ �ε�
        // "Score" Ű�� ������ (����� ������ ������) �̾��ϱ⸦ �ߴ��մϴ�.
        if (!PlayerPrefs.HasKey("Score"))
            return;

        menuCam.SetActive(false); // �޴� ī�޶� ��Ȱ��ȭ
        gameCam.SetActive(true);  // ���� ī�޶� Ȱ��ȭ

        menuPanel.SetActive(false); // �޴� �г� ��Ȱ��ȭ
        gamePanel.SetActive(true);  // ���� UI �г� Ȱ��ȭ

        player.gameObject.SetActive(true); // �÷��̾� ������Ʈ Ȱ��ȭ
    }

    // ���� �����͸� �ʱ�ȭ�ϴ� �޼��� (�� ���� ���� �� ȣ��)
    void Re()
    {
        player.gameObject.SetActive(false); // �÷��̾� ������Ʈ ��Ȱ��ȭ (�ʱ�ȭ ��)
        int maxScore = PlayerPrefs.GetInt("MaxScore"); // ���� �ְ� ������ �ӽ� ����
        PlayerPrefs.DeleteAll(); // ��� PlayerPrefs ������ ���� (�ʱ�ȭ)
        PlayerPrefs.SetInt("MaxScore", maxScore); // ������ �ְ� ������ �ٽ� ���� (����)

        // ���� ���� ���� �ʱ�ȭ
        stage = 1;
        playTime = 0;
        player.hasGrenades = 0;
        player.level = 1;
        player.currentExperience = 0;
        player.maxExperience = 100;
        player.ammo = 0;
        player.coin = 5500; // �ʱ� ���� ����
        player.health = 100;
        player.maxHealth = 100;
        player.score = 0;
        player.hasWeapons = new bool[] { false, false, false }; // ��� ���� �̺����� �ʱ�ȭ
        player.hasWeapons = LoadWeapons(3); // ���� ���� ���� �ε� (PlayerPrefs����)
    }

    // ����� ���� �����͸� �ε��ϴ� �޼���
    void Load()
    {
        // PlayerPrefs���� �� ���� �����͸� �ҷ��� ������ �Ҵ��մϴ�.
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
        player.hasWeapons = LoadWeapons(3); // ���� ���� ���� �ε�
    }

    // ���� ���� ���¸� ���ڿ��� ����� PlayerPrefs���� �ҷ��� bool �迭�� ��ȯ�ϴ� �޼���
    public bool[] LoadWeapons(int weaponCount)
    {
        // "HasWeapons" Ű�� ����� ���ڿ��� �ҷ��ɴϴ�. ������ '0'���� ä���� ���ڿ��� �⺻������ ����մϴ�.
        string weaponData = PlayerPrefs.GetString("HasWeapons", new string('0', weaponCount));
        bool[] hasWeapons = new bool[weaponCount]; // ���� ������ŭ bool �迭 ����

        // ���ڿ��� ��ȸ�ϸ� '1'�̸� true, '0'�̸� false�� �迭�� �����մϴ�.
        for (int i = 0; i < weaponCount && i < weaponData.Length; i++)
        {
            hasWeapons[i] = weaponData[i] == '1';
        }
        return hasWeapons; // �ϼ��� bool �迭 ��ȯ
    }

    // ������ �г��� �����ִ� �޼���
    public void ShowLevelUpPanel()
    {
        levelUpGroup.SetActive(true); // ������ �г� Ȱ��ȭ

        RectTransform levelUpRect = levelUpGroup.GetComponent<RectTransform>();
        if (levelUpRect != null)
        {
            levelUpRect.anchoredPosition = Vector3.zero; // ������ �г��� ȭ�� �߾����� �̵�
        }

        // ���׷��̵� ��ư�� Ŭ�� ������ �Ҵ� (���� �����ʴ� �����Ͽ� �ߺ� ����)
        itemUpgradeButtonA.onClick.RemoveAllListeners();
        itemUpgradeButtonA.onClick.AddListener(() => OnUpgradeSelected(0)); // ��ư A Ŭ�� �� 0�� ����

        itemUpgradeButtonB.onClick.RemoveAllListeners();
        itemUpgradeButtonB.onClick.AddListener(() => OnUpgradeSelected(1)); // ��ư B Ŭ�� �� 1�� ����

        itemUpgradeButtonC.onClick.RemoveAllListeners();
        itemUpgradeButtonC.onClick.AddListener(() => OnUpgradeSelected(2)); // ��ư C Ŭ�� �� 2�� ����
    }

    // ���׷��̵� ���� �� ȣ��Ǵ� �޼���
    public void OnUpgradeSelected(int selection)
    {
        player.ApplyUpgrade(selection); // �÷��̾��� ApplyUpgrade �޼��带 ȣ���Ͽ� ���õ� ���׷��̵� ����

        levelUpGroup.SetActive(false); // ������ �г� ��Ȱ��ȭ
        Time.timeScale = 1f;           // ���� �ð� �簳 (�Ͻ����� ���¿��� ȣ��� ���)
    }

    // ���� ���� �� ȣ��Ǵ� �޼���
    public void GameOver()
    {
        gamePanel.SetActive(false); // ���� UI �г� ��Ȱ��ȭ
        overPanel.SetActive(true);  // ���� ���� �г� Ȱ��ȭ
        curScoreText.text = scoreTxt.text; // ���� ������ ���� ���� �гο� ǥ��

        bestText.gameObject.SetActive(false); // "BEST" �ؽ�Ʈ �ʱ⿡�� ��Ȱ��ȭ

        int maxScore = PlayerPrefs.GetInt("MaxScore", 0); // ����� �ְ� ���� �ҷ�����
        if (player.score > maxScore) // ���� ������ �ְ� �������� ������
        {
            bestText.gameObject.SetActive(true); // "BEST" �ؽ�Ʈ Ȱ��ȭ
            PlayerPrefs.SetInt("MaxScore", player.score); // �ְ� ���� ������Ʈ
        }
    }

    // ���� ���� �� ���� �޴��� ���ư� �� ȣ��Ǵ� �޼���
    public void GameReturn()
    {
        menuCam.SetActive(true);  // �޴� ī�޶� Ȱ��ȭ
        gameCam.SetActive(false); // ���� ī�޶� ��Ȱ��ȭ
        menuPanel.SetActive(true);  // �޴� �г� Ȱ��ȭ
        gamePanel.SetActive(false); // ���� UI �г� ��Ȱ��ȭ
        pausePanel.SetActive(false); // �Ͻ����� �г� ��Ȱ��ȭ (Ȥ�� �� ���)

        // ���� ���� ���� ����
        PlayerPrefs.SetInt("Stage", stage);
        PlayerPrefs.SetFloat("PlayTime", playTime);
        player.Save(); // �÷��̾� ������ ����

        player.gameObject.SetActive(false); // �÷��̾� ������Ʈ ��Ȱ��ȭ
    }

    // ���� ����� ��ư Ŭ�� �� ȣ��Ǵ� �޼��� (�� �ٽ� �ε�)
    public void Restart()
    {
        SceneManager.LoadScene(0); // ���� �� (�ε��� 0)�� �ٽ� �ε��Ͽ� ���� �����
    }

    // �������� ���� �� ȣ��Ǵ� �޼���
    public void StageStart()
    {
        itemShop.SetActive(false);   // ������ ���� ��Ȱ��ȭ
        weaponShop.SetActive(false); // ���� ���� ��Ȱ��ȭ
        startZone.SetActive(false);  // ���� ���� ��Ȱ��ȭ

        // ��� �� ���� ���� Ȱ��ȭ
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;           // ���� ���·� ����
        StartCoroutine(InBattle()); // ���� �ڷ�ƾ ����
    }

    // �������� ���� �� ȣ��Ǵ� �޼���
    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f; // �÷��̾ Ư�� ��ġ�� �̵� (�������� �߾� ��)

        itemShop.SetActive(true);   // ������ ���� Ȱ��ȭ
        weaponShop.SetActive(true); // ���� ���� Ȱ��ȭ
        startZone.SetActive(true);  // ���� ���� Ȱ��ȭ

        // ��� �� ���� ���� ��Ȱ��ȭ
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false; // ���� ���� ����
        stage++;          // �������� ��ȣ ����
    }

    // ������ �����ϴ� �ڷ�ƾ (�� ���� ���� ����)
    IEnumerator InBattle()
    {
        // �������� ��ȣ�� 5�� ����̸� ���� ��������
        if (stage % 5 == 0)
        {
            enemyCntD++; // DŸ��(����) �� ī��Ʈ ����
            // ���� �� ���� (enemies �迭�� 3�� �ε����� ���� ������)
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>(); // ������ ���� Enemy ��ũ��Ʈ ��������
            enemy.Target = player.transform;                  // ���� Ÿ���� �÷��̾�� ����
            enemy.manager = this;                             // ������ GameManager ���� ����
            boss = instantEnemy.GetComponent<Boss>();         // ������ ���� Boss ��ũ��Ʈ ������ ���� ������ �Ҵ�
        }
        // �Ϲ� ��������
        else
        {
            // ���� �������� ��ȣ��ŭ ���� ����Ʈ�� �߰�
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3); // 0, 1, 2 �� ���� ���� (A, B, C Ÿ�� ��)
                enemyList.Add(ran);            // �� Ÿ�� �ε����� ����Ʈ�� �߰�

                // �� Ÿ�Ժ� �� ī��Ʈ ����
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

            // �� ����Ʈ�� ���� �����ִ� ���� ����
            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4); // 0���� 3���� ���� ���� ���� ����
                // ����Ʈ�� ù ��° ���� ���� ������ ����
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>(); // ������ ���� Enemy ��ũ��Ʈ ��������
                enemy.Target = player.transform;                  // ���� Ÿ���� �÷��̾�� ����
                enemy.manager = this;                             // ������ GameManager ���� ����
                enemyList.RemoveAt(0);                            // ������ ���� ����Ʈ���� ����
                yield return new WaitForSeconds(4f);              // 4�� ��� �� ���� �� ����
            }
        }

        // ��� �� (A, B, C, D Ÿ��)�� óġ�� ������ ���
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null; // �� ������ ���
        }

        yield return new WaitForSeconds(4f); // ��� �� óġ �� 4�� ���
        boss = null; // ���� ���� �ʱ�ȭ (���� ���������� ����)
        StageEnd();  // �������� ���� �޼��� ȣ��
    }

    // �� ������ ������Ʈ�Ǵ� �޼���
    void Update()
    {
        // ���� ���� ���� �÷��� �ð��� ������ŵ�ϴ�.
        if (isBattle)
            playTime += Time.deltaTime;
    }

    // ��� Update �Լ��� ȣ��� �� �� ������ ȣ��Ǵ� �޼��� (�ַ� UI ������Ʈ�� ���)
    void LateUpdate()
    {
        // ���� ��� UI ������Ʈ
        int maxscore = PlayerPrefs.GetInt("MaxScore", 0); // �ְ� ���� �ҷ�����
        scoreTxt.text = string.Format("{0:n0}", player.score); // ���� ���� UI ������Ʈ
        PausebestText.text = string.Format("Best:{0:n0}", maxscore); // �Ͻ����� ȭ�� �ְ� ����
        PauseText.text = string.Format("Now:{0:n0}", player.score);   // �Ͻ����� ȭ�� ���� ����
        if (player.score > maxscore) // ���� ������ �ְ� �������� ������
        {
            PausebestText.text = string.Format("Best:{0:n0}", player.score); // �Ͻ����� ȭ�� �ְ� ���� ������Ʈ
        }

        // ���� ��� UI ������Ʈ
        stageTxt.text = "STAGE " + stage; // �������� ��ȣ UI ������Ʈ
        int hour = (int)(playTime / 3600);           // �÷��� �ð��� �÷� ��ȯ
        int min = (int)((playTime - hour * 3600) / 60); // �÷��� �ð��� ������ ��ȯ
        int second = (int)(playTime % 60);           // �÷��� �ð��� �ʷ� ��ȯ
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second); // �÷��� �ð� UI ������Ʈ (00:00:00 ����)

        // �÷��̾� (���� �ϴ�) UI ������Ʈ
        playerHealthTxt.text = player.health + " / " + player.maxHealth; // �÷��̾� ü�� UI ������Ʈ
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);     // �÷��̾� ���� UI ������Ʈ
        // �÷��̾ ������ ���⿡ ���� �Ѿ�/ź�� UI ������Ʈ
        if (player.equipWeapon == null) // ������ ���Ⱑ ������
            playerAmmoTxt.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee) // ���� �����̸�
            playerAmmoTxt.text = "- / " + player.ammo;
        else // ���Ÿ� �����̸�
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        // ���� (�߾� �ϴ�) UI ������Ʈ (�÷��̾ Ư�� ���⸦ ������ �ִ����� ���� �̹��� ���� ����)
        weapon1Img.color = new Color(1, 1, 1, (player.hasWeapons[0] ? 1 : 0)); // ���� 1 ���� �� ������, �ƴϸ� ����
        weapon2Img.color = new Color(1, 1, 1, (player.hasWeapons[1] ? 1 : 0)); // ���� 2
        weapon3Img.color = new Color(1, 1, 1, (player.hasWeapons[2] ? 1 : 0)); // ���� 3
        weaponRImg.color = new Color(1, 1, 1, (player.hasGrenades > 0 ? 1 : 0)); // ����ź ���� �� ������, �ƴϸ� ����

        // ���� ���� (���� �ϴ�) UI ������Ʈ
        enemyATxt.text = enemyCntA.ToString(); // AŸ�� �� �� UI ������Ʈ
        enemyBTxt.text = enemyCntB.ToString(); // BŸ�� �� �� UI ������Ʈ
        enemyCTxt.text = enemyCntC.ToString(); // CŸ�� �� �� UI ������Ʈ

        // ���� ü�� (�߾� ���) UI ������Ʈ
        if (boss != null) // ������ �����ϸ�
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30; // ���� ü�¹ٸ� ȭ�鿡 ���̵��� �̵�

            // ���� ü�� ���� ��� (0�� 1 ���̷� ���� �����Ͽ� �ٰ� �ùٸ��� ǥ�õǵ��� ��)
            float bossHpRatio = Mathf.Clamp((float)boss.curHealth / boss.maxHealth, 0f, 1f);

            // ���� ü�¹��� ���� �������� ü�� ������ ���� ����
            bossHealthBar.localScale = new Vector3(bossHpRatio, 1, 1);
        }
        else // ������ ������
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200; // ���� ü�¹ٸ� ȭ�� ������ �̵��Ͽ� ����
        }

        // �÷��̾� ����ġ �� UI ������Ʈ
        if (player != null && expBar != null)
        {
            float expRatio = (float)player.currentExperience / player.maxExperience; // ����ġ ���� ���
            expBar.fillAmount = Mathf.Clamp01(expRatio); // ����ġ �� ä��� �� ������Ʈ (0�� 1 ���̷� ����)
        }
    }
}
