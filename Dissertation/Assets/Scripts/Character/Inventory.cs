using System;
using System.Collections;
using UnityEngine;

namespace Dissertation.Character
{
	public class Inventory : MonoBehaviour
	{
		[Serializable]
		public class InventoryContents
		{
			public uint Currency = 0;

			public InventoryContents(InventoryContents other)
			{
				Currency = other.Currency;
			}

			public void Add(InventoryContents additionalContents)
			{
				if(additionalContents != null)
				{
					Currency += additionalContents.Currency;
				}
			}

			public bool IsEmpty()
			{
				return Currency == 0;
			}

			public InventoryContents Copy()
			{
				return new InventoryContents(this);
			}
		}

		[SerializeField] private GameObject _prompt;
		[SerializeField] private UnityEngine.UI.Image _progressbar;

		[SerializeField] private float _pickDuration = 4.0f;

		public InventoryContents Contents { get; private set; }

		public BaseCharacterController Owner { get; private set; }
		public bool Dropped { get; private set; } = false;

		private Vector3 _deathLocation;

		private BaseCharacterController _picker;
		private float _pickerEnterTime;
		private Coroutine _pickUp;

		public void Initialise(BaseCharacterController owner, InventoryContents initialContents, bool dropped = false)
		{
			Contents = initialContents;
			Owner = owner;
			Dropped = dropped;

			Debug.Assert(Contents != null);

			if (!Dropped)
			{
				Owner.Health.OnDied += OnDie;
				Owner.Health.OnRespawn += DropInventory;
			}
		}

		public void Add(InventoryContents additionalContents)
		{
			Contents.Add(additionalContents);
		}

		public void TransferCurrencyTo(Inventory other, int amount)
		{
			if (Contents.Currency >= amount)
			{
				other.Contents.Currency += (uint)amount;
				Contents.Currency -= (uint)amount;
			}
		}

		private void OnDestroy()
		{
			if (!Dropped && Owner != null)
			{
				Owner.Health.OnDied -= OnDie;
				Owner.Health.OnRespawn -= DropInventory;
			}
		}

		private void OnDie()
		{
			_deathLocation = Owner.transform.position;
		}

		private void DropInventory()
		{
			if(!Contents.IsEmpty())
			{
				Inventory droppedInventory = Instantiate(Owner.Config.DropInventoryPrefab, _deathLocation, Quaternion.identity).GetComponent<Inventory>();
				droppedInventory.Initialise(Owner, Contents, true);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (Dropped)
			{
				BaseCharacterController controller = collision.gameObject.GetComponent<BaseCharacterController>();
				if ( controller != null && _picker == null )
				{
					_picker = controller;
					_pickerEnterTime = Time.time;
					_prompt.SetActive(true);
					_pickUp = StartCoroutine(TryPickUp());
				}
			}
		}

		private IEnumerator TryPickUp()
		{
			if (Dropped)
			{
				float stayDuration = 0.0f;
				while (stayDuration < _pickDuration)
				{
					stayDuration = (Time.time - _pickerEnterTime);
					_progressbar.fillAmount = (stayDuration / _pickDuration);
					yield return null;
				}

				_picker.Inventory.Add(Contents);
				Destroy(gameObject);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (Dropped)
			{
				if ( _picker != null && collision.gameObject == _picker.gameObject )
				{
					_picker = null;
					_prompt.SetActive(false);
					StopCoroutine(_pickUp);
					_pickUp = null;
				}
			}
		}
	}
}