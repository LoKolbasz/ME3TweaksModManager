﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LegendaryExplorerCore.Misc;
using ME3TweaksModManager.modmanager.localizations;
using PropertyChanged;
using CheckBoxSelectionPair = ME3TweaksModManager.ui.CheckBoxSelectionPair;

namespace ME3TweaksModManager.modmanager.usercontrols.generic
{
    /// <summary>
    /// Interaction logic for CheckBoxComboBox.xaml
    /// </summary>
    public partial class CheckBoxComboBox : UserControl, INotifyPropertyChanged
    {

        #region Dependency Properties
        #region NoItemsSelectedText
        public string NoItemsSelectedText
        {
            get => (string)GetValue(NoItemsSelectedTextProperty);
            set => SetValue(NoItemsSelectedTextProperty, value);
        }

        public static readonly DependencyProperty NoItemsSelectedTextProperty =
            DependencyProperty.Register(@"NoItemsSelectedText", typeof(string), typeof(CheckBoxComboBox), new PropertyMetadata(M3L.GetString(M3L.string_noItemsSelected)));

        #endregion

        #region DefaultCheckState
        public bool DefaultCheckState
        {
            get => (bool)GetValue(DefaultCheckStateProperty);
            set => SetValue(DefaultCheckStateProperty, value);
        }

        public static readonly DependencyProperty DefaultCheckStateProperty =
            DependencyProperty.Register(@"DefaultCheckState", typeof(bool), typeof(CheckBoxComboBox), new PropertyMetadata(false));

        #endregion

        #region ItemJoinString
        public string ItemJoinString
        {
            get => (string)GetValue(ItemJoinStringProperty);
            set => SetValue(ItemJoinStringProperty, value);
        }

        public static readonly DependencyProperty ItemJoinStringProperty =
            DependencyProperty.Register(@"ItemJoinString", typeof(string), typeof(CheckBoxComboBox), new PropertyMetadata(","));

        #endregion

        #region ItemsSource
        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(@"ItemsSource", typeof(ICollection), typeof(CheckBoxComboBox), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

        [SuppressPropertyChangedWarnings]
        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as CheckBoxComboBox;
            control?.OnItemsSourceChanged((ICollection)e.OldValue, (ICollection)e.NewValue);
        }

        [SuppressPropertyChangedWarnings]
        private void OnItemsSourceChanged(ICollection oldValue, ICollection newValue)
        {
            // Remove handler for oldValue.CollectionChanged

            if (oldValue is INotifyCollectionChanged oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }
            // Add handler for newValue.CollectionChanged (if possible)
            if (newValue is INotifyCollectionChanged newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(newValueINotifyCollectionChanged_CollectionChanged);
            }

            // ICollection doesn't have linq support so just kinda old school it
            SetInternalItems(newValue);
            CBSelector.SelectedItem = InternalItemsSource.FirstOrDefault();
        }

        private void SetInternalItems(ICollection newItems)
        {
            List<CheckBoxSelectionPair> pairs = new List<CheckBoxSelectionPair>();
            foreach (var v in newItems)
            {
                pairs.Add(new CheckBoxSelectionPair(v, DefaultCheckState, CheckChanged));
            }

            InternalItemsSource.ReplaceAll(pairs);
            UpdateSelectedItemString();
        }

        private void CheckChanged(CheckBoxSelectionPair itemChanging)
        {
            UpdateSelectedItemString();
        }

        private void newValueINotifyCollectionChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                SetInternalItems(sender as ICollection);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                InternalItemsSource.Insert(e.NewStartingIndex, new CheckBoxSelectionPair(e.NewItems[0], DefaultCheckState, CheckChanged));
                UpdateSelectedItemString();
            }
        }
        #endregion

        #endregion

        #region Properties

        public string SelectedItemsString
        {
            get;
            set;
        }

        public void OnSelectedItemsStringChanged()
        {
            // Total hack. No idea how tf this binding works
            var index = CBSelector.SelectedIndex;
            var count = InternalItemsSource.Count;
            if (count > 1)
            {
                CBSelector.SelectedIndex = index == 0 ? 1 : 0;
            }
        }

        private void UpdateSelectedItemString()
        {
            var selectedItems = InternalItemsSource.Where(x => x.IsChecked).ToList();
            if (selectedItems.Any())
                SelectedItemsString = string.Join(ItemJoinString, selectedItems.Select(x => x.ToString()));
            else
                SelectedItemsString = NoItemsSelectedText;
        }
        #endregion


        public ObservableCollectionExtended<CheckBoxSelectionPair> InternalItemsSource { get; } = new ObservableCollectionExtended<CheckBoxSelectionPair>();

        /// <summary>
        /// Sets the list of selected items
        /// </summary>
        /// <param name="selectedItems"></param>
        public void SetSelectedItems(IEnumerable<object> selectedItems)
        {
            var sic = selectedItems.ToList();
            foreach (var v in InternalItemsSource)
            {
                v.IsChecked = sic.Contains(v.Item);
            }
            UpdateSelectedItemString();
        }

        public void ClearAllSelectedItems()
        {
            foreach (var v in InternalItemsSource)
            {
                v.IsChecked = false;
            }
            UpdateSelectedItemString();
        }

        public CheckBoxComboBox()
        {
            InitializeComponent();
            UpdateSelectedItemString();
        }


        private void CheckChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                UpdateSelectedItemString();
            }
        }

#pragma warning disable
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore

        private void CB_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ComboBox cb && cb.IsDropDownOpen)
            {
                e.Handled = true; // Do not auto close
            }
        }

        private void CBCB_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                cb.IsChecked = !cb.IsChecked;
                e.Handled = true;
            }
        }
        public IEnumerable<object> GetSelectedItems()
        {
            return InternalItemsSource.Where(x => x.IsChecked).Select(x => x.Item);
        }
    }
}
