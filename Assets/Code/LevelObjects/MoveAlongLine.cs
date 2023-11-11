using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongLine : MonoBehaviour
{

    public enum LineMode
    {
        REVERSE,
        REPEAT,
    }

    public LineRenderer lineToFollow;

    [Tooltip("Add a target to activate the movement once the target is activated.")]
    public Target target;

    [Tooltip("Behaviour for how the movement should act.")]
    public LineMode movementMode = LineMode.REVERSE;

    [Tooltip("How much should we offset ourselves by between 2p oints")]
    public float startOffset;

	private Vector3[] positions;

    private int index = 0;

    private bool activated = true;

    public float speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        positions = new Vector3[lineToFollow.positionCount];
        lineToFollow.GetPositions(positions);
        Vector3 startPosition = positions[(int)Mathf.Floor(startOffset) % positions.Length];
		Vector3 endPosition = positions[(int)Mathf.Ceil(startOffset) % positions.Length];
        transform.position = startPosition * (1 - (startOffset % 1)) + endPosition * (startOffset % 1);
        index = (int)Mathf.Ceil(startOffset);

		if (target != null)
        {
            activated = false;
            target.onActivate += () =>
            {
                activated = true;
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!activated)
        {
            return;
        }
        
        var targetPosition = new Vector3(positions[index].x, positions[index].y, transform.position.z); 
        transform.position = Vector3.MoveTowards(transform.position,
            targetPosition,
            speed * Time.deltaTime);

        if ((Vector2)transform.position == (Vector2)positions[index])
        {
            switch (movementMode)
            {
                case LineMode.REVERSE:
                    index = (index + 1) % positions.Length;
                    break;
                case LineMode.REPEAT:
					index = (index + 1) % positions.Length;
                    // Teleport back to the start
                    if (index == 0)
                    {
                        transform.position = new Vector3(positions[0].x, positions[0].y, transform.position.z);
                        index = 1 % positions.Length;
                    }
					break;
            }
        }
    }

    public Vector3 GetPositionDelta()
    {
        if (!activated)
        {
            return new Vector3();
        }
        var targetPosition = new Vector3(positions[index].x, positions[index].y, transform.position.z); 
        var newPosition = Vector3.MoveTowards(transform.position,
            targetPosition,
            speed * Time.deltaTime);
        return newPosition - transform.position;
    }
}
