using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Movement.Collectables
{
	internal class PaintGun : CollectableItem
	{

		public GameObject attachedTutorial;

		public override void SetEffect()
		{
			applyEffect += thing =>
			{
				CollectableTracker.hasUnlockedPaintGun = true;
				attachedTutorial.SetActive(true);
			};
		}

	}
}
