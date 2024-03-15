using System.Numerics;

namespace _02_RayTracing.Rendering;

public class Ray
{

		public Vector3 Origin { get; private set; }
		public Vector3 Direction { get; private set; }

		public Ray(Vector3 origin, Vector3 direction)
		{
				this.Origin = origin;
				this.Direction = direction;
		}
}
