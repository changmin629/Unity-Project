using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BossRock 클래스는 Bullet 클래스를 상속받아 보스 몬스터가 발사하는 바위의 특수 동작을 정의합니다.
public class BossRock : Bullet
{
    Rigidbody rigid;    // 바위의 물리적 움직임을 제어할 Rigidbody 컴포넌트
    float angularPower = 2; // 바위의 회전력 (초기값 2)
    float scaleValue = 0.1f;  // 바위의 크기 (초기값 0.1)
    bool isShoot;       // 바위가 발사될 준비가 되었는지 여부를 나타내는 플래그

    // 게임 오브젝트가 처음 활성화될 때 호출됩니다. (시작 시 한 번)
    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // 이 오브젝트에 붙어있는 Rigidbody 컴포넌트를 가져와 할당합니다.
        StartCoroutine(GainPowerTimer()); // 특정 시간 후 발사 준비 완료를 알리는 코루틴 시작
        StartCoroutine(GainPower());      // 발사 전까지 바위의 힘을 축적하고 크기를 키우는 코루틴 시작
    }

    // 일정 시간 후 바위가 발사 준비가 되었음을 알리는 타이머 코루틴
    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f); // 2.2초 대기
        isShoot = true; // 2.2초가 지나면 isShoot 플래그를 true로 설정하여 발사 준비 완료
    }

    // 바위의 힘을 축적하고 크기를 키우는 코루틴
    IEnumerator GainPower()
    {
        // isShoot이 true가 될 때까지 반복합니다.
        while (!isShoot)
        {
            angularPower += 0.05f; // 회전력을 점진적으로 증가시킵니다.
            scaleValue += 0.003f;  // 크기 값을 점진적으로 증가시킵니다.
            transform.localScale = Vector3.one * scaleValue; // 바위의 로컬 스케일을 scaleValue에 맞춰 조절하여 크기를 키웁니다.
            // 바위에 회전력(토크)을 가하여 회전시킵니다. (transform.right 방향으로 angularPower만큼 가속)
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null; // 다음 프레임까지 대기
        }
    }
}