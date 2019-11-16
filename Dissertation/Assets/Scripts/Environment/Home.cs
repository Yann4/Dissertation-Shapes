using Dissertation.Character;
using UnityEngine;

namespace Dissertation.Environment
{
	public class Home : MonoBehaviour
	{
		[SerializeField] private Vector2 _size;
		[SerializeField] private Spawner _associatedSpawner;

		private Vector2 Size { get { return _size; } set { _size = value; RecalculateBounds(); } }
		private Vector3 Centre { get { return transform.position; } set { transform.position = value; RecalculateBounds(); } }

		private Bounds _bounds;
		private BaseCharacterController _owner;

		private void Start()
		{
			RecalculateBounds();

			if(_associatedSpawner != null)
			{
				_associatedSpawner.OnSpawnNonStatic += OnSpawnedAssociatedCharacter;
			}
		}

		private void OnSpawnedAssociatedCharacter(BaseCharacterController spawned)
		{
			_owner = spawned;
			_owner.CharacterHome = this;
		}

		private void RecalculateBounds()
		{
			_bounds = new Bounds(Centre, _size);
		}

		public bool Contains(Vector3 position)
		{
			return _bounds.Contains(position);
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(Centre, _size);
		}
#endif //UNITY_EDITOR
	}
}