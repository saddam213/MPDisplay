﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Common.Log;
using GUIConfig.Settings;

namespace GUIConfig.Dialogs
{
    /// <summary>
    /// Interaction logic for LanguageDialog.xaml
    /// </summary>
    public partial class LanguageDialog : INotifyPropertyChanged
    {
        private readonly Log _log = LoggingManager.GetLog(typeof(LanguageDialog));

        /// <summary>
        /// Initializes a new instance of the DialogLanguagePicker class.
        /// </summary>
        public LanguageDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            _log.Message(LogLevel.Info, "Displaying Language picker dialog");
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<summary>
         ///Gets the languages.
        ///</summary>
        public IEnumerable<string> Languages => LanguageHelper.Languages;

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        public string SelectedLanguage { get; set; } = "English";

        /// <summary>
        /// Handles the Click event of the Button_OK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            _log.Message(LogLevel.Info, "Language selected, Lanuage: {0}", SelectedLanguage);
            DialogResult = true;
        }
      
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="info">The info.</param>
        public void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
