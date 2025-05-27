using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public Camera followCamera;
    public GameManager manager;


    public AudioSource jumpSound;
    public AudioSource dodgeSound;
    public AudioSource interactionSound;
    public AudioSource swapSound;

    public int level = 1;
    public int currentExperience = 0;
    public int maxExperience = 100; // 레벨업에 필요한 경험치 기본값 

    public int ammo;
    public int coin;
    public int health;
    public int score;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool escDown;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead = false;
    bool isPause = false;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;

    public Weapon equippedWeapon;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interation();
        Pause();
        if (Input.GetButtonDown("Fire1") && equippedWeapon != null)
        {
            equippedWeapon.Use();
        }
    }
    // 경험치를 추가하는 함수
    public void AddExperience(int exp)
    {
        currentExperience += exp;

        // 여러 레벨업 처리
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
        maxHealth += 10;
        health = maxHealth;
    }

    public void Save()
    {
        isPause = false;
        PlayerPrefs.SetInt("HasGrenades", hasGrenades);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("CurrentExperience", currentExperience);
        PlayerPrefs.SetInt("MaxExperience", maxExperience);
        PlayerPrefs.SetInt("Ammo", ammo);
        PlayerPrefs.SetInt("Coin", coin);
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetInt("MaxHealth", maxHealth);
        PlayerPrefs.SetInt("Score", score);
        SaveWeapons(hasWeapons);
    }

    public void SaveWeapons(bool[] hasWeapons)
    {
        string weaponData = "";
        foreach (bool has in hasWeapons)
        {
            weaponData += has ? "1" : "0";
        }
        PlayerPrefs.SetString("HasWeapons", weaponData);
        PlayerPrefs.Save();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButton("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        escDown = Input.GetButtonDown("Pause");
    }

    void Pause()
    {
        if (!escDown || manager.isBattle)
            return;
        if (isPause)
        {
            isPause = false;
            manager.pausePanel.SetActive(false);
        }
        else if (!isPause)
        {
            isPause = true;
            manager.pausePanel.SetActive(true);
        }
    }


    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady || isReload || isDead || isPause)
            moveVec = Vector3.zero;

        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        if (fDown && !isDead && !isPause)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }

    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isDead && !isPause)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

            jumpSound.Play();
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if (gDown && !isReload && !isSwap && !isDead && !isPause)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.freezeRotation = true;
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);

            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead && !isPause)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null) return;

        if (equipWeapon.type == Weapon.Type.Melee) return;

        if(equipWeapon.curAmmo == equipWeapon.maxAmmo) return;

        if (ammo == 0) return;

        if (isReload) return;

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead && !isPause)
        {
            anim.SetTrigger("doReload");
            isReload = true;
            Debug.Log(isReload);
            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        int reAmmo =  equipWeapon.maxAmmo;
        int needAmmo = reAmmo - equipWeapon.curAmmo;
        if (needAmmo > ammo)
        {
            equipWeapon.curAmmo += ammo;
            ammo = 0;
        }
        else
        {
            equipWeapon.curAmmo = equipWeapon.maxAmmo;
            ammo -= needAmmo;
        }
        isReload = false;
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop && !isDead && !isPause)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            dodgeSound.Play();

            Invoke("DodgeOut", 0.6f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead && !isPause)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
            swapSound.Play();
        }

    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !isDead && !isPause)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if(nearObject.tag == "shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
            interactionSound.Play();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);  

        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
            Debug.Log("���� ���� �õ�: " + mesh.gameObject.name);
        }
        if (isBossAtk)
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }

        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f);

        isDamage = false;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
            Debug.Log("���� ����: " + mesh.gameObject.name);
        }

        if (isBossAtk)
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "shop")
            nearObject = other.gameObject;
        if (nearObject != null)
            Debug.Log(nearObject.name);

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        else if (other.tag == "shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
    public void EquipWeapon(Weapon newWeapon)
    {
        equippedWeapon = newWeapon;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            OnDie();
        }
    }
}