using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;

    Vector3 lookVec;
    Vector3 tauntVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;

        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(Target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
            case 2:
                StartCoroutine(MissileShot());
                break;
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);

        if (missile != null && missilePortA != null)
        {
            GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
            BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
            bossMissileA.target = Target;

            yield return new WaitForSeconds(0.3f);

            GameObject instantMissileB = Instantiate(missile, missilePortA.position, missilePortA.rotation);
            BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
            bossMissileB.target = Target;
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");

        if (bullet != null)
            Instantiate(bullet, transform.position, transform.rotation);

        yield return new WaitForSeconds(3f);
        isLook = true;

        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = Target.position + lookVec;

        isLook = false;
        nav.isStopped = false;

        // 안전하게 비활성화 시도
        if (boxCollider != null)
        {
            try { boxCollider.enabled = false; } catch { }
        }

        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);

        if (meleeArea != null)
            meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);

        if (meleeArea != null)
            meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        isLook = true;
        nav.isStopped = true;

        if (boxCollider != null)
        {
            try { boxCollider.enabled = true; } catch { }
        }

        StartCoroutine(Think());
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}

