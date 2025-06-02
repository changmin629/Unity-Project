using UnityEngine;

// Bullet 클래스는 게임 내에서 발사되는 모든 총알(투사체)의 기본적인 동작을 정의합니다.
// 미사일, 바위 등 다양한 투사체가 이 클래스를 상속받아 사용할 수 있습니다.
public class Bullet : MonoBehaviour
{
    public int damage;    // 총알의 공격력 (적에게 가할 데미지)
    public bool isMelee;  // 이 총알이 근접 공격(예: 칼 휘두르기)에 의한 것인지 여부 
    public bool isRock;   // 이 총알이 보스 락(바위)인지 여부 

    // 게임 오브젝트가 처음 활성화될 때 한 번 호출됩니다.
    void Start()
    {
        // 총알이 생성된 후 5초 뒤에 자동으로 게임 오브젝트를 파괴합니다.
        // 이는 화면에 불필요하게 남아있는 총알 수를 줄여 성능을 최적화하는 데 도움을 줍니다.
        Destroy(gameObject, 5f);
    }

    // 다른 Collider와 물리적 충돌이 발생했을 때 호출됩니다.
    void OnCollisionEnter(Collision collision)
    {
        // 만약 이 총알이 '바위(isRock)'가 아니고, 충돌한 오브젝트의 태그가 "Floor" (바닥)이라면
        if (!isRock && collision.gameObject.tag == "Floor")
        {
            // 충돌 후 3초 뒤에 게임 오브젝트를 파괴합니다.
            Destroy(gameObject, 3f);
        }
    }

    // 다른 Collider의 트리거 영역에 진입했을 때 호출됩니다.
    void OnTriggerEnter(Collider other)
    {
        // 만약 이 총알이 '근접 공격(isMelee)'에 의한 것이 아니고, 충돌한 오브젝트의 태그가 "Wall" (벽)이라면
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            // 즉시 게임 오브젝트를 파괴합니다.
            // 이는 총알이 벽을 뚫고 지나가지 않도록 하여 현실감을 높입니다.
            Destroy(gameObject);
        }
    }
}
