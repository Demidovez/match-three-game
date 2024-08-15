using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CellSpace;
using DatabaseSpace;
using DG.Tweening;
using RowSpace;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BoardSpace
{
    public sealed class Board : MonoBehaviour
    {
        public static Board Instance { get; private set; }
        
        public Row[] Rows;
        public Cell[,] Cells { get; private set; }
        
        public int Width => Cells.GetLength(1); 
        public int Height => Cells.GetLength(0);

        private readonly List<Cell> _selectedCells = new();
        private const float TweenDuration = 0.25f;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Cells = new Cell[Rows.Length, Rows.Max(row => row.Cells.Length)];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var cell = Rows[y].Cells[x];

                    cell.x = x;
                    cell.y = y;
                    cell.Item = Database.Items[Random.Range(0, Database.Items.Length)];
                    
                    Cells[x, y] = cell;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                foreach (var connectedCell in Cells[0,0].GetConnectedCells())
                {
                    connectedCell.Icon.transform.DOScale(1.25f, TweenDuration).Play();
                }
            }
        }

        public async void Select(Cell cell)
        {
            if (!_selectedCells.Contains(cell))
            {
                _selectedCells.Add(cell);
            }
            
            
            if (_selectedCells.Count < 2)
            {
                return;
            }

            await Swap(_selectedCells[0], _selectedCells[1]);
            
            _selectedCells.Clear();
        }

        private async Task Swap(Cell cell1, Cell cell2)
        {
            var icon1 = cell1.Icon;
            var icon2 = cell2.Icon;
            var item1 = cell1.Item;
            var item2 = cell2.Item;

            var icon1Transform = icon1.transform;
            var icon2Transform = icon2.transform;

            await DOTween.Sequence()
                .Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration))
                .Play().AsyncWaitForCompletion();
            
            icon1Transform.SetParent(cell2.transform);
            icon2Transform.SetParent(cell1.transform);

            cell1.SetData(icon2, item2);
            cell2.SetData(icon1, item1);
        }

        private void CanPop()
        {
            
        }

        private void Pop()
        {
            
        }
    }
}
