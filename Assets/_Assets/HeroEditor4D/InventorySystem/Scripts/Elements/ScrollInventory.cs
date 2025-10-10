using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor4D.Common.Scripts.Common;
using Assets.HeroEditor4D.InventorySystem.Scripts.Data;
using Assets.HeroEditor4D.InventorySystem.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.HeroEditor4D.InventorySystem.Scripts.Elements
{
    /// <summary>
    /// Scrollable item container that can display item list. Automatic vertical scrolling.
    /// </summary>
    public class ScrollInventory : ItemContainer
    {
        [Tooltip("Sort items automatically using SortingFunc (can be redefined).")]
        public bool AutoSorting;
        [Tooltip("Add an extra empty row or a column at the end.")]
        public bool Extend;
        
        [Header("UI")]
        public ScrollRect ScrollRect;
        public GridLayoutGroup Grid;
        public InventoryItem ItemPrefab;

        public Func<Item, int> SortingFunc = item => TypePriority.IndexOf(item.Params.Type); // You can override this.
        public Func<Item, bool> FilterFunc; // You can override this.

        public Action OnRefresh;

		#if TAP_HEROES

        public TMPro.TextMeshProUGUI Gold;

        #endif

		private static readonly List<ItemType> TypePriority = new List<ItemType>
        {
            ItemType.Currency,
            ItemType.Container,
            ItemType.Booster,
            ItemType.Supply,
            ItemType.Weapon,
            ItemType.Helmet,
            ItemType.Armor,
            ItemType.Vest,
            ItemType.Bracers,
            ItemType.Leggings,
            ItemType.Shield,
            ItemType.Fragment,
            ItemType.Backpack,
            ItemType.Jewelry,
            ItemType.Loot,
            ItemType.Recipe,
            ItemType.Material
        };
        private readonly List<InventoryItem> _itemInstances = new List<InventoryItem>(); // Reusing instances to reduce Instantiate() calls.

        public void Initialize(ref List<Item> items, Item selected, bool reset = false)
        {
            base.Initialize(ref items, selected);
		}

        public void Initialize(ref List<Item> items)
        {
            base.Initialize(ref items);
            ResetNormalizedPosition();
        }

        public void SelectItem(Item item)
        {
            _itemInstances.FirstOrDefault(i => i.Item == item)?.Select(true);
        }

        public bool SelectAny()
        {
            var any = _itemInstances.FirstOrDefault(i => i.Item != null);

            if (any == null) return false;

            any.Select(true);

            return true;
        }

		public void SetTypeFilter(string input)
        {
            var type = input.ToEnum<ItemType>();

			SetTypeFilter(new List<ItemType> { type });
        }

		public void SetTypeFilter(List<ItemType> types)
        {
            FilterFunc = item => types.Contains(item.Params.Type);
			Refresh(null);
        }

        public void UnsetFilter()
        {
            FilterFunc = null;
            Refresh(null);
        }

        public override void Refresh(Item selected)
		{
            if (Items == null) return;

            List<Item> items;

            if (AutoSorting && SortingFunc != null)
            {
                items = new List<Item>();

                var groups = Items.OrderBy(SortingFunc).ToList().GroupBy(i => i.Params.Type);
                
                foreach (var group in groups)
                {
                    items.AddRange(group.OrderBy(i => i.Params.Class).ThenBy(i => i.Params.Price));
                }
            }
            else
            {
                items = Items.ToList();
            }

            if (FilterFunc != null)
            {
                items.RemoveAll(i => !FilterFunc(i));
			}

            foreach (var instance in _itemInstances)
            {
                instance.Reset();
                instance.SetActive(false);
            }

            var toggleGroup = GetComponentInParent<ToggleGroup>(includeInactive: true);

            for (var i = 0; i < items.Count; i++)
            {
                var instance = GetItemInstance();

                instance.transform.SetSiblingIndex(i);
                instance.Initialize(items[i], toggleGroup);
                instance.Count.SetActive(Stacked);

                if (AutoSelect) instance.Select(items[i] == selected);
            }

            var columns = 0;
		    var rows = 0;

		    switch (Grid.constraint)
		    {
			    case GridLayoutGroup.Constraint.FixedColumnCount:
			    {
				    var height = Mathf.FloorToInt((ScrollRect.GetComponent<RectTransform>().rect.height + Grid.spacing.y) / (Grid.cellSize.y + Grid.spacing.y));

				    columns = Grid.constraintCount;
				    rows = Mathf.Max(height, Mathf.FloorToInt((float) items.Count / columns));

                    if (Extend) rows++;

					break;
			    }
			    case GridLayoutGroup.Constraint.FixedRowCount:
			    {
				    var width = Mathf.FloorToInt((ScrollRect.GetComponent<RectTransform>().rect.width + Grid.spacing.x) / (Grid.cellSize.x + Grid.spacing.x));

				    rows = Grid.constraintCount;
				    columns = Mathf.Max(width, Mathf.FloorToInt((float) items.Count / rows));

                    if (Extend) columns++;

                    break;
			    }
		    }

		    for (var i = items.Count; i < columns * rows; i++)
		    {
                var instance = GetItemInstance();

                instance.Initialize(null);
            }
	        
            OnRefresh?.Invoke();

			#if TAP_HEROES

            var gold = Items.Where(i => i.Id == "Gold").Sum(i => i.Count);

			Gold?.SetText($"{gold} <sprite=0>");

            TapHeroes.Scripts.Interface.Elements.ItemComparer.Compare(_itemInstances);

            #endif
        }

        private InventoryItem GetItemInstance()
        {
            var instance = _itemInstances.FirstOrDefault(i => !i.gameObject.activeSelf);

            if (instance == null)
            {
                instance = Instantiate(ItemPrefab, Grid.transform);
                _itemInstances.Add(instance);
            }
            else
            {
                instance.gameObject.SetActive(true);
            }

            return instance;
        }

        public InventoryItem FindItem(Item item)
        {
            return _itemInstances.SingleOrDefault(i => i.gameObject.activeSelf && i.Item != null && i.Item.Hash == item.Hash);
        }

        public InventoryItem FindItem(string itemId)
        {
            return _itemInstances.SingleOrDefault(i => i.gameObject.activeSelf && i.Item != null && i.Item.Id == itemId);
        }

        public void ResetNormalizedPosition()
        {
            if (ScrollRect.horizontal) ScrollRect.horizontalNormalizedPosition = 0;
            if (ScrollRect.vertical) ScrollRect.verticalNormalizedPosition = 1;
        }

        public IEnumerator SnapTo(RectTransform target, bool horizontal = true, bool vertical = true)
        {
            yield return null;

            Canvas.ForceUpdateCanvases();

            var pos = (Vector2) ScrollRect.transform.InverseTransformPoint(ScrollRect.content.position) - (Vector2) ScrollRect.transform.InverseTransformPoint(target.position);

            if (!horizontal) pos.x = ScrollRect.content.anchoredPosition.x;
            if (!vertical) pos.y = ScrollRect.content.anchoredPosition.y;

            ScrollRect.content.anchoredPosition = pos;
        }
    }
}