using System.Collections;
using UnityEngine;

// Weapon Ŭ������ ���� ������ ���Ǵ� ������� �⺻���� ������ �����մϴ�.
// ���� ����� ���Ÿ� ���� ��θ� �����ϸ�, ���� ��Ŀ� ���� �ٸ� ������ �����մϴ�.
public class Weapon : MonoBehaviour
{
    // ������ ������ �����ϴ� ������ (Enum)
    public enum Type { Melee, Range };
    public Type type;   // �� ������ ���� (���� �Ǵ� ���Ÿ�)
    public int damage;  // ������ ���ݷ�
    public float rate;  // ���� �ӵ� (�ʴ� ���� Ƚ�� �Ǵ� ���� �� ���� �ð�)
    public int maxAmmo; // �ִ� ź�� �� (���Ÿ� ���⿡�� �ش�)
    public int curAmmo; // ���� ź�� �� (���Ÿ� ���⿡�� �ش�)

    // ���� ���� ���� ������Ʈ
    public BoxCollider meleeArea;     // ���� ���� ������ ��Ÿ���� BoxCollider (������ ������)
    public TrailRenderer trailEffect; // ���� ���� �� ��Ÿ���� Ʈ���� ȿ��

    // ���Ÿ� ���� ���� ������Ʈ
    public Transform bulletPos;       // �Ѿ��� �߻�� ��ġ Transform
    public GameObject bullet;         // �߻��� �Ѿ� ������
    public Transform bulletCasePos;   // ź�ǰ� ����� ��ġ Transform
    public GameObject bulletCase;     // ������ ź�� ������

    public AudioClip attackSound;     // ���� �� ����� ����� Ŭ��
    private AudioSource audioSource;  // ����� ����� ���� AudioSource ������Ʈ

    // ���� ������Ʈ�� ó�� Ȱ��ȭ�� �� ȣ��˴ϴ�. (���� �� �� ��)
    private void Awake()
    {
        // �� ������Ʈ�� �پ��ִ� AudioSource ������Ʈ�� ������ �Ҵ��մϴ�.
        audioSource = GetComponent<AudioSource>();
    }

    // ���⸦ ����ϴ� �޼��� (�÷��̾� �Է¿� ���� ȣ��� �� ����)
    public void Use()
    {
        // ���� ������ ���� �ٸ� ���� �ڷ�ƾ�� �����մϴ�.
        if (type == Type.Melee) // ���� ������ ���
        {
            StopCoroutine("Swing");   // ������ ���� ���� "Swing" �ڷ�ƾ�� �����մϴ�.
            StartCoroutine("Swing");  // "Swing" �ڷ�ƾ�� ���� �����Ͽ� ���� ������ �����մϴ�.
        }
        else if (type == Type.Range && curAmmo > 0) // ���Ÿ� �����̰� ���� ź���� 0���� ���� ���
        {
            curAmmo--;                // ���� ź�� ���� 1 ���ҽ�ŵ�ϴ�.
            StartCoroutine("Shot");   // "Shot" �ڷ�ƾ�� �����Ͽ� �Ѿ��� �߻��մϴ�.
        }
    }

    // ���� ������ �ֵθ��� ������ ó���ϴ� �ڷ�ƾ
    IEnumerator Swing()
    {
        PlaySound(); // ���� ���带 ����մϴ�.

        yield return new WaitForSeconds(0.1f); // 0.1�� ���

        meleeArea.enabled = true; // ���� ���� ������ Ȱ��ȭ�Ͽ� ������ ������ �����մϴ�.
        trailEffect.enabled = true; // Ʈ���� ȿ���� Ȱ��ȭ�Ͽ� �ð��� ȿ���� �ο��մϴ�.

        yield return new WaitForSeconds(0.3f); // 0.3�� ���

        meleeArea.enabled = false; // ���� ���� ������ ��Ȱ��ȭ�Ͽ� ������ ������ �����մϴ�.

        yield return new WaitForSeconds(0.3f); // 0.3�� ���

        trailEffect.enabled = false; // Ʈ���� ȿ���� ��Ȱ��ȭ�մϴ�.
    }

    // ���Ÿ� ������ �߻� ������ ó���ϴ� �ڷ�ƾ
    IEnumerator Shot()
    {
        PlaySound(); // ���� ���带 ����մϴ�.

        // �Ѿ��� bulletPos ��ġ�� ȸ������ �����մϴ�.
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>(); // ������ �Ѿ��� Rigidbody ������Ʈ�� �����ɴϴ�.
        bulletRigid.linearVelocity = bulletPos.forward * 50; // �Ѿ��� bulletPos�� ���� �������� �ü� 50�� �ӵ��� �߻��մϴ�.

        yield return null; // ���� �����ӱ��� ���

        // ź�Ǹ� bulletCasePos ��ġ�� ȸ������ �����մϴ�.
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>(); // ������ ź���� Rigidbody ������Ʈ�� �����ɴϴ�.
        // ź�ǿ� ������ ����� ���� ���Ͽ� ����Ǵ� ȿ���� ����ϴ�.
        Vector3 caseVec = bulletCasePos.forward * -Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse); // ��� ���� ���մϴ�.
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // ��� ȸ����(��ũ)�� ���մϴ�.
    }

    // ���� ���带 ����ϴ� �޼���
    void PlaySound()
    {
        // AudioSource�� attackSound�� ��� ��ȿ�ϸ� ���带 �� �� ����մϴ�.
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }
}