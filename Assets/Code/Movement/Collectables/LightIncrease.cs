using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Movement.Collectables
{
	internal class LightIncrease : CollectableItem
	{

		public override void SetEffect()
		{
			applyEffect += thing =>
			{
				thing.StartCoroutine(LightAnimation(thing));
			};
		}

		public IEnumerator LightAnimation(CollectableTracker thing)
		{
			float lightSizeAmount = 4 / Mathf.Sqrt(thing.lightTransform.localScale.x);
			// Since multiplication is commutative, this animation can be played on top of itself multiple times.
			thing.lightTransform.localScale *= lightSizeAmount;
			double logScale = Math.Pow(1 / lightSizeAmount, 1d/900);
			float actual = lightSizeAmount;
			double desired = lightSizeAmount;
			for (int i = 0; i < 900; i++)
			{
				desired *= logScale;
				// What do we need to multiply actual by to reach desired?
				// Deal with floating point rounding inaccuracies at small values
				float multiplicationAmount = (float)desired / actual;
				actual *= multiplicationAmount;
				thing.lightTransform.localScale *= multiplicationAmount;
				yield return new WaitForSeconds(0.1f);
			}
			// Full reset
			thing.lightTransform.localScale *= (1 / actual);
		}

	}
}
