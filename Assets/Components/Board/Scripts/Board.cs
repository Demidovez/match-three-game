using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CellSpace;
using DatabaseSpace;
using DG.Tweening;
using RowSpace;
using ScoreSpace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BoardSpace
{
    public sealed class Board : MonoBehaviour
    {
        public static Board Instance { get; private set; }
        
        public Row[] Rows;
        public Cell[,] Cells { get; private set; }
        
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _tweenDuration = 0.25f;
        
        public int CountRows => Cells.GetLength(0); 
        public int CountColumns => Cells.GetLength(1);

        private readonly List<Cell> _selectedCells = new();
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Cells = new Cell[Rows.Length, Rows.Max(row => row.Cells.Length)];

            for (int x = 0; x < CountRows; x++)
            {
                for (int y = 0; y < CountColumns; y++)
                {
                    var cell = Rows[x].Cells[y];

                    cell.x = x;
                    cell.y = y;
                    cell.Item = Database.Items[Random.Range(0, Database.Items.Length)];
                    
                    Cells[x, y] = cell;
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

            if (CanPop())
            {
                Pop();
            }
            
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
                .Join(icon1Transform.DOMove(icon2Transform.position, _tweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, _tweenDuration))
                .Play().AsyncWaitForCompletion();
            
            icon1Transform.SetParent(cell2.transform);
            icon2Transform.SetParent(cell1.transform);

            cell1.SetData(icon2, item2);
            cell2.SetData(icon1, item1);
        }

        private bool CanPop()
        {
            for (int x = 0; x < CountRows; x++)
            {
                for (int y = 0; y < CountColumns; y++)
                {
                    if (Cells[x, y].GetConnectedCells() != null)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        private async void Pop()
        {
            for (int x = 0; x < CountRows; x++)
            {
                for (int y = 0; y < CountColumns; y++)
                {
                    var cells = Cells[x, y].GetConnectedCells();
                    
                    if(cells == null || cells.Skip(1).Count() < 2) continue;
                    
                    var deflateSequence = DOTween.Sequence();

                    foreach (var cell in cells)
                    {
                        deflateSequence.Join(cell.Icon.transform.DOScale(Vector3.zero, _tweenDuration));
                    }

                    _audioSource.PlayOneShot(_audioClip);
                    
                    Score.Instance.Value += Cells[x, y].Item.Value * cells.Count;
                    
                    await deflateSequence.Play().AsyncWaitForCompletion();
                    
                    var inflateSequence = DOTween.Sequence();
                    
                    foreach (var cell in cells)
                    {
                        cell.Item = Database.Items[Random.Range(0, Database.Items.Length)];
                        inflateSequence.Join(cell.Icon.transform.DOScale(Vector3.one, _tweenDuration));
                    }
                    
                    await inflateSequence.Play().AsyncWaitForCompletion();
                    
                    y = CountColumns;
                    x = -1;
                }
            }
        }
    }
}
