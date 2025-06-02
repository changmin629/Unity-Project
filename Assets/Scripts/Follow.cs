using UnityEngine;

// NewMonoBehaviourScript (일반적으로 Follow 등으로 명명될 수 있음) 클래스는
// 특정 Transform(target)을 따라다니는 오브젝트의 움직임을 정의합니다.
public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform target; // 이 오브젝트가 따라갈 목표 Transform (예: 플레이어, 카메라 대상)
    public Vector3 offset;   // 목표 Transform과의 상대적인 위치 차이 (오프셋)

    // 매 프레임 업데이트됩니다.
    void Update()
    {
        // 이 오브젝트의 위치를 목표(target)의 위치에 오프셋을 더한 값으로 설정합니다.
        // 이를 통해 오브젝트는 매 프레임 목표를 따라다니며, 오프셋만큼 떨어진 위치에 유지됩니다.
        transform.position = target.position + offset;
    }
}
