using UnityEngine;

// PoisonGas 클래스는 독가스 구역의 동작을 정의합니다.
// 이 구역에 들어온 플레이어에게 지속적으로 피해를 입힙니다.
public class PoisonGas : MonoBehaviour
{
    public int duration = 5;      // 독가스 구역이 유지될 시간 (초 단위)
    public int damagePerSecond = 1; // 초당 플레이어에게 가할 데미지

    private float damageInterval = 0.5f; // 데미지를 입히는 간격 (0.5초마다 데미지)
    private float nextDamageTime = 0f;   // 다음 데미지를 입힐 시간

    // 게임 오브젝트가 처음 활성화될 때 한 번 호출됩니다.
    private void Start()
    {
        // duration 변수에 설정된 시간(예: 5초) 후에 독가스 게임 오브젝트를 파괴합니다.
        Destroy(gameObject, duration);
    }

    // 다른 Collider가 이 오브젝트의 트리거 영역 안에 머무르는 동안 계속 호출됩니다.
    private void OnTriggerStay(Collider other)
    {
        // 트리거 안에 있는 오브젝트에서 Player 컴포넌트를 가져옵니다.
        Player hp = other.GetComponent<Player>();

        // Player 컴포넌트가 존재하고, 현재 시간이 다음 데미지 시간보다 크거나 같으면
        if (hp != null && Time.time >= nextDamageTime)
        {
            // 플레이어에게 damagePerSecond 만큼의 피해를 입힙니다.
            hp.TakeDamage(damagePerSecond);
            // 이를 통해 지정된 간격마다 데미지가 적용됩니다.
            nextDamageTime = Time.time + damageInterval;
        }
    }
}