using System.Collections.Generic;
using System.Linq;
using BoardSpace;
using ItemSpace;
using UnityEngine;
using UnityEngine.UI;

namespace CellSpace
{
    public enum Direction
    {
        Horizontal,
        Vertical,
        DiagonalRight,
        DiagonalLeft,
    }
    
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
        private Cell TopLeft => (x > 0) && (y > 0) ? Board.Instance.Cells[x - 1, y - 1] : null;
        private Cell TopRight => (x > 0) && (y < Board.Instance.CountColumns - 1) ? Board.Instance.Cells[x - 1, y + 1] : null;
        private Cell BottomLeft => (x < Board.Instance.CountRows - 1) && (y > 0) ? Board.Instance.Cells[x + 1, y - 1] : null;
        private Cell BottomRight => (x < Board.Instance.CountRows - 1) && (y < Board.Instance.CountColumns - 1) ? Board.Instance.Cells[x + 1, y + 1] : null;

        private Cell[] GetNeighbours(Direction direction)
        {
            Cell[] neighbours = {};
            
            switch (direction)
            {
                case Direction.Horizontal:
                    neighbours = new [] { Right, Left };
                    break;
                case Direction.Vertical:
                    neighbours = new [] { Top, Bottom };
                    break;
                case Direction.DiagonalRight:
                    neighbours = new [] { TopRight, BottomLeft };
                    break;
                case Direction.DiagonalLeft:
                    neighbours = new [] { TopLeft, BottomRight };
                    break;
            }

            return neighbours;
        }

        private void Start()
        {
            Button.onClick.AddListener(() => Board.Instance.Select(this));
        }

        public void SetData(Image icon, Item item)
        {
            Icon = icon;
            Item = item;
        }
        
        public List<Cell> GetConnectedCells()
        {
            var directions = new[]
                { Direction.Horizontal, Direction.Vertical, Direction.DiagonalLeft, Direction.DiagonalRight };
            
            foreach (var direction in directions)
            {
                List<Cell> cells = GetConnectedCellsByDirection(direction);

                if (cells.Skip(1).Count() >= 2)
                {
                    return cells;
                }
            }

            return null;
        }

        private List<Cell> GetConnectedCellsByDirection(Direction direction, List<Cell> excluse = null)
        {
            var result = new List<Cell> { this };

            if (excluse == null)
            {
                excluse = new List<Cell> { this }; 
            } else {
                excluse.Add(this);
            }

            foreach (var neighbour in GetNeighbours(direction))
            {
                if (!neighbour || excluse.Contains(neighbour) || neighbour.Item != Item)
                {
                    continue;
                }
                
                result.AddRange(neighbour.GetConnectedCellsByDirection(direction, excluse));
            }

            return result;
        }
    }
}
