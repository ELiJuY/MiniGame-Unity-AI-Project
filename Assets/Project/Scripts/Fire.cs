using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    [SerializeField]
    private int timeBetweenShots = 3;
    private float timeSinceLastShot = 0;
    //Reference to prefab to be used as bullet
    [SerializeField]
    private GameObject bullet;

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLastShot >= timeBetweenShots &&
        Input.GetButtonDown("Fire1"))
        {
            Vector3 angle = bullet.transform.rotation.eulerAngles;
            Instantiate(bullet, this.transform.position,
            Quaternion.Euler(angle));
            timeSinceLastShot = 0f;
        }
        else
        {
            timeSinceLastShot += Time.deltaTime;
        }
    }
}
