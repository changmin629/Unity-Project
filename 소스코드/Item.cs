using UnityEngine;

// Item 클래스는 게임 내에서 플레이어가 획득할 수 있는 아이템의 기본적인 동작을 정의합니다.
public class Item : MonoBehaviour
{
    // 아이템의 종류를 정의하는 열거형 (Enum)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;   // 이 아이템의 종류
    public int value;   // 아이템의 가치 (예: 탄약 개수, 코인 금액, 체력 회복량 등)

    Rigidbody rigid;          // 아이템의 물리적 움직임을 제어할 Rigidbody 컴포넌트
    SphereCollider sphereCollider; // 아이템의 충돌을 감지할 SphereCollider 컴포넌트

    // 게임 오브젝트가 처음 활성화될 때 호출됩니다. (시작 시 한 번)
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();           // 이 오브젝트에 붙어있는 Rigidbody 컴포넌트를 가져와 할당합니다.
        sphereCollider = GetComponent<SphereCollider>(); // 이 오브젝트에 붙어있는 SphereCollider 컴포넌트를 가져와 할당합니다.
    }

    // 매 프레임 업데이트됩니다.
    void Update()
    {
        // 아이템 오브젝트를 Y축을 기준으로 초당 20도씩 회전시킵니다.
        // 이는 아이템이 눈에 띄게 하여 플레이어가 쉽게 인식하고 획득할 수 있도록 돕습니다.
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    // 다른 Collider와 물리적 충돌이 발생했을 때 호출됩니다.
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트의 태그가 "Floor" (바닥)이라면
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;     // Rigidbody를 키네마틱으로 설정하여 물리적 영향(중력, 충돌 등)을 받지 않게 합니다.
                                          // 이는 아이템이 바닥에 떨어진 후 굴러가지 않고 제자리에 고정되도록 합니다.
            sphereCollider.enabled = false; // SphereCollider를 비활성화하여 더 이상 충돌을 감지하지 않게 합니다.
                                            // 플레이어가 이 아이템을 주울 때 트리거를 사용하기 위함일 수 있습니다.
        }
    }
}
