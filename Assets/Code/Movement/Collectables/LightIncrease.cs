using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Movement.Collectables
{
	internal class LightIncrease : CollectableItem
	{

		public override void SetEffect()
		{
			applyEffect += thing =>
			{
				thing.lightTransform.localScale *= 2;
			};
		}

	}
}
