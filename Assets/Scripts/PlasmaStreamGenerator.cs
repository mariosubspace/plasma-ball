using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaStreamGenerator : MonoBehaviour
{
	public float outerSphereRadius = 2;
	new ParticleSystem particleSystem;
	List<LineRenderer> lines = new List<LineRenderer>();

	public Mesh centerCap;

	[Header("Line")]
	public AnimationCurve lineWidthCurve;
	public Material lineMaterial;
	[GradientHDR]
	public Gradient lineColor;
	public int subdivisionCount = 8;
	public float noiseAmplitude = 0.1f;
	public float noiseScale = 1f;
	public float noiseFrequency = 1f;

	void Awake()
	{
		// Create a particle system, will be used to randomly generate
		// spawn points.
		particleSystem = gameObject.AddComponent<ParticleSystem>();

		// Set up emission shape.
		ParticleSystem.ShapeModule pshape = particleSystem.shape;
		pshape.shapeType = ParticleSystemShapeType.Mesh;
		pshape.mesh = centerCap;
		pshape.rotation = new Vector3(-90, 0, 0);
		pshape.radiusThickness = 0;

		// Set up the main particle system settings.
		ParticleSystem.MainModule pmain = particleSystem.main;
		pmain.startSpeed = 0;
		pmain.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 2f);

		// Disable particle system renderer.
		particleSystem.gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
	}

	void Update()
	{
		// Get particles (A).
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(particles);

		// Add additional plasma lines if there are less than the amount of particles.
		while (lines.Count < particles.Length)
		{
			GameObject gob = new GameObject();
			gob.transform.SetParent(transform);
			LineRenderer line = gob.AddComponent<LineRenderer>();
			line.widthCurve = lineWidthCurve;
			line.material = lineMaterial;
			line.colorGradient = lineColor;
			line.numCapVertices = 8;
			line.numCornerVertices = 8;
			lines.Add(line);
		}

		// Remove superfluous lines (really inefficient, but not really important
		// to make it efficient right now...)
		{
			int li = lines.Count - 1;
			while (lines.Count > particles.Length)
			{
				Destroy(lines[li].gameObject);
				lines.RemoveAt(li);
				--li;
			}
		}

		// Project point onto sphere from center to outer sphere (B).
		for (int i = 0; i < particles.Length; ++i)
		{
			Vector3 A = particles[i].position;
			Vector3 B = Vector3.Normalize(A) * outerSphereRadius;

			// Create line renderer (pooled) (line A to B).
			// - subdivide line so it can be perturbed later.
			LineRenderer line = lines[i];
			line.positionCount = subdivisionCount + 1;
			Vector3[] subdivisions = new Vector3[subdivisionCount + 1];
			for (int j = 0; j <= subdivisionCount; ++j)
			{
				Vector3 sPos = Vector3.Lerp(A, B, ((float)j) / subdivisionCount);
				float seed = Time.time * noiseFrequency + i + j * noiseScale;
				// Add noise if it's not the start or end-point.
				if (j != 0 && j != subdivisionCount)
				{
					sPos += new Vector3 (
					Mathf.PerlinNoise(seed, 0) * 2f- 1f,
					Mathf.PerlinNoise(seed + 1, 0) * 2f - 1f,
					Mathf.PerlinNoise(seed + 2, 0) * 2f - 1f) * noiseAmplitude;
				}
				subdivisions[j] = sPos;
			}
			line.SetPositions(subdivisions);
		}
	}
}
