using System.Collections;
using UnityEngine;

// Weapon 클래스는 게임 내에서 사용되는 무기들의 기본적인 동작을 정의합니다.
// 근접 무기와 원거리 무기 모두를 포함하며, 공격 방식에 따라 다른 동작을 수행합니다.
public class Weapon : MonoBehaviour
{
    // 무기의 종류를 정의하는 열거형 (Enum)
    public enum Type { Melee, Range };
    public Type type;   // 이 무기의 종류 (근접 또는 원거리)
    public int damage;  // 무기의 공격력
    public float rate;  // 공격 속도 (초당 공격 횟수 또는 공격 간 지연 시간)
    public int maxAmmo; // 최대 탄약 수 (원거리 무기에만 해당)
    public int curAmmo; // 현재 탄약 수 (원거리 무기에만 해당)

    // 근접 무기 관련 컴포넌트
    public BoxCollider meleeArea;     // 근접 공격 영역을 나타내는 BoxCollider (데미지 판정용)
    public TrailRenderer trailEffect; // 근접 공격 시 나타나는 트레일 효과

    // 원거리 무기 관련 컴포넌트
    public Transform bulletPos;       // 총알이 발사될 위치 Transform
    public GameObject bullet;         // 발사할 총알 프리팹
    public Transform bulletCasePos;   // 탄피가 배출될 위치 Transform
    public GameObject bulletCase;     // 배출할 탄피 프리팹

    public AudioClip attackSound;     // 공격 시 재생될 오디오 클립
    private AudioSource audioSource;  // 오디오 재생을 위한 AudioSource 컴포넌트

    // 게임 오브젝트가 처음 활성화될 때 호출됩니다. (시작 시 한 번)
    private void Awake()
    {
        // 이 오브젝트에 붙어있는 AudioSource 컴포넌트를 가져와 할당합니다.
        audioSource = GetComponent<AudioSource>();
    }

    // 무기를 사용하는 메서드 (플레이어 입력에 따라 호출될 수 있음)
    public void Use()
    {
        // 무기 종류에 따라 다른 공격 코루틴을 시작합니다.
        if (type == Type.Melee) // 근접 무기인 경우
        {
            StopCoroutine("Swing");   // 기존에 실행 중인 "Swing" 코루틴을 중지합니다.
            StartCoroutine("Swing");  // "Swing" 코루틴을 새로 시작하여 근접 공격을 수행합니다.
        }
        else if (type == Type.Range && curAmmo > 0) // 원거리 무기이고 현재 탄약이 0보다 많은 경우
        {
            curAmmo--;                // 현재 탄약 수를 1 감소시킵니다.
            StartCoroutine("Shot");   // "Shot" 코루틴을 시작하여 총알을 발사합니다.
        }
    }

    // 근접 무기의 휘두르기 동작을 처리하는 코루틴
    IEnumerator Swing()
    {
        PlaySound(); // 공격 사운드를 재생합니다.

        yield return new WaitForSeconds(0.1f); // 0.1초 대기

        meleeArea.enabled = true; // 근접 공격 영역을 활성화하여 데미지 판정을 시작합니다.
        trailEffect.enabled = true; // 트레일 효과를 활성화하여 시각적 효과를 부여합니다.

        yield return new WaitForSeconds(0.3f); // 0.3초 대기

        meleeArea.enabled = false; // 근접 공격 영역을 비활성화하여 데미지 판정을 중지합니다.

        yield return new WaitForSeconds(0.3f); // 0.3초 대기

        trailEffect.enabled = false; // 트레일 효과를 비활성화합니다.
    }

    // 원거리 무기의 발사 동작을 처리하는 코루틴
    IEnumerator Shot()
    {
        PlaySound(); // 공격 사운드를 재생합니다.

        // 총알을 bulletPos 위치와 회전으로 생성합니다.
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>(); // 생성된 총알의 Rigidbody 컴포넌트를 가져옵니다.
        bulletRigid.linearVelocity = bulletPos.forward * 50; // 총알을 bulletPos의 정면 방향으로 시속 50의 속도로 발사합니다.

        yield return null; // 다음 프레임까지 대기

        // 탄피를 bulletCasePos 위치와 회전으로 생성합니다.
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>(); // 생성된 탄피의 Rigidbody 컴포넌트를 가져옵니다.
        // 탄피에 랜덤한 방향과 힘을 가하여 배출되는 효과를 만듭니다.
        Vector3 caseVec = bulletCasePos.forward * -Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse); // 즉시 힘을 가합니다.
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 즉시 회전력(토크)을 가합니다.
    }

    // 공격 사운드를 재생하는 메서드
    void PlaySound()
    {
        // AudioSource와 attackSound가 모두 유효하면 사운드를 한 번 재생합니다.
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }
}