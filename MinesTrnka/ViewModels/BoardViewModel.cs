using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MinesTrnka.ViewModels;

// ViewModel pro celé hrací pole (správa gridu a políček)
public class BoardViewModel : INotifyPropertyChanged
{
    private readonly Grid _grid; // odkaz na mřížku v UI
    public ICommand NewGameCommand { get; } // příkaz pro spuštění nové hry
    private CellViewModel[,] _cellViewModelBoard = new CellViewModel[1,1]; // 2D pole jednotlivých políček

    private const int MineChancePercent = 15; // ~15 % políček bude minových

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public bool isWinEndGame;

    private string _rows = "15"; // výchozí počet řádků
    public string Rows
    {
        get
        {
            return _rows;
        }
        set
        {
            _rows = value;
            OnPropertyChanged();
        }
    }

    private string _columns = "15"; // výchozí počet řádků
    public string Columns
    {
        get
        {
            return _columns;
        }
        set
        {
            _columns = value;
            OnPropertyChanged();
        }
    }

    private string buttonText = "Zahájit novou hru ";
    public string ButtonText
    {
        get
        {
            return buttonText;
        }
        set
        {
            buttonText = value;
            OnPropertyChanged();
        }
    }

    private int parsedRows;
    private int parsedColumns;

    public int bombCount;
    public int cellCount;
    public int unrevealedCellCount;


    public BoardViewModel(Grid grid)
    {
        _grid = grid;
        NewGameCommand = new Command(StartNewGame);

        parsedRows = Preferences.Get("Rows", 15);
        Rows = parsedRows.ToString();
        parsedColumns = Preferences.Get("Columns", 10);
        Columns = parsedColumns.ToString();
    }

    public void StartNewGame()
    {
        ButtonText = "Zahájit novou hru ";
        bombCount = 0;
        isWinEndGame = false;

        if (!int.TryParse(Rows, out parsedRows)) parsedRows = 15;
        if (!int.TryParse(Columns, out parsedColumns)) parsedColumns = 10;

        parsedRows = Math.Clamp(parsedRows, 1, 99);
        parsedColumns = Math.Clamp(parsedColumns, 1, 99);

        cellCount = parsedRows * parsedColumns;
        unrevealedCellCount = cellCount;

        Preferences.Set("Rows", parsedRows);
        Preferences.Set("Columns", parsedColumns);

        _cellViewModelBoard = new CellViewModel[parsedRows, parsedColumns];

        // Vymazání předchozího obsahu
        _grid.Children.Clear();
        _grid.RowDefinitions.Clear();
        _grid.ColumnDefinitions.Clear();

        // Definování řádků a sloupců v gridu
        for (int r = 0; r < parsedRows; r++) _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        for (int c = 0; c < parsedColumns; c++) _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var rnd = new Random();

        // Vytvoření všech políček a jejich tlačítek
        for (int row = 0; row < parsedRows; row++)
        {
            for (int col = 0; col < parsedColumns; col++)
            {
                var isMine = rnd.Next(100) < MineChancePercent; // náhodné rozhodnutí, zda je to mina
                if ( isMine)
                    bombCount++;

                var cell = new CellViewModel(row, col, this) { IsMine = isMine };
                _cellViewModelBoard[row, col] = cell;

                var button = new Button
                {
                    FontFamily = "FontAwesome",
                    BackgroundColor = Colors.LightGray,
                    BorderColor = Colors.Black,
                    BorderWidth = 1,
                    FontSize = 18,
                    Padding = 0,
                    Margin = 1,
                    BindingContext = cell
                };

                // Bindingy na vlastnosti CellViewModelu
                button.SetBinding(Button.TextProperty, new Binding(nameof(CellViewModel.DisplayText), mode: BindingMode.OneWay));
                button.SetBinding(Button.TextColorProperty, new Binding(nameof(CellViewModel.TextColor), mode: BindingMode.OneWay));
                button.SetBinding(Button.BackgroundColorProperty, new Binding(nameof(CellViewModel.BackgroundColor), mode: BindingMode.OneWay));
                button.SetBinding(Button.CommandProperty, new Binding(nameof(CellViewModel.ClickCommand)));
                //button.Clicked += (s, e) => cell.OnClick();

                _grid.Add(button, col, row);
            }
        }

        // Spočítání počtu okolních min pro každé políčko
        for (int r = 0; r < parsedRows; r++)
            for (int c = 0; c < parsedColumns; c++)
                _cellViewModelBoard[r, c].UpdateAdjacentMineCount();
    }

    public void EndLossGame()
    {
        isWinEndGame = false;
        RevealAllCells();
        ButtonText = "Prohral jste! Zahájit novou hru ";
    }

    public void EndWinGame()
    {
        isWinEndGame = true;
        RevealAllCells();
        ButtonText = "Vyhrál jste! Zahájit novou hru ";
    }

    public void RevealAllCells()
    {
        foreach (var cell in _cellViewModelBoard)
        {
            cell.IsRevealed = true; // odkryje všechna políčka
            cell.OnPropertyChanged();
            cell.OnPropertyChanged();
            cell.OnPropertyChanged();
            cell.OnPropertyChanged();
        }
    }

    // Vrací počet min v sousedství konkrétního políčka
    public int CountAdjacentMines(int row, int col)
    {
        int count = 0;
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue; // přeskočíme sebe
                int nr = row + dr, nc = col + dc;
                if (nr >= 0 && nr < parsedRows && nc >= 0 && nc < parsedColumns && _cellViewModelBoard[nr, nc].IsMine)
                    count++;
            }
        }
        return count;
    }
}