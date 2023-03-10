using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [Min(1)]
    public int speed;

    void Start()
    {
        speed = 3;
    }


    void Update()
    {
        MoveTarget();
    }


    void MoveTarget()
    {
        float new_x = transform.position.x
            + (Input.GetAxis("Horizontal") * Time.deltaTime * speed);
        float new_y = transform.position.y
            + (Input.GetAxis("Vertical") * Time.deltaTime * speed);

        transform.position = new Vector3(
            Mathf.Clamp(new_x, -4.5f, 4.5f),
            Mathf.Clamp(new_y, 0f, 3.5f),
            transform.position.z
        ); 
    }
}
