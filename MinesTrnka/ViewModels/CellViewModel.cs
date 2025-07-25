using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MinesTrnka.ViewModels;

// ViewModel pro jedno políčko (buňku) v herním poli
public class CellViewModel : INotifyPropertyChanged
{
    public int Row { get; }
    public int Column { get; }
    public bool IsMine { get; set; } // true, pokud je pod políčkem mina
    public bool IsRevealed { get; set; } // true, pokud již bylo odkryto
    public int AdjacentMines { get; private set; } // počet sousedních min

    private readonly BoardViewModel _boardViewModel;

    public ICommand ClickCommand { get; }

    // Zobrazovaný text podle stavu políčka
    public string DisplayText => !IsRevealed ? "" :
                                  IsMine ? "\uf1e2" :
                                  AdjacentMines == 0 ? "" : AdjacentMines.ToString();

    // Barva pozadí podle stavu a typu políčka
    public Color BackgroundColor
    {
        get
        {
            if (!IsRevealed)
                return Colors.LightGray; // nezakryté políčko má šedé pozadí
            if (!IsMine)
                return Colors.White; // odkryté políčko bez miny má bílé pozadí
            if (_boardViewModel.isWinEndGame)
                return Colors.LightGreen; // pokud je hra vyhraná, odkryté miny mají zelené pozadí
            return Colors.Red; // odkryté políčko s vybouchlou minou má červené pozadí
        }
    }

    // Barva textu (např. bílý text na červeném poli s minou)
    public Color TextColor => IsMine ? Colors.White : Colors.Black;

    public event PropertyChangedEventHandler? PropertyChanged;

    public CellViewModel(int row, int column, BoardViewModel boardViewModel)
    {
        Row = row;
        Column = column;
        _boardViewModel = boardViewModel;
        ClickCommand = new Command(OnClick); // příkaz po kliknutí
    }

    // Spočítá počet okolních min a uloží je
    public void UpdateAdjacentMineCount()
    {
        AdjacentMines = _boardViewModel.CountAdjacentMines(Row, Column);
    }

    // Reakce na kliknutí: odkrytí políčka
    public void OnClick()
    {
        if (IsRevealed) return;
        IsRevealed = true;
        OnPropertyChanged(nameof(IsRevealed));
        OnPropertyChanged(nameof(DisplayText));
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));

        _boardViewModel.unrevealedCellCount--;

        if (IsMine)
            _boardViewModel.EndLossGame();

        if (_boardViewModel.unrevealedCellCount == _boardViewModel.bombCount)
            _boardViewModel.EndWinGame();
    }

    // Notifikace o změně vlastnosti pro binding
    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}