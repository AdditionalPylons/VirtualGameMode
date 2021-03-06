﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VirtualGameMode.Commands;
using VirtualGameMode.ViewModels;

namespace VirtualGameMode.Dialogs
{
    /// <summary>
    /// Interaction logic for AddApplicationDialog.xaml
    /// </summary>
    public partial class AddApplicationDialog
    { 
        public AddApplicationDialog()
        {
            InitializeComponent();
        }

        private void ComboBox_OnDropDownOpened(object sender, EventArgs e)
        {
            ((AddApplicationViewModel)DataContext).FindActiveWindows();
        }
    }
}
