using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnyMoveScript : MonoBehaviour
{
    public float rotationSpeed = 50f; // Adjust this to change rotation speed

    // Update is called once per frame
    void Update()
    {
        // Check if the up arrow key is pressed
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // Rotate the object up
            transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.right);
        }
        // Check if the down arrow key is pressed
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            // Rotate the object down
            transform.Rotate(rotationSpeed * Time.deltaTime * -Vector3.right);
        }
    }
}
