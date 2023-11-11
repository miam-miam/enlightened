using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Level
{
	[RequireComponent(typeof(QueryableHitboxComponent))]
	internal class TeleportTrigger : MonoBehaviour
	{

		public Vector3 delta;

		private void Start()
		{
			GetComponent<QueryableHitboxComponent>().onCollisionEnter += thing =>
			{
				if (thing.collider.tag != "Player")
					return;
				Transform current = thing.collider.transform;
				while (current.parent != null)
					current = current.parent;
				current.position += delta;
			};
		}

	}
}
