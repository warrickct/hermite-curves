using UnityEngine;

namespace Procedural
{
	public class Plane
	{
		public Vector3 pos;

		public Vector3 normal;

		public Plane(Vector3 pos, Vector3 normal)
		{
			this.pos = pos;

			this.normal = normal;
		}
	}
}
