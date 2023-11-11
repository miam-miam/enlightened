using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.GlobalEvents
{
	public static class GlobalEvents
	{

		public static GlobalEvent<Vector3> onPlayerDeath = new();

	}
}
