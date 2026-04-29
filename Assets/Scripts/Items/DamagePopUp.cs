using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro damageText;

    private float lifetime = 1f;
    private float timer;

    private Vector3 velocity;

    void Awake()
    {
        damageText = GetComponent<TextMeshPro>();
    }

    public void PopNum(int damage)
    {
        damageText.text = damage.ToString();

        velocity = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(2f, 4f),
            0f
        );
    }

    void Update()
    {
        float dt = Time.deltaTime;
        velocity += new Vector3(0, -9f, 0) * dt;
        transform.position += velocity * dt;

        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }

        timer += dt;
        float t = timer / lifetime;

        Color c = damageText.color;
        c.a = Mathf.Lerp(1f, 0f, t);
        damageText.color = c;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}