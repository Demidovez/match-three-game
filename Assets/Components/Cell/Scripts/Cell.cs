using System;
using System.Collections.Generic;
using BoardSpace;
using ItemSpace;
using UnityEngine;
using UnityEngine.UI;

namespace CellSpace
{
    public sealed class Cell : MonoBehaviour
    {
        public int x;
        public int y;
        
        public Image Icon;
        public Button Button;
        
        private Item _item;

        public Item Item
        {
            get => _item;

            set
            {
                if(_item == value) return;

                _item = value;
                Icon.sprite = _item.Sprite;
            }
        }

        private Cell Left => y > 0 ? Board.Instance.Cells[x, y - 1] : null;
        private Cell Top => x > 0 ? Board.Instance.Cells[x - 1, y] : null;
        private Cell Right => y < Board.Instance.CountColumns - 1 ? Board.Instance.Cells[x, y + 1] : null;
        private Cell Bottom => x < Board.Instance.CountRows - 1 ? Board.Instance.Cells[x + 1, y] : null;

        public Cell[] Neighbours => new[]
        {
            Left,
            Right,
            Top,
            Bottom
        };

        private void Start()
        {
            Button.onClick.AddListener(() => Board.Instance.Select(this));
        }

        public void SetData(Image icon, Item item)
        {
            Icon = icon;
            Item = item;
        }

        public List<Cell> GetConnectedCells(List<Cell> excluse = null)
        {
            var result = new List<Cell> { this };

            if (excluse == null)
            {
                excluse = new List<Cell> { this }; 
            } else {
                excluse.Add(this);
            }

            foreach (var neighbour in Neighbours)
            {
                if (!neighbour || excluse.Contains(neighbour) || neighbour.Item != Item)
                {
                    continue;
                }
                
                result.AddRange(neighbour.GetConnectedCells(excluse));
            }

            return result;
        }
    }
}
