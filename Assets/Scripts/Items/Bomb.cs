using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Rigidbody rb;
    private float forwardForce = 1000;
    private float upForce = 50;
    public float timer = 2;
    [SerializeField] private GameObject explosion;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * forwardForce);
        rb.AddForce(transform.up * upForce);
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (timer <= 0)
        {
            Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
