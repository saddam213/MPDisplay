using System;
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
    public partial class AutoCompleteTextbox : UserControl
     {
        #region Members
        private bool insertText;
        private int searchThreshold = 2;
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
            set { SetValue(TextProperty, value); insertText = true; }
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
            get { return searchThreshold; }
            set { searchThreshold = value; }
        }

        private void TextChanged()
        {
            try
            {
                comboBox.Items.Clear();
                if (_currentWord.Length >= searchThreshold)
                {
                    foreach (AutoCompleteEntry entry in AutoCompletionList)
                    {
                        foreach (string word in entry.KeywordStrings)
                        {
                            if (word.StartsWith(_currentWord, StringComparison.CurrentCultureIgnoreCase))
                            {
                                ComboBoxItem cbItem = new ComboBoxItem();
                                cbItem.Content = entry.ToString();
                                comboBox.Items.Add(cbItem);
                                break;
                            }
                        }
                    }
                    comboBox.Margin = new Thickness(textbox.GetRectFromCharacterIndex(textbox.Text.Length).X, 0, 0, 0);
                    comboBox.IsDropDownOpen = comboBox.HasItems;
                }
                else
                {
                    comboBox.IsDropDownOpen = false;
                }
            }
            catch { }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != comboBox.SelectedItem)
            {
                insertText = true;
                ComboBoxItem cbItem = (ComboBoxItem)comboBox.SelectedItem;
                Text = Text.TrimEnd(_currentWord.ToArray()) + cbItem.Content.ToString();
                textbox.Focus();
                textbox.CaretIndex = Text.Length;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (insertText == true)
            {
                insertText = false;
            }
            else
            {
                _currentWord = new string(Text.Skip(Text.LastIndexOf(" ")).ToArray()).Trim();
                TextChanged();
            }
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (comboBox.IsDropDownOpen)
                {
                    comboBox.Focus();
                }
            }
        }

        private void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (comboBox.IsDropDownOpen && comboBox.Items.IndexOf(comboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(c => c.IsHighlighted)) == 0)
                {
                    textbox.Focus();
                }
            }
        }

        #endregion
    }


}