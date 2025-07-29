using Godot;
using System;
using System.Collections.Generic;

public partial class MainScene : Node2D
{
	private PackedScene boidScene;
	private int numBoids = 300;
	private List<BoidScript> boids;
	private float presenceRadius = 100.0f;
	private float personalSpaceRadius = 35.0f;
	private float coherenceMultiplier = 0.3f;
	private float alignmentMultiplier = 0.3f;
	private float separationMultiplier = 2f;
	private bool drawDebug = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		boids = new List<BoidScript>();
		boidScene = GD.Load<PackedScene>("res://Boid.tscn");
		for (int i = 0; i < numBoids; i++)
		{
			SpawnBoid();
		}
	}

	public override void _Draw()
	{
		base._Draw();

		if (!drawDebug) return;
		
		if (boids.Count != numBoids) return;

		BoidScript focusedBoid = boids[0];
		DrawCircle(focusedBoid.GlobalPosition, presenceRadius, new Color(100, 100, 100, 0.5f));
		DrawCircle(focusedBoid.GlobalPosition, personalSpaceRadius, new Color(200, 200, 200, 0.5f));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float deltaTime = (float) delta;
		QueueRedraw();
		
		foreach (var boid in boids)
		{
			List<BoidScript> localFlockmates = new List<BoidScript>();
			Vector2 localSumPosition = Vector2.Zero;
			Vector2 closeSumPosition = Vector2.Zero;
			float localSumRotation = 0.0f;
			Vector2 forwardVector = boid.GlobalTransform.BasisXform(Vector2.Up);
			
			foreach (var otherBoid in boids)
			{
				// Ignore self
				if (boid == otherBoid) continue;
				
				// Distance check
				if (boid.Position.DistanceTo(otherBoid.Position) > presenceRadius) continue;
				
				Vector2 directionToOtherBoid = otherBoid.Position - boid.Position;
				float angle = Mathf.RadToDeg(forwardVector.AngleTo(directionToOtherBoid));
				
				// FoV check
				if (angle > 120.0f && angle < 240.0f) continue;
				
				localFlockmates.Add(otherBoid);
				localSumPosition += otherBoid.Position;
				localSumRotation += otherBoid.GetRotationDegrees();
				
				// Close distance check
				if (boid.Position.DistanceTo(otherBoid.Position) > personalSpaceRadius) continue;
				
				closeSumPosition += otherBoid.Position;
			}
			
			if (localFlockmates.Count == 0) continue;
			
			Vector2 averagePosition = localSumPosition / localFlockmates.Count;
			float averageRotation = localSumRotation / localFlockmates.Count;
			Vector2 closeAveragePosition = closeSumPosition / localFlockmates.Count;

			if (closeSumPosition != Vector2.Zero)
			{
				Vector2 separationDirection = (closeAveragePosition - boid.Position) * -1;
				float separationDeltaAngle = Mathf.RadToDeg(forwardVector.AngleTo(separationDirection)); 
				float separationSteeringAmount = separationDeltaAngle * separationMultiplier * deltaTime;
				float separationSteeredRotation = boid.GetRotationDegrees() + separationSteeringAmount;
				boid.SetRotationDegrees(separationSteeredRotation);
			}
			else
			{
				Vector2 coherenceDirection = averagePosition - boid.Position;
				float coherenceDeltaAngle = Mathf.RadToDeg(forwardVector.AngleTo(coherenceDirection));
				float coherenceSteeringAmount = coherenceDeltaAngle * coherenceMultiplier * deltaTime;
				float coherenceSteeredRotation = boid.GetRotationDegrees() + coherenceSteeringAmount;

				float alignmentDeltaAngle = averageRotation - boid.GetRotationDegrees();
				float alignmentSteeringAmount = alignmentDeltaAngle * alignmentMultiplier * deltaTime;
				float alignmentSteeredRotation = coherenceSteeredRotation + alignmentSteeringAmount;
				boid.SetRotationDegrees(alignmentSteeredRotation);
			}
		}
	}

	private void SpawnBoid()
	{
		BoidScript boidInstance = boidScene.Instantiate<BoidScript>();
		boidInstance.Position = new Vector2(GetViewportRect().Size.X * GD.Randf(), GetViewportRect().Size.Y * GD.Randf());
		boidInstance.Scale = new Vector2(0.2f, 0.2f);
		boidInstance.SetRotationDegrees(GD.Randf() * 50.0f);
		AddChild(boidInstance);
		boids.Add(boidInstance);
	}

}
