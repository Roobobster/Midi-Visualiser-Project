using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField]
    private float projectileSpeed;

    [SerializeField]
    private float projectileDuration;

    private float timeAlive;

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * projectileSpeed;
        CheckLifeSpan();
        
    }

    private void CheckLifeSpan()
    {

        timeAlive += Time.deltaTime;
        if (projectileDuration < timeAlive)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

}
