using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF_PDFDocument.Dialog
{
    
    public partial class InsertPage : Window
    {
        TabItem tabitem;
        private List<int> ListPageInsert;
        private int _offset;
        private int offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                tboffset.Text = _offset.ToString();
            }
              
            
        }
        private string DesPath;
        public InsertPage()
        {
            InitializeComponent();
            ListPageInsert = new List<int>();
            offset = PreviewPDF.PagesContainer.Items.Count;
        }
        
        //Woring
        public InsertPage(TabItem tab)
        {
            InitializeComponent();
            ListPageInsert = new List<int>();
            this.tboffset.Text = "0";
            var pdfviewer = tab.Content as Controls.PdfViewer;
            try
            {
                DesPath = pdfviewer.PdfPath;
            }catch(NullReferenceException)
            {

            }
            this.tabitem = tab;
        }

        private void InsertCurrentPage_Click(object sender, RoutedEventArgs e)
        {
            ListPageInsert.Add(Convert.ToInt32(PreviewPDF.CurrentPage));
            UpdateListPage();
        }

        public void UpdateListPage()
        {
            ListBox_Data.Items.Clear();
            for (int i = 0; i < ListPageInsert.Count; i++)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = "Page " + ListPageInsert[i];
                this.ListBox_Data.Items.Add(listBoxItem);
            }

        }

        private void InsertPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (ListPageInsert.Count == 0)
            {
                MessageBox.Show("There is no page to insert!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if(tboffset.Text=="")
            {
                MessageBox.Show("Please enter offset!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var pdfviever = tabitem.Content as Controls.PdfViewer;

            if(pdfviever==null)
            {
                pdfviever = new Controls.PdfViewer();
                tabitem.Content = pdfviever;
                pdfviever.PdfPath = PDFAction.InsertPageFromPdf(this.PreviewPDF.PdfPath, ListPageInsert);
                pdfviever.OriginalPdfPath = "";

                MessageBox.Show("Insert Pages successfully!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                return;
            }

            PDFAction.InsertPageFromPdf(this.PreviewPDF.PdfPath, this.DesPath, ListPageInsert,this.offset);
            MessageBox.Show("Insert pages successfully!","Notification",MessageBoxButton.OK,MessageBoxImage.Information);
            //Create Temporary
            
            pdfviever.PdfPath= System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Merged" + --PDFAction.number + ".pdf");
            this.Close();
        }

        private void Insertatob_Click(object sender, RoutedEventArgs e)
        {
            ChoosePageInsert Pagea2b = new ChoosePageInsert(this.ListPageInsert,this);
            Pagea2b.Show();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            int removeindex = ListBox_Data.SelectedIndex;
            this.ListPageInsert.RemoveAt(removeindex);
            UpdateListPage();
        }

        private void PageOffset_Changed(object sender, TextChangedEventArgs e)
        {
            if (tboffset.Text == "")
                tboffset.Text = "0";
            this.offset = Convert.ToInt32(this.tboffset.Text);
        }

        private void Offset_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(e.Key == System.Windows.Input.Key.NumPad0 || e.Key == System.Windows.Input.Key.NumPad1 || e.Key == System.Windows.Input.Key.NumPad2 || e.Key == System.Windows.Input.Key.NumPad3 || e.Key == System.Windows.Input.Key.NumPad4 || e.Key == System.Windows.Input.Key.NumPad5 || e.Key == System.Windows.Input.Key.NumPad6 || e.Key == System.Windows.Input.Key.NumPad7 || e.Key == System.Windows.Input.Key.NumPad8 || e.Key == System.Windows.Input.Key.NumPad9 || (e.Key < System.Windows.Input.Key.D9 && e.Key > System.Windows.Input.Key.D0)))
            {
                e.Handled = true;
            }
        }
    }
}
