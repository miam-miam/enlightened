using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Rendering
{
	public enum PlaneDrawMode
	{
		/// <summary>
		/// Simply draw on top of the layer underneath
		/// </summary>
		DEFAULT,
		/// <summary>
		/// Draw additively on top of the layer below
		/// </summary>
		ADDITIVE,
		/// <summary>
		/// Draw multiplicatively on top of the layer below.
		/// </summary>
		MULTIPLICATIVE,
		/// <summary>
		/// The incomming plane will be used as an alpha mask for the layer below.
		/// </summary>
		ALPHA_MASK,
	}
}
