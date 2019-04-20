using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public Vector3 direction;
    public float speed;


    // Use this for initialization
    void Start()
    {

    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + direction.normalized * speed, Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }
}
