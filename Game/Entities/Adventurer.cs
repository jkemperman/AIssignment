﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ISGPAI.Game.Artwork;
using ISGPAI.Game.Collections;
using ISGPAI.Game.Maths;
using ISGPAI.Game.SteeringBehaviors;

namespace ISGPAI.Game.Entities
{
	internal class Adventurer : MovingEntity
	{
		// In seconds.
		private const double AnimationInterval = 0.05;

		// If this entity's speed is below this value, it will stand still
		// (instead of showing the walking animation.
		private const double AnimationSpeedThreshold = 25;

		private const double Drag = 20;
		private const double NavigationDistance = 10;

		private World _world;
		private ISteeringBehavior _keyboardSteering;
		private SeekAtSteering _seekAtSteering;
		private AnimatedSpriteSet _spriteSet;
		private IEnumerator<GraphNode> _path;

		// In seconds.
		private double _timeSinceLastAnimation;

		public Adventurer(World world)
		{
			this._keyboardSteering = new KeyboardSteering();
			this._seekAtSteering = new SeekAtSteering();
			this._spriteSet = new AnimatedSpriteSet("adventurer.png", 32, 64);
			this._world = world;
			Mass = 1;
			MaxSpeed = 400;
		}

		public override void Update(double elapsed)
		{
			if (Mouse.IsButtonDown(MouseButtons.Left))
			{
				if (_path == null)
				{
					// Calculate shortest path from current position to
					// the current mouse position and store the IEnumerator
					// of that path for future navigation.
					GraphNode nearestCurrent =
						_world.Graph.NearestNode(Position);
					GraphNode nearestDestination =
						_world.Graph.NearestNode(Mouse.Position);
					IEnumerable<GraphNode> path = new AStarAlgorithm(_world.Graph)
						.GetShortestPath(nearestCurrent, nearestDestination);
					_path = path.GetEnumerator();
				}
			}

			// Navigate to the previously clicked location.
			if (_path != null)
			{
				// Move to the next node in our path if we are close enough to
				// our current target node.
				if ((Position - _path.Current.Position).Length < NavigationDistance)
				{
					// Set path to null if we have reached our destination.
					if (!_path.MoveNext())
					{
						_path = null;
					}
				}
				_seekAtSteering.Location = _path.Current.Position;
				Vector2 steeringForce = _seekAtSteering.Steer(this, elapsed) * 2000;
				Vector2 acceleration = steeringForce / Mass;
				Velocity += acceleration * elapsed;
				Velocity = Velocity.Truncate(MaxSpeed);
				Position += Velocity * elapsed;
			}
			else // Use keyboard steering to naviate the adventurer.
			{
				Vector2 steeringForce = _keyboardSteering.Steer(this, elapsed) * 2000;
				Vector2 acceleration = steeringForce / Mass;
				Velocity += acceleration * elapsed;
				Velocity = Velocity.Truncate(MaxSpeed);
				Position += Velocity * elapsed;
				if (acceleration.Length == 0)
				{
					if (Velocity.Length - Drag > 0)
					{
						Velocity = Velocity.Truncate(Velocity.Length - Drag);
					}
					else
					{
						Velocity = new Vector2();
					}
				}
			}

			UpdateSpriteDirection();
			UpdateSpriteAnimation(elapsed);
		}

		private void UpdateSpriteDirection()
		{
			// Change the direction the sprite is facing depending on
			// the current velocity.
			if (Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
			{
				if (Velocity.X < 0)
				{
					_spriteSet.ChangeRow(2);
				}
				else
				{
					_spriteSet.ChangeRow(3);
				}
			}
			else
			{
				if (Velocity.Y < 0)
				{
					_spriteSet.ChangeRow(0);
				}
				else
				{
					_spriteSet.ChangeRow(1);
				}
			}
		}

		private void UpdateSpriteAnimation(double elapsed)
		{
			if (Velocity.Length > AnimationSpeedThreshold)
			{
				double timeFactor = Velocity.Length / MaxSpeed;
				if (_timeSinceLastAnimation * timeFactor > AnimationInterval)
				{
					_timeSinceLastAnimation = 0;
					_spriteSet.AdvanceAnimation();
				}
				else
				{
					_timeSinceLastAnimation += elapsed;
				}
			}
			else
			{
				_spriteSet.ChangeColumn(1);
			}
		}

		public override void Paint(Graphics g)
		{
			_spriteSet.PaintAt(g, this.Position);
		}
	}
}
