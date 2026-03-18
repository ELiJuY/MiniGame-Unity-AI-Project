using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float lifeTime = 3f;

    [SerializeField]
    private GameObject explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        //Due to bullet axis, to move bullet
        //horizontalle a force must be applied on Y axis
        GetComponent<Rigidbody>().velocity = transform.up * speed;
        Invoke("Destroy", lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
            return; //do nothing
                    //First: Cancel bullet destruction
        CancelInvoke("Destroy");
        if (collision.gameObject.tag.Equals("Enemy"))
        {//Enemy must die
            collision.gameObject.SendMessage("Die");
        }

        if (explosionEffect != null)
        {
            GameObject explosion =
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Destroy(explosion, 2f);
        }
        Destroy();
    }
}
