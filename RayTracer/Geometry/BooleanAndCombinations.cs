using System.Collections.Generic;
using System.Linq;
using RayTracer.Materials;
using RayTracer.Rendering;

namespace RayTracer.Geometry;

public class BooleanAndCombinations(ISet<IGeometry> subGeometries, IMaterial material) : IRenderable
{
    private ISet<IGeometry> SubGeometries { get; set; } = subGeometries;
    public IMaterial Material { get; private set; } = material;

    public HitPoint? Intersect(Ray ray)
    {
        var allLambdas = new Dictionary<IGeometry, IntersectionLambda[]>();
        foreach (var geometry in this.SubGeometries)
        {
            allLambdas.Add(geometry, []);
            var lambdas = geometry.GetAllIntersectionLambdas(ray);
            if (lambdas.Length != 2)
            {
                return null;
            }

            allLambdas[geometry] = lambdas;
        }

        // Case 1, Ray is outside of object --> all lambdas are positive and hitpoint will be at closest lambda
        if (allLambdas.All(geometryEntry => geometryEntry.Value.All(lambda => lambda.Lambda > 0)))
        {
            var correspondingGeometry = allLambdas.Keys.First();
            var smallestLambda = allLambdas[correspondingGeometry].First();
            foreach (var entry in allLambdas)
            {
                for (var i = 0; i < entry.Value.Length; i++)
                {
                    var lambda = entry.Value[i];
                    if (lambda.Lambda < smallestLambda.Lambda)
                    {
                        smallestLambda = lambda;
                        correspondingGeometry = entry.Key;
                    }
                }
            }
            var hitPosition = ray.Origin + (smallestLambda.Lambda * ray.Direction);
            return new HitPoint(hitPosition, this, smallestLambda.SurfaceNormal);
        }

        // Case 2, Ray is inside of object --> for all spheres one lambda is negative and the other positive
        if (allLambdas.All(geometryEntry => geometryEntry.Value.Any(lambda => lambda.Lambda > 0) && geometryEntry.Value.Any(lambda => lambda.Lambda <= 0)))
        {
            var correspondingGeometry = allLambdas.Keys.First();
            var smallestLambda = allLambdas[correspondingGeometry].Single(lambda => lambda.Lambda > 0);
            foreach (var entry in allLambdas)
            {
                for (var i = 0; i < entry.Value.Length; i++)
                {
                    var lambda = entry.Value[i];
                    if (lambda.Lambda < smallestLambda.Lambda && lambda.Lambda > 0)
                    {
                        smallestLambda = lambda;
                        correspondingGeometry = entry.Key;
                    }
                }
            }
            var hitPosition = ray.Origin + (smallestLambda.Lambda * ray.Direction);
            return new HitPoint(hitPosition, this, smallestLambda.SurfaceNormal);
        }

        return null;
    }

    public IntersectionLambda[] GetAllIntersectionLambdas(Ray ray)
    {
        var allLambdas = new Dictionary<IGeometry, IntersectionLambda[]>();
        foreach (var geometry in this.SubGeometries)
        {
            allLambdas.Add(geometry, []);
            var lambdas = geometry.GetAllIntersectionLambdas(ray);
            if (lambdas.Length != 2)
            {
                return [];
            }

            allLambdas[geometry] = lambdas;
        }

        var flatMap = allLambdas.SelectMany(x => x.Value);

        // Case 1, Ray is outside of object --> all lambdas are positive and lambdas are at biggest first lambda and smallest second lambda
        if (allLambdas.All(geometryEntry => geometryEntry.Value.All(lambda => lambda.Lambda > 0)))
        {
            // TODO:
        }

        return [];
    }
}
