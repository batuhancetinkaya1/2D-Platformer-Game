using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionControl : MonoBehaviour
{
    private void OnCollision2D(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {

        }
    }
}
