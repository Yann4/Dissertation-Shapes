using Dissertation.Pathfinding;
using Dissertation.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class PathToState : State
	{
		public abstract class ValidCheck
		{
			public abstract bool IsValid();
		}

		public class PathToConfig : StateConfig
		{
			public Vector3 Target;
			private ValidCheck _isValid;

			public PathToConfig(AgentController owner, Vector3 target, ValidCheck isTargetValid = null)
				: base(States.PathTo, StatePriority.Normal, owner)
			{
				Target = target;

				_isValid = isTargetValid;
			}

			public bool IsPathStillValid()
			{
				return _isValid == null || _isValid.IsValid();
			}
		}

		private PathToConfig _config;
		private List<Node> _path;
		private int _currentPathIndex = 0;
		private Node _targetNode = null;

		public PathToState(PathToConfig config) : base(config)
		{
			_config = config;
			_path = Pathfinder.GetPath(Config.Owner.transform.position, _config.Target);
			_targetNode = Pathfinder.NearestNode(Config.Owner.transform.position);
		}

		public override void OnEnable()
		{
			base.OnEnable();
			if (_currentPathIndex < _path.Count)
			{
				Vector3 owner = Config.Owner.transform.position;
				Node nearest = Pathfinder.NearestNode(owner);
				//This means that since the last time we've run the path has become invalid & we should recalculate it
				if ( (_currentPathIndex == 0 && nearest != _path[0]) || (_currentPathIndex > 0 && nearest != _path[_currentPathIndex - 1]) )
				{
					_path = Pathfinder.GetPath(owner, _config.Target);
					_currentPathIndex = 0;
				}

				Vector3 nextTarget = _path[_currentPathIndex].Position;

				float targetNextNodeDist = (_config.Target - nextTarget).sqrMagnitude;
				float ownerTargetDist = (owner - _config.Target).sqrMagnitude;
				//If we're closer to the ultimate target than the next node is
				//Don't bother with the nodes
				if ( ownerTargetDist > targetNextNodeDist )
				{
					PushMoveState(nextTarget, _path[_currentPathIndex].Platform);
				}

				_currentPathIndex++;
			}
		}

		public override bool Update()
		{
			if( !base.Update() )
			{
				return false;
			}

			//This is for when the last node in the path isn't actually the target
			//but we still want to move towards it
			MoveToState.MoveTowards(Config.Owner, _config.Target);

			return true;
		}

		private void PushMoveState( Vector3 target, Transform targetPlatform = null)
		{
			if (targetPlatform == null)
			{
				targetPlatform = Positional.GetPlatform(target);
			}

			if (targetPlatform == Positional.GetPlatform(Config.Owner.transform))
			{
				Config.Owner.PushState(new MoveToState.MoveToConfig(target, Config.Owner));
			}
			else
			{
				Config.Owner.PushState(new TraverseState.TraverseStateConfig(Config.Owner, targetPlatform));
			}
		}

		protected override bool IsValid()
		{
			if( !_config.IsPathStillValid() )
			{
				return false;
			}

			return !Positional.IsAtPosition(Config.Owner.transform, _config.Target, 2.0f);
		}
	}
}