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

namespace PPTControllerHost.Content
{
    /// <summary>
    /// Interaction logic for CommonProblems.xaml
    /// </summary>
    public partial class CommonProblems : UserControl
    {
        public CommonProblems()
        {
            InitializeComponent();

            this.DataContext = new CommonProblemsViewModel();
        }
    }
}
