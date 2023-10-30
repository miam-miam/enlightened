using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QueryableHitboxComponent))]
public class DisplayMessageTrigger : MonoBehaviour
{

	[Tooltip("The big message")]
	public string titleMessage;

	[Tooltip("The small text that displays under the title")]
	public string smallMessage;

    [Tooltip("How long should the message show for?")]
    public float time = 20;

	private bool triggered = false;

	// Start is called before the first frame update
	void Start()
    {
        GetComponent<QueryableHitboxComponent>().onNewCollisionEnter += (collision) =>
        {
            if (triggered)
                return;
			triggered = true;
			FindObjectOfType<CinematicText>().DisplayText(titleMessage, smallMessage, time);
        };
    }
}
