using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 컴포넌트를 사용하기 위한 네임스페이스
using TMPro;          // TextMeshProUGUI를 사용하기 위한 네임스페이스

// Shop 클래스는 게임 내 상점의 동작을 정의합니다.
// 플레이어가 상점에 진입하고 아이템을 구매하는 로직을 처리합니다.
public class Shop : MonoBehaviour
{
    public RectTransform uiGroup; // 상점 UI 그룹의 RectTransform (UI 활성화/비활성화 및 위치 조절)
    public Animator anim;         // 상점 NPC 또는 상점 UI의 애니메이터 컴포넌트

    public GameObject[] itemObj;  // 상점에서 판매하는 아이템 프리팹 배열
    public int[] itemPrice;       // 각 아이템의 가격 배열 (itemObj와 인덱스 매칭)
    public Transform[] itemPos;   // 아이템이 생성될 위치 Transform 배열
    public Text talkText;         // 상점 NPC의 대화 텍스트 (UI Text 컴포넌트)

    public AudioClip buySound;    // 아이템 구매 시 재생될 오디오 클립
    private AudioSource audioSource; // 오디오 재생을 위한 AudioSource 컴포넌트

    Player enterPlayer;           // 현재 상점에 진입한 플레이어 참조

    // 게임 오브젝트가 처음 활성화될 때 호출됩니다. (시작 시 한 번)
    void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // 이 오브젝트에 붙어있는 AudioSource 컴포넌트를 가져와 할당합니다.
        // 만약 AudioSource 컴포넌트가 없으면 새로 추가합니다.
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // 플레이어가 상점에 진입했을 때 호출됩니다.
    public void Enter(Player player)
    {
        enterPlayer = player; // 상점에 진입한 플레이어를 enterPlayer 변수에 저장합니다.
        uiGroup.anchoredPosition = Vector3.zero; // 상점 UI 그룹의 위치를 화면 중앙으로 설정하여 UI를 활성화합니다.
    }

    // 플레이어가 상점을 나갔을 때 호출됩니다.
    public void Exit()
    {
        anim.SetTrigger("doHello"); // 상점 NPC 또는 UI의 "doHello" 애니메이션 트리거를 활성화합니다.
        uiGroup.anchoredPosition = Vector3.down * 1000; // 상점 UI 그룹을 화면 밖으로 이동시켜 비활성화합니다.
    }

    // 아이템 구매 로직을 처리합니다.
    // index는 구매할 아이템의 배열 인덱스를 나타냅니다.
    public void Buy(int index)
    {
        int price = itemPrice[index]; // 구매하려는 아이템의 가격을 가져옵니다.

        // 플레이어의 코인이 아이템 가격보다 적으면 구매를 취소합니다.
        if (price > enterPlayer.coin)
        {
            return;
        }

        enterPlayer.coin -= price; // 플레이어의 코인에서 아이템 가격을 차감합니다.

        // 구매 사운드가 존재하고 AudioSource가 유효하면 사운드를 한 번 재생합니다.
        if (buySound != null && audioSource != null)
            audioSource.PlayOneShot(buySound);

        // 아이템이 생성될 때 약간의 랜덤한 위치 오프셋을 계산합니다.
        // 이는 아이템들이 겹치지 않고 자연스럽게 떨어지도록 합니다.
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        // 선택된 아이템을 지정된 위치에 생성하고 랜덤 오프셋을 적용합니다.
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }
}