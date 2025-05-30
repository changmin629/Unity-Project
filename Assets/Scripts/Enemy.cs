using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public int score;
    public GameManager manager;
    public Transform Target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    public GameObject poisonGas;

    public AudioClip attackSound;     // 적이 공격할 때 나는 소리
    public AudioClip hitSound;        // 적이 맞을 때 나는 소리
    public AudioClip deathSound;      // 적이 죽을 때 나는 소리
    private AudioSource audioSource;

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    public int experiencePoints;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.D)
            Invoke("ChaseStart", 2);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }


    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(Target.position);
            nav.isStopped = !isChase;
        }

    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }

    }

    void Targerting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadious = 1.5f;
            float targetRange = 3f;

            switch (enemyType)
            {
                case Type.A:
                    targetRadious = 1.5f;
                    targetRange = 3f;

                    break;
                case Type.B:
                    targetRadious = 1f;
                    targetRange = 12f;
                    break;

                case Type.C:
                    targetRadious = 0.5f;
                    targetRange = 25f;


                    break;
            }


            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position,
                targetRadious, transform.forward,
                targetRange,
                LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);

                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.linearVelocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);


                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.linearVelocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);

                break;
        }



        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;


            StartCoroutine(OnDamage(reactVec, false));


        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));

        }
    }
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        // 이미 죽은 적이라면 피해 무시
        if (isDead)
            yield break;

        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            yield return new WaitForSeconds(0.1f);

            if (isDead)
                yield break;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            isDead = true;

            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;

            
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            Player player = Target.GetComponent<Player>();
            
            if (player != null)
            {
                // 적의 타입에 따라 경험치를 다르게 처리
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
                    case Type.D:
                        exp = 100;  // Type.D의 적은 100 경험치를 줍니다.
                        break;
                }
                player.AddExperience(exp);  // 경험치 추가
            }

            player.score += score;
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    Instantiate(poisonGas,transform.position, Quaternion.identity);
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


            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);

            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            }

            Destroy(gameObject, 2f);
        }
    }

}



