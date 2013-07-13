﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using GUISkinFramework;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Styles;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    public partial class ImageEditorView : EditorViewModel
    {
        public ImageEditorView()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Image Editor"; }
        }
    }
}
