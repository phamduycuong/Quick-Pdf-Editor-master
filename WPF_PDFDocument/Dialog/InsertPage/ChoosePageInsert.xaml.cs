using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_PDFDocument.Dialog
{
    /// <summary>
    /// Interaction logic for ChoosePageInsert.xaml
    /// </summary>
    public partial class ChoosePageInsert : Window
    {
        List<int> ListPage;
        InsertPage parent;
        public ChoosePageInsert()
        {
            InitializeComponent();
        }
        public ChoosePageInsert(List<int> ListPageInsert, InsertPage insertPage)
        {
            InitializeComponent();
            ListPage = ListPageInsert;
            parent = insertPage;
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            int begin, end;
            begin = Convert.ToInt32(tbBegin.Text);
            end = Convert.ToInt32(tbEnd.Text);
            for (int i = begin; i <= end; i++)
            {
                ListPage.Add(i);
            }
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChoosePageClosed(object sender, EventArgs e)
        {
            parent.UpdateListPage();
        }

        private void tbBegin_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(!(e.Key==System.Windows.Input.Key.NumPad0|| e.Key == System.Windows.Input.Key.NumPad1|| e.Key == System.Windows.Input.Key.NumPad2|| e.Key == System.Windows.Input.Key.NumPad3|| e.Key == System.Windows.Input.Key.NumPad4|| e.Key == System.Windows.Input.Key.NumPad5|| e.Key == System.Windows.Input.Key.NumPad6|| e.Key == System.Windows.Input.Key.NumPad7|| e.Key == System.Windows.Input.Key.NumPad8|| e.Key == System.Windows.Input.Key.NumPad9||(e.Key < System.Windows.Input.Key.D9 && e.Key > System.Windows.Input.Key.D0)))
            {
                e.Handled = true;
            }
            
        }
    }
}
