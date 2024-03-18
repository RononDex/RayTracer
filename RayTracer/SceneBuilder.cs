using System;
using System.Collections.Generic;
using System.Numerics;
using RayTracer.Materials;
using RayTracer.Models;

namespace RayTracer;

public static class SceneBuilder
{

    public static Scene CornellBox()
    {
        var eyePosition = new Vector3(0, 0, -4);
        var lookAt = new Vector3(0, 0, 6);
        var fov = 36f * (float)Math.PI / 180f;
        var scene = new Scene(new List<IGeometry> {
                        // a: Left wall (red)
                        new Sphere(new Vector3(-1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.95f, 0.1f, 0.1f))),

                        // b: Right wall (blue)
                        new Sphere(new Vector3(1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.1f, 0.1f, 0.95f))),

                        // c: Back wall (gray)
                        new Sphere(new Vector3(0, 0, 1001), 1000, new LambertMaterial(new Vector3(0.4f, 0.4f, 0.4f))),

                        // d: Bottom wall (gray)
                        new Sphere(new Vector3(0, -1001, 0), 1000, new LambertMaterial(new Vector3(0.4f, 0.4f, 0.4f))),

                        // e: Top wall (light)
                        new Sphere(new Vector3(0, 1001, 0), 1000, new LambertMaterial(new Vector3(0.8f, 0.8f, 0.8f), emissionColor: new Vector3(2f))),

                        // f: Smaller Sphere (yellow)
                        new Sphere(new Vector3(-0.6f, -0.7f, -0.6f), 0.3f, new LambertMaterial(new Vector3(0.8f, 0.8f, 0.2f))),

                        // g: Bigger sphere (cyan)
                        new Sphere(new Vector3(0.3f, -0.4f, 0.3f), 0.6f, new LambertMaterial(new Vector3(0.2f, 0.9f, 0.9f))),
                }, eyePosition, lookAt, fov, new Vector3(0f, 1f, 0f));

        return scene;
    }

    public static Scene CornellBoxReflections()
    {
        var eyePosition = new Vector3(0, 0, -4);
        var lookAt = new Vector3(0, 0, 6);
        var fov = 36f * (float)Math.PI / 180f;
        var scene = new Scene(new List<IGeometry> {
                        // a: Left wall (red)
                        new Sphere(new Vector3(-1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.95f, 0.1f, 0.1f))),

                        // b: Right wall (blue)
                        new Sphere(new Vector3(1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.1f, 0.1f, 0.95f))),

                        // c: Back wall (gray)
                        new Sphere(new Vector3(0, 0, 1001), 1000, new LambertMaterial(new Vector3(0.4f, 0.4f, 0.4f))),

                        // d: Bottom wall (gray)
                        new Sphere(new Vector3(0, -1001, 0), 1000, new LambertMaterial(new Vector3(0.4f, 0.4f, 0.4f))),

                        // e: Top wall (light)
                        new Sphere(new Vector3(0, 1001, 0), 1000, new LambertMaterial(new Vector3(0.8f, 0.8f, 0.8f), emissionColor: new Vector3(2f))),

                        // f: Smaller Sphere (yellow)
                        new Sphere(new Vector3(-0.6f, -0.7f, -0.6f), 0.3f, new ReflectiveSpecularMaterial(new Vector3(0.8f, 0.8f, 0.2f), new Vector3(0.8f))),

                        // g: Bigger sphere (cyan)
                        new Sphere(new Vector3(0.3f, -0.4f, 0.3f), 0.6f, new ReflectiveSpecularMaterial(new Vector3(0.2f, 0.9f, 0.9f), new Vector3(0.8f))),
                }, eyePosition, lookAt, fov, new Vector3(0f, 1f, 0f));

        return scene;
    }

    public static Scene CornellBoxTextured()
    {
        var eyePosition = new Vector3(0, 0, -4);
        var lookAt = new Vector3(0, 0, 6);
        var fov = 36f * (float)Math.PI / 180f;
        var scene = new Scene(new List<IGeometry> {
                        // a: Left wall (red)
                        new Sphere(new Vector3(-1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.95f, 0.1f, 0.1f))),

                        // b: Right wall (blue)
                        new Sphere(new Vector3(1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.1f, 0.1f, 0.95f))),

                        // c: Back wall (gray)
                        new Sphere(new Vector3(0, 0, 1001), 1000, new LambertMaterial(new Vector3(0.4f, 0.4f, 0.4f))),

                        // d: Bottom wall (gray)
                        new Sphere(new Vector3(0, -1001, 0), 1000, new ReflectiveSpecularMaterial(new Vector3(0.1f, 0.1f, 0.1f), new Vector3(1), reflectionMultiplier: 15)),

                        // e: Top wall (light)
                        new Sphere(new Vector3(0, 1001, 0), 1000, new LambertMaterial(new Vector3(0.8f, 0.8f, 0.8f), emissionColor: new Vector3(0.6f))),

                        // f: Smaller Sphere (black marble)
                        new Sphere(new Vector3(-0.6f, -0.7f, -0.6f), 0.3f, new TextureMaterial("Textures/close-up-black-marble-background.jpg", specularColor: new Vector3(1), specularMultiplier: 1.8f)),

                        // g: Bigger sphere (sun)
                        new Sphere(new Vector3(0.3f, -0.4f, 0.3f), 0.6f, new TextureMaterial("Textures/Sun2.jpg", pathToEmissionTexture: "Textures/Sun2.jpg", emissionMultiplier: 15f)),
                }, eyePosition, lookAt, fov, new Vector3(0f, 1f, 0f));

        return scene;
    }

    public static Scene SolarSystemScene()
    {
        var eyePosition = new Vector3(0, 0, -1);
        var lookAt = new Vector3(0, 0, 1);
        var fov = 60f * (float)Math.PI / 180f;

        var milkyWayRotation = Matrix4x4.CreateRotationZ(MathF.PI / 4 + 0.1f) * Matrix4x4.CreateRotationY(2 * MathF.PI / 2);
        var jupiterRotation = Matrix4x4.CreateRotationY(0);
        var earthRotation = Matrix4x4.CreateRotationY(MathF.PI / 2);

        var scene = new Scene(new List<IGeometry> {
                        // Milkyway bg
                        new Sphere(new Vector3(0.0f, 0.0f, 0.0f), 1000f, new TextureMaterial(null, pathToEmissionTexture: "Textures/Milkyway.jpg", textureTransformation: milkyWayRotation, emissionMultiplier: 1.0f)),

                        // Bottom table (reflective)
                        new Sphere(new Vector3(0, -1001, 0), 1000, new ReflectiveSpecularMaterial(new Vector3(0.1f, 0.1f, 0.1f), new Vector3(1), reflectionMultiplier: 150)),

                        // Sun
                        new Sphere(new Vector3(0.1f, -0.5f, 2.5f), 0.4f, new TextureMaterial("Textures/Sun2.jpg", pathToEmissionTexture: "Textures/Sun2-white.jpg", emissionMultiplier: 10f)),

                        // Jupiter
                        new Sphere(new Vector3(-1.6f, -0.6f, 3.2f), 0.4f, new TextureMaterial("Textures/8k_jupiter.jpg", textureTransformation: jupiterRotation)),

                        // Europa
                        new Sphere(new Vector3(-2.0f, -0.89f, 2.9f), 0.11f, new TextureMaterial("Textures/Europa.jpg")),

                        // Earth
                        new Sphere(new Vector3(1.8f, -0.8f, 3f), 0.2f, new TextureMaterial("Textures/8k_earth_daymap.jpg", textureTransformation: earthRotation)),

                        // Mars
                        new Sphere(new Vector3(-0.8f, -0.8f, 1.5f), 0.2f, new TextureMaterial("Textures/8k_mars.jpg")),

                        // Venus
                        new Sphere(new Vector3(0.8f, -0.8f, 2.0f), 0.2f, new TextureMaterial("Textures/8k_venus_surface.jpg")),

                        // Neptune
                        new Sphere(new Vector3(-0.9f, -0.6f, 5.0f), 0.4f, new TextureMaterial("Textures/2k_neptune.jpg")),

                }, eyePosition, lookAt, fov, new Vector3(0f, 1f, 0f));

        return scene;
    }

    public static Scene JupiterScene()
    {
        var eyePosition = new Vector3(0, 0, -1);
        var lookAt = new Vector3(0, 0, 1);
        var fov = 60f * (float)Math.PI / 180f;

        var milkyWayRotation = Matrix4x4.CreateRotationY(-0.3f) * Matrix4x4.CreateRotationZ(MathF.PI / 4 + 0.1f) * Matrix4x4.CreateRotationY(2 * MathF.PI / 2);
        var jupiterRotation = Matrix4x4.CreateRotationY(0f);
        var jupiterPos = new Vector3(-1.0f, -0.25f, 8.2f);
        var sunPos = new Vector3(25f, 0f, -15f);

        var scene = new Scene(new List<IGeometry> {
                        // Milkyway bg
                        new Sphere(new Vector3(0.0f, 0.0f, 0.0f), 1000f, new TextureMaterial(null, pathToEmissionTexture: "Textures/Milkyway.jpg", textureTransformation: milkyWayRotation, emissionMultiplier: 1.0f)),

                        // Sun
                        new Sphere(sunPos, 3.5f, new TextureMaterial("Textures/Sun2.jpg", pathToEmissionTexture: "Textures/Sun2-white.jpg", emissionMultiplier: 250f)),

                        // Jupiter
                        new Sphere(jupiterPos, 2.0f, new TextureMaterial("Textures/8k_jupiter.jpg", textureTransformation: jupiterRotation)),

                        // Europa
                        new Sphere(new Vector3(1.5f, 0.5f, 2.9f), 0.07f, new TextureMaterial("Textures/Europa.jpg")),

                        // Io
                        new Sphere(Vector3.Lerp(jupiterPos, sunPos, 0.085f), 0.1f, new TextureMaterial("Textures/2k-Io.jpg")),

                        // Haumea
                        new Sphere(new Vector3(-2.3f, -0.3f, 4f), 0.1f, new TextureMaterial("Textures/4k_haumea_fictional.jpg")),

                }, eyePosition, lookAt, fov, new Vector3(0f, 1f, 0f));

        return scene;
    }

    public static Scene CornellBoxTransparency()
    {
        var eyePosition = new Vector3(0, 0, -4);
        var lookAt = new Vector3(0, 0, 6);
        var fov = 36f * (float)Math.PI / 180f;
        var scene = new Scene(new List<IGeometry> {
                        // a: Left wall (red)
                        new Sphere(new Vector3(-1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.95f, 0.1f, 0.1f))),

                        // b: Right wall (blue)
                        new Sphere(new Vector3(1001, 0, 0), 1000, new LambertMaterial(new Vector3(0.1f, 0.1f, 0.95f))),

                        // c: Back wall (gray)
                        new Sphere(new Vector3(0, 0, 1001), 1000, new LambertMaterial(new Vector3(0.4f, 0.4f, 0.4f))),

                        // d: Bottom wall (gray)
                        new Sphere(new Vector3(0, -1001, 0), 1000, new LambertMaterial(new Vector3(0.4f, 0.4f, 0.4f))),

                        // e: Top wall (light)
                        new Sphere(new Vector3(0, 1001, 0), 1000, new LambertMaterial(new Vector3(0.8f, 0.8f, 0.8f), emissionColor: new Vector3(2f))),

                        // f: Smaller Sphere (black marble)
                        new Sphere(new Vector3(-0.6f, -0.7f, -0.3f), 0.3f, new TextureMaterial("Textures/close-up-black-marble-background.jpg", specularColor: new Vector3(1), specularMultiplier: 1.8f)),

                        // g: Bigger sphere (sun)
                        new Sphere(new Vector3(0.3f, -0.4f, 0.3f), 0.6f, new TextureMaterial("Textures/4k_haumea_fictional.jpg",emissionMultiplier: 15f)),

                        // Smaller lense sphere in front
                        new Sphere(new Vector3(0.3f, -0.8f, -0.6f), 0.2f, new TransparentMaterial(1.1f)),
                        new Sphere(new Vector3(-0.3f, -0.85f, -0.7f), 0.15f, new TransparentMaterial(1.2f)),
    }, eyePosition, lookAt, fov, new Vector3(0f, 1f, 0f))
        {
            NumberOfBounces = 10
        };

        return scene;
    }
}
