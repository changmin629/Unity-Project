using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ������Ʈ�� ����ϱ� ���� ���ӽ����̽�
using TMPro;          // TextMeshProUGUI�� ����ϱ� ���� ���ӽ����̽�

// Shop Ŭ������ ���� �� ������ ������ �����մϴ�.
// �÷��̾ ������ �����ϰ� �������� �����ϴ� ������ ó���մϴ�.
public class Shop : MonoBehaviour
{
    public RectTransform uiGroup; // ���� UI �׷��� RectTransform (UI Ȱ��ȭ/��Ȱ��ȭ �� ��ġ ����)
    public Animator anim;         // ���� NPC �Ǵ� ���� UI�� �ִϸ����� ������Ʈ

    public GameObject[] itemObj;  // �������� �Ǹ��ϴ� ������ ������ �迭
    public int[] itemPrice;       // �� �������� ���� �迭 (itemObj�� �ε��� ��Ī)
    public Transform[] itemPos;   // �������� ������ ��ġ Transform �迭
    public Text talkText;         // ���� NPC�� ��ȭ �ؽ�Ʈ (UI Text ������Ʈ)

    public AudioClip buySound;    // ������ ���� �� ����� ����� Ŭ��
    private AudioSource audioSource; // ����� ����� ���� AudioSource ������Ʈ

    Player enterPlayer;           // ���� ������ ������ �÷��̾� ����

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� ȣ��˴ϴ�. (���� �� �� ��)
    void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // �� ������Ʈ�� �پ��ִ� AudioSource ������Ʈ�� ������ �Ҵ��մϴ�.
        // ���� AudioSource ������Ʈ�� ������ ���� �߰��մϴ�.
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // �÷��̾ ������ �������� �� ȣ��˴ϴ�.
    public void Enter(Player player)
    {
        enterPlayer = player; // ������ ������ �÷��̾ enterPlayer ������ �����մϴ�.
        uiGroup.anchoredPosition = Vector3.zero; // ���� UI �׷��� ��ġ�� ȭ�� �߾����� �����Ͽ� UI�� Ȱ��ȭ�մϴ�.
    }

    // �÷��̾ ������ ������ �� ȣ��˴ϴ�.
    public void Exit()
    {
        anim.SetTrigger("doHello"); // ���� NPC �Ǵ� UI�� "doHello" �ִϸ��̼� Ʈ���Ÿ� Ȱ��ȭ�մϴ�.
        uiGroup.anchoredPosition = Vector3.down * 1000; // ���� UI �׷��� ȭ�� ������ �̵����� ��Ȱ��ȭ�մϴ�.
    }

    // ������ ���� ������ ó���մϴ�.
    // index�� ������ �������� �迭 �ε����� ��Ÿ���ϴ�.
    public void Buy(int index)
    {
        int price = itemPrice[index]; // �����Ϸ��� �������� ������ �����ɴϴ�.

        // �÷��̾��� ������ ������ ���ݺ��� ������ ���Ÿ� ����մϴ�.
        if (price > enterPlayer.coin)
        {
            return;
        }

        enterPlayer.coin -= price; // �÷��̾��� ���ο��� ������ ������ �����մϴ�.

        // ���� ���尡 �����ϰ� AudioSource�� ��ȿ�ϸ� ���带 �� �� ����մϴ�.
        if (buySound != null && audioSource != null)
            audioSource.PlayOneShot(buySound);

        // �������� ������ �� �ణ�� ������ ��ġ �������� ����մϴ�.
        // �̴� �����۵��� ��ġ�� �ʰ� �ڿ������� ���������� �մϴ�.
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        // ���õ� �������� ������ ��ġ�� �����ϰ� ���� �������� �����մϴ�.
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }
}