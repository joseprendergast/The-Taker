using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFoundation
{
	[DefaultExecutionOrder(-850)]
	public class GameInventory : MonoBehaviour
	{
		public static GameInventory Instance { get; private set; }

		[SerializeField] List<InventoryItemData> catalog = new List<InventoryItemData>();

		public string SelectedItemId { get; private set; }
		public IReadOnlyList<InventoryItemData> Catalog => catalog;
		public IEnumerable<InventoryItemData> OwnedItems => catalog.Where(item => item != null && GameGlobals.Instance.HasItem(item.id) && !item.hidden);

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void Collect(string itemId)
		{
			GameGlobals.Instance.CollectItem(itemId);
		}

		public void Select(string itemId)
		{
			SelectedItemId = string.IsNullOrWhiteSpace(itemId) ? null : itemId;
			GameUI.Instance?.RefreshInventory();
		}

		public void ClearSelection()
		{
			SelectedItemId = null;
			GameUI.Instance?.RefreshInventory();
		}

		public InventoryItemData GetItem(string itemId)
		{
			return catalog.FirstOrDefault(item => item != null && item.id == itemId);
		}

		public void ClearInventoryForDebug()
		{
			SelectedItemId = null;
		}
	}
}
