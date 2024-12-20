using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public Vector2 startPosition, endPosition;

    public Mover CreateMover(float speed = 1) => new Mover(this, speed);

    void Reset()
    {
        startPosition = Vector3.left;
        endPosition = Vector3.right;
    }

    public class Mover
    {
        private readonly PatrolPath path;
        private readonly float speed;
        private float t = 0f;

        public Mover(PatrolPath path, float speed)
        {
            this.path = path;
            this.speed = speed;
        }

        public Vector2 Position
        {
            get
            {
                t += Time.deltaTime * speed;
                return Vector2.Lerp(path.startPosition, path.endPosition, Mathf.PingPong(t, 1));
            }
        }
    }
}