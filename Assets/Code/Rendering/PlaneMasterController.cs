using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Rendering
{
	[CreateAssetMenu(fileName = "PlaneMasterController", menuName = "Plane Master Controller", order = 1)]
	public class PlaneMasterController : ScriptableObject
	{

		[Tooltip("Anything that doesn't specify a plane will be drawn to this plane.")]
		public PlaneMaster defaultPlane;

		[Tooltip("A list of all the planes to create renderers for.")]
		public PlaneMaster[] planes;

		[Tooltip("Material to render 2 render textures multiplicatively.")]
		public Material multiplicativeMaterial;

		[Tooltip("Material to render 2 render textures normally.")]
		public Material defaultMaterial;

		[Tooltip("Material to render 2 render textures additively.")]
		public Material additiveMaterial;

		[Tooltip("Mateiral to render 2 render textures using an alpha mask technique.")]
		public Material alphaMaskMaterial;

	}
}
