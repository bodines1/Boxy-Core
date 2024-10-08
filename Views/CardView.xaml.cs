﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Boxy_Core.Views
{
    /// <summary>
    /// Interaction logic for CardView.xaml
    /// </summary>
    public partial class CardView
    {
        public CardView()
        {
            InitializeComponent();
        }

        private void ComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ComboBox comboBox)
            {
                return;
            }

            if (comboBox.Template.FindName("PART_Popup", comboBox) is Popup pop)
            {
                pop.Placement = PlacementMode.Top;
            }
        }

        private void ToolTip_OnOpened(object sender, RoutedEventArgs e)
        {
            PrintingsComboBox.Focus();
        }
    }
}
