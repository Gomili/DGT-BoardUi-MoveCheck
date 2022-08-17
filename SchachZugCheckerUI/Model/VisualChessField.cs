using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchachZugCheckerUI.Model
{
    public enum FieldState
    {
        None = 0, // Field is empty and has no color
        BlueState = 1, // Figure can set on this field
        Red = 2, // Figure can hit the figure on the field
    }

    public partial class VisualChessField : ObservableObject
    {
        [ObservableProperty] private string? _figur;
        [ObservableProperty] private FieldState _fieldState = FieldState.None;
        [ObservableProperty] private int _arrayPos;
        [ObservableProperty] private string _boardPos = string.Empty;
    }
}
