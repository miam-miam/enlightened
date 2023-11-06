using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongLine : MonoBehaviour
{
    public LineRenderer lineToFollow;

    private Vector3[] positions;

    private int index = 0;

    public float speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        positions = new Vector3[lineToFollow.positionCount];
        lineToFollow.GetPositions(positions);
        transform.position = new Vector3(positions[0].x, positions[0].y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        var targetPosition = new Vector3(positions[index].x, positions[index].y, transform.position.z); 
        transform.position = Vector3.MoveTowards(transform.position,
            targetPosition,
            speed * Time.deltaTime);

        if ((Vector2)transform.position == (Vector2)positions[index])
        {
            index = (index + 1) % positions.Length;
        }
    }

    public Vector3 GetPositionDelta()
    {
        var targetPosition = new Vector3(positions[index].x, positions[index].y, transform.position.z); 
        var newPosition = Vector3.MoveTowards(transform.position,
            targetPosition,
            speed * Time.deltaTime);
        return newPosition - transform.position;
    }
}
