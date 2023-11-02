using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Snowstorm
{
	[RequireComponent(typeof(QueryableHitboxComponent))]
	public class SnowstormTimerTrigger : MonoBehaviour
	{

		private bool activated = false;

		private void Start()
		{
			GetComponent<QueryableHitboxComponent>().onNewCollisionEnter += collision =>
			{
				if (activated)
					return;
				SnowstormTimer snowstormTimer = collision.collider.transform.parent.GetComponentInChildren<SnowstormTimer>();
				if (snowstormTimer != null)
				{
					snowstormTimer.ShowBlips(10);
					activated = true;
				}
			};
		}

	}
}
