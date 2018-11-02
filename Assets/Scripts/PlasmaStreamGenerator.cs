using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaStreamGenerator : MonoBehaviour
{
	public float outerSphereRadius = 2;
	new ParticleSystem particleSystem;
	List<LineRenderer> lines = new List<LineRenderer>();

	[Header("Line")]
	public AnimationCurve lineWidthCurve;
	public Material lineMaterial;
	public Gradient lineColor;
	public int subdivisionCount = 8;
	public float noiseAmplitude = 0.1f;
	public float noiseScale = 1f;

	void Awake()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		// Get particles (A).
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(particles);

		while (lines.Count < particles.Length)
		{
			GameObject gob = new GameObject();
			gob.transform.SetParent(transform);
			LineRenderer line = gob.AddComponent<LineRenderer>();
			line.widthCurve = lineWidthCurve;
			line.material = lineMaterial;
			line.colorGradient = lineColor;
			lines.Add(line);
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
				float seed = Time.time + i + j * noiseScale;
				sPos += new Vector3 (
					Mathf.PerlinNoise(seed, 0) * 2f- 1f,
					Mathf.PerlinNoise(seed + 1, 0) * 2f - 1f,
					Mathf.PerlinNoise(seed + 2, 0) * 2f - 1f) * noiseAmplitude;
				subdivisions[j] = sPos;
			}
			line.SetPositions(subdivisions);
		}
	}
}
