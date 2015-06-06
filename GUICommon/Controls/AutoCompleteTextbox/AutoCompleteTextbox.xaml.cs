﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MPDisplay.Common.Controls
{
    /// <summary>
    /// Interaction logic for AutoCompleteTextbox.xaml
    /// </summary>
    public partial class AutoCompleteTextbox
    {
        #region Members
        private bool _insertText;
        private int _searchThreshold = 2;
        private string _currentWord = string.Empty;

        #endregion

        #region Constructor

        public AutoCompleteTextbox()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods
     
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); _insertText = true; }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteTextbox), 
            new UIPropertyMetadata(string.Empty));

        public ObservableCollection<AutoCompleteEntry> AutoCompletionList
        {
            get { return (ObservableCollection<AutoCompleteEntry>)GetValue(AutoCompletionListProperty); }
            set { SetValue(AutoCompletionListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoCompletionList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoCompletionListProperty =
            DependencyProperty.Register("AutoCompletionList", typeof(ObservableCollection<AutoCompleteEntry>), typeof(AutoCompleteTextbox)
            , new UIPropertyMetadata(new ObservableCollection<AutoCompleteEntry>()));


        
        public int Threshold
        {
            get { return _searchThreshold; }
            set { _searchThreshold = value; }
        }

        private void TextChanged()
        {
            try
            {
                comboBox.Items.Clear();
                if (_currentWord.Length >= _searchThreshold)
                {
                    foreach (var entry in AutoCompletionList)
                    {
                        if (!entry.KeywordStrings.Any(word => word.StartsWith(_currentWord, StringComparison.CurrentCultureIgnoreCase)))
                            continue;
                        var cbItem = new ComboBoxItem {Content = entry.ToString()};
                        comboBox.Items.Add(cbItem);
                    }
                    comboBox.Margin = new Thickness(textbox.GetRectFromCharacterIndex(textbox.Text.Length).X, 0, 0, 0);
                    comboBox.IsDropDownOpen = comboBox.HasItems;
                }
                else
                {
                    comboBox.IsDropDownOpen = false;
                }
            }
            catch
            {
                // ignored
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null == comboBox.SelectedItem) return;

            _insertText = true;
            var cbItem = (ComboBoxItem)comboBox.SelectedItem;
            Text = Text.TrimEnd(_currentWord.ToArray()) + cbItem.Content;
            textbox.Focus();
            textbox.CaretIndex = Text.Length;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_insertText)
            {
                _insertText = false;
            }
            else
            {
                _currentWord = new string(Text.Skip(Text.LastIndexOf(" ", StringComparison.Ordinal)).ToArray()).Trim();
                TextChanged();
            }
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down) return;
            if (comboBox.IsDropDownOpen)
            {
                comboBox.Focus();
            }
        }

        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up) return;
            var item = comboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(c => c.IsHighlighted);
            if (item != null && (comboBox.IsDropDownOpen && comboBox.Items.IndexOf(item) == 0))
            {
                textbox.Focus();
            }
        }

        #endregion
    }


}