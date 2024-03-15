
using System;
using System.Numerics;
using _02_RayTracing.Models;
using _02_RayTracing.Rendering;
using NUnit.Framework;

public class BasicRendrererTests
{
		[Test]
		public void CreateEyeRay_ForStaticValues_CreatesCorrectRay()
		{
				var fov = (float)Math.PI / 2;
				var eyePosition = new Vector3(0);
				var lookAt = new Vector3(1, 0, 0);
				var testee = new BasicRenderer(eyePosition, lookAt, fov);

				var actual = testee.CreateEyeRay(eyePosition, lookAt, fov, new Vector2(0, 0));

				Assert.AreEqual(actual.Direction, Vector3.Normalize(lookAt - eyePosition));

				Console.WriteLine(testee.CreateEyeRay(eyePosition, lookAt, fov, new Vector2(-1, 0)).Direction);
		}

		[Test]
		public void FindClosestHitPoint_ForStaticValues_CreatesCorrectRay()
		{
				var fov = (float)Math.PI / 2;
				var eyePosition = new Vector3(0);
				var lookAt = new Vector3(1, 0, 0);
				var testee = new BasicRenderer(eyePosition, lookAt, fov);
				var scene = new Scene(new List<IGeometry> {
						new Sphere(new Vector3(2, 0, 0), 1f, new Vector3(0.4f))
				});

				var actual = testee.FindClosestHitPoint(scene, new Ray(eyePosition, lookAt));

				Assert.AreEqual(new Vector3(1, 0, 0), actual.Position);
		}
}
