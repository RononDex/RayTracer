using System.Numerics;
using _02_RayTracing.Models;
using _02_RayTracing.Rendering;

public class SphereTests
{
		[Test]
		public void Intersects_WithTwoIntersections_ReturnsBoth()
		{
				var testee = new Sphere(new Vector3(2, 0, 0), 1f, new Vector3(0.4f));

				var actual = testee.Intersect(new Ray(new Vector3(0, 0, 0), new Vector3(1, 0, 0)));

				Assert.AreEqual(2, actual.Count);
				Console.WriteLine(actual[0].Position);
				Console.WriteLine(actual[1].Position);
				Assert.IsTrue(actual.Any(h => h.Position.Equals(new Vector3(1, 0, 0))));
				Assert.IsTrue(actual.Any(h => h.Position.Equals(new Vector3(3, 0, 0))));

				actual = testee.Intersect(new Ray(new Vector3(0, 0, 0), Vector3.Normalize(new Vector3(1, 0.1f, 0))));
		actual.ForEach(h => Console.WriteLine(h.Position));
		}
}
