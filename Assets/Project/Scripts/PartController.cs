using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //Si ponemos esto hace que se añada el rigidbody al asignar el script a un objeto
public class PartController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float xDegree = 0.5f;
    [SerializeField]
    private float yDegree = 0.7f;
    [SerializeField]
    private float force = 60;

    private int incX = 1;
    private int incZ = -1;

    private Rigidbody rigid;
    void Start()
    {
        //Random.InitState(77);
        rigid = this.GetComponent<Rigidbody>();
        // Debug.Log("Numero =" + Random.value);
        incX = Random.value < 0.5f ? 1 : -1;
        incZ = Random.value < 0.5f ? -1 : 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = force * Time.deltaTime * incX;
        float z = force * Time.deltaTime * incZ;
        rigid.AddForce(new Vector3(x, 0.0f, z));
        //Rotating gameObect
        rigid.AddTorque(new Vector3(xDegree, yDegree, 0));
    }

    //We have to change direction when there are a collision with a wall
    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.name) { //SW
            case "North":
                incZ = -1;
                break;
            case "South":
                incZ = 1;
                break;
            case "West":
                incX = 1;
                break;
            case "East":
                incX = -1;
                break;
        }//SW
    }
}
