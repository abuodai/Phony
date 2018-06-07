﻿using System.Windows;

namespace Phony.View
{
    /// <summary>
    /// Interaction logic for SalesBillsViewer.xaml
    /// </summary>
    public partial class SalesBillsViewer : Window
    {
        public SalesBillsViewer()
        {
            InitializeComponent();
        }

        public SalesBillsViewer(long billNo)
        {
            InitializeComponent();
            this.DataContext = new ViewModel.SalesBillsViewerVM(billNo);
        }
    }
}