﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character
{
	public class Inventory : MonoBehaviour
	{
		[Serializable]
		public class InventoryContents
		{
			public uint Currency = 0;

			public InventoryContents() { }

			public InventoryContents(InventoryContents other)
			{
				Currency = other.Currency;
			}

			public void Add(InventoryContents additionalContents)
			{
				if(additionalContents != null)
				{
					Currency += additionalContents.Currency;

					additionalContents.Clear();
				}
			}

			public void Clear()
			{
				Currency = 0;
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
		[SerializeField] private InventoryContents _baseContents;
		[SerializeField] private bool _placedContainer = false;

		public InventoryContents Contents { get; private set; } = new InventoryContents();

		public BaseCharacterController Owner { get; private set; }
		public bool OnGround { get; private set; } = false;

		private Vector3 _deathLocation;

		private List<BaseCharacterController> _pickers = new List<BaseCharacterController>();
		private float _pickerEnterTime;
		private Coroutine _pickUp;

		private void Start()
		{
			OnGround |= _placedContainer;
			Contents.Add(_baseContents);
		}

		public void Initialise(BaseCharacterController owner, InventoryContents initialContents, bool dropped = false)
		{
			Contents.Add(initialContents);
			Owner = owner;
			OnGround = dropped;

			Debug.Assert(Contents != null);

			if (!OnGround)
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
			if (!OnGround && Owner != null)
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
			if (OnGround && !Contents.IsEmpty())
			{
				BaseCharacterController controller = collision.gameObject.GetComponent<BaseCharacterController>();
				if ( controller != null && !_pickers.Contains(controller) )
				{
					_pickers.Add(controller);
					_pickerEnterTime = Time.time;

					if (controller is Player.PlayerController)
					{
						_prompt.SetActive(true);
					}

					if (_pickDuration != 0)
					{
						_pickUp = StartCoroutine(TryPickUp(controller));
					}
				}
			}
		}

		private void Update()
		{
			if( _pickDuration == 0 && !Contents.IsEmpty() )
			{
				foreach (BaseCharacterController picker in _pickers)
				{
					if(picker.CharacterYoke.GetButton(Input.InputAction.Interact))
					{
						picker.Inventory.Add(Contents);
						_prompt.SetActive(false);
					}
				}
			}
		}

		private IEnumerator TryPickUp(BaseCharacterController picker)
		{
			if (OnGround)
			{
				float stayDuration = 0.0f;
				while (stayDuration < _pickDuration)
				{
					stayDuration = (Time.time - _pickerEnterTime);
					_progressbar.fillAmount = (stayDuration / _pickDuration);
					yield return null;
				}

				picker.Inventory.Add(Contents);
				Destroy(gameObject);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (OnGround)
			{
				BaseCharacterController picker = _pickers.Find(p => p.gameObject == collision.gameObject);
				if ( picker != null )
				{
					_pickers.Remove(picker);

					if (picker is Player.PlayerController)
					{
						_prompt.SetActive(false);
					}

					if (_pickUp != null)
					{
						StopCoroutine(_pickUp);
						_pickUp = null;
					}
				}
			}
		}
	}
}