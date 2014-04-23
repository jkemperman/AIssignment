﻿using System;
using ISGPAI.Game.Entities;
using ISGPAI.Game.Maths;

namespace ISGPAI.Game.SteeringBehaviors
{
	/// <summary>
	/// Steering behavior that steers towards a target, but slows down when close.
	/// </summary>
	internal class ArriveSteering : ISteeringBehavior
	{
		private Entity _target;

		public ArriveSteering(Entity target)
		{
			this._target = target;
		}

		public Vector2 Steer(MovingEntity agent, double elapsed)
		{
			const double Deceleration = 1;

			Vector2 toTarget = _target.Position - agent.Position;
			double distance = toTarget.Length;
			if (distance > 0)
			{
				double speed = distance / Deceleration;
				speed = Math.Min(speed, agent.MaxSpeed);
				Vector2 desiredVelocity = toTarget * speed / distance;
				return desiredVelocity - agent.Velocity;
			}

			// We're ON our target... Don't move.
			return new Vector2(0, 0);
		}
	}
}