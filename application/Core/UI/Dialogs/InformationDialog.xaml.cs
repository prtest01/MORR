﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Morr.Core.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for InformationDialog.xaml
    /// </summary>
    public partial class InformationDialog : Window
    {
        public InformationDialog()
        {
            InitializeComponent();
        }

        private void OnAccept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
