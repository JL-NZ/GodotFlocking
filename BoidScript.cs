using Godot;
using System;

public partial class BoidScript : Sprite2D
{
	private float _speed = 600.0f;
	
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		float deltaTime = (float)delta;
		Rect2 viewportRect = GetViewportRect();
		
		// Movement
		Position = Position + (GlobalTransform.BasisXform(Vector2.Up) * _speed * deltaTime);
		
		// Wrapping
		if (Position.X > viewportRect.Size.X)
		{
			Position = new Vector2(Position.X - viewportRect.Size.X, Position.Y);
		}
		if (Position.Y > viewportRect.Size.Y)
		{
			Position = new Vector2(Position.X, Position.Y - viewportRect.Size.Y);
		}
		if (Position.X < 0.0f)
		{
			Position = new Vector2(Position.X + viewportRect.Size.X, Position.Y);
		}
		if (Position.Y < 0.0f)
		{
			Position = new Vector2(Position.X, Position.Y + viewportRect.Size.Y);
		}
	}
}
