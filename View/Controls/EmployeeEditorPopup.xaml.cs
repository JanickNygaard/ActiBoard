using System;
using System.Windows;
using System.Windows.Controls;

namespace ActiBoard.View.Controls
{
    public partial class EmployeeEditorPopup : UserControl
    {
        public event KeyboardRequestedEventHandler? KeyboardRequested;
        public EmployeeEditorPopup()
        {
            InitializeComponent();
        }
    }
}