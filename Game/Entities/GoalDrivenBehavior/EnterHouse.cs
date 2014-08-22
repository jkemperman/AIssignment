﻿using System;

namespace ISGPAI.Game.Entities.GoalDrivenBehavior
{
	public class EnterHouse : Goal<Helper>
	{
		public EnterHouse(Helper owner)
		{
			this._owner = owner;
		}

		public override void Activate()
		{
			_status = Status.Active;
		}

		public override Status Process()
		{
			Activate();
			_owner.StopMovement();
			_owner.IsVisible = false;
			_status = Status.Completed;
			return _status;
		}

		public override void Terminate()
		{
		}

		public override void AddSubGoal(Goal<Helper> goal)
		{
			throw new InvalidOperationException(
				"Cannot add subgoals to an atomic goal");
		}
	}
}
