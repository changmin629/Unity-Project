using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;


    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public GameObject pausePanel;
    public TextMeshProUGUI maxScoreTxt;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI stageTxt;
    public TextMeshProUGUI playTimeTxt;
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerAmmoTxt;
    public TextMeshProUGUI playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public TextMeshProUGUI enemyATxt;
    public TextMeshProUGUI enemyBTxt;
    public TextMeshProUGUI enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public TextMeshProUGUI curScoreText;
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI PausebestText;
    public TextMeshProUGUI PauseText;

    public RectTransform expGroup;
    public Image expBar;

    void Awake()
    {
        enemyList = new List<int>();
        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);

        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    public void GameStart()
    {
        Re();
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameQuit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void GameContinue()
    {
        Load();
        if (!PlayerPrefs.HasKey("Score"))
            return;
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    void Re()
    {
        player.gameObject.SetActive(false);
        int maxScore = PlayerPrefs.GetInt("MaxScore");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("MaxScore",maxScore);
        stage = 1;
        playTime = 0;
        player.hasGrenades = 0;
        player.level = 1;
        player.currentExperience = 0;
        player.maxExperience = 100;
        player.ammo = 0;
        player.coin = 5500;
        player.health = 100;
        player.maxHealth = 100;
        player.score = 0;
        player.hasWeapons = new bool[] { false, false, false };
        player.hasWeapons = LoadWeapons(3);
    }
    void Load()
    {
        stage = PlayerPrefs.GetInt("Stage");
        playTime = PlayerPrefs.GetFloat("PlayTime");
        player.hasGrenades = PlayerPrefs.GetInt("HasGrenades");
        player.level = PlayerPrefs.GetInt("Level");
        player.currentExperience = PlayerPrefs.GetInt("CurrentExperience");
        player.maxExperience = PlayerPrefs.GetInt("MaxExperience");
        player.ammo =  PlayerPrefs.GetInt("Ammo");
        player.coin = PlayerPrefs.GetInt("Coin");
        player.health = PlayerPrefs.GetInt("Health");
        player.maxHealth = PlayerPrefs.GetInt("MaxHealth");
        player.score =  PlayerPrefs.GetInt("Score");
        player.hasWeapons = LoadWeapons(3);
    }
    public bool[] LoadWeapons(int weaponCount)
    {
        string weaponData = PlayerPrefs.GetString("HasWeapons", new string('0', weaponCount));
        bool[] hasWeapons = new bool[weaponCount];

        for (int i = 0; i < weaponCount && i < weaponData.Length; i++)
        {
            hasWeapons[i] = weaponData[i] == '1';
        }

        return hasWeapons;
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        bestText.gameObject.SetActive(false);

        int maxScore = PlayerPrefs.GetInt("MaxScore", 0);
        if (player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void GameReturn()
    {
        menuCam.SetActive(true);
        gameCam.SetActive(false);
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        PlayerPrefs.SetInt("Stage", stage);
        PlayerPrefs.SetFloat("PlayTime", playTime);
        player.Save();
        player.gameObject.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.Target = player.transform;
            enemy.manager = this;   //GameManager �����ֱ�
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

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

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.Target = player.transform;
                enemy.manager = this;   //GameManager �����ֱ�
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }

    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        //���� ��� UI
        int maxscore = PlayerPrefs.GetInt("MaxScore", 0);
        scoreTxt.text = string.Format("{0:n0}", player.score);
        PausebestText.text = string.Format("Best:{0:n0}", maxscore);
        PauseText.text = string.Format("Now:{0:n0}", player.score);
        if (player.score > maxscore)
        {
            PausebestText.text = string.Format("Best:{0:n0}", player.score);
        }

        //���� ��� UI
        stageTxt.text = "STAGE " + stage;
        int hour = (int)(playTime / 3600);  //��
        int min = (int)((playTime - hour * 3600) / 60);   //��
        int second = (int)(playTime % 60);  //��
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        //�÷��̾�(���� �ϴ�) UI
        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if (player.equipWeapon == null)
            playerAmmoTxt.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "- / " + player.ammo;
        else
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        //����(�߾� �ϴ�) UI
        weapon1Img.color = new Color(1, 1, 1, (player.hasWeapons[0] ? 1 : 0));
        weapon2Img.color = new Color(1, 1, 1, (player.hasWeapons[1] ? 1 : 0));
        weapon3Img.color = new Color(1, 1, 1, (player.hasWeapons[2] ? 1 : 0));
        weaponRImg.color = new Color(1, 1, 1, (player.hasGrenades > 0 ? 1 : 0));

        //���� ����(���� �ϴ�) UI
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        //���� ü��(�߾� ���) UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;

            // ü�� ���� ��� (0 ���Ϸ� �������� �ʵ��� Mathf.Clamp ���)
            float bossHpRatio = Mathf.Clamp((float)boss.curHealth / boss.maxHealth, 0f, 1f);

            // ü�¹� ������ ����
            bossHealthBar.localScale = new Vector3(bossHpRatio, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }

        if (player != null && expBar != null)
        {
            float expRatio = (float)player.currentExperience / player.maxExperience;
            expBar.fillAmount = Mathf.Clamp01(expRatio);
        }

    }
}
