using System;
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
using System.Windows.Shapes;

namespace WPF_PDFDocument.Dialog
{
    
    public partial class DeletePage : Window
    {
        TabItem currenttab;
        public DeletePage()
        {
            InitializeComponent();
            this.currentpage.IsChecked = true;
        }

        public DeletePage(TabItem tab)
        {
            InitializeComponent();
            currenttab = tab;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Perform_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete?", "Quick Pdf Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if(result== MessageBoxResult.Yes)
            {
                var pdfviewer = this.currenttab.Content as Controls.PdfViewer;
                if (this.currentpage.IsChecked == true)
                {
                    string newpath = PDFAction.DeletePage(pdfviewer.PdfPath, pdfviewer.CurrentPage, pdfviewer.CurrentPage);
                    pdfviewer.PdfPath = newpath;
                    MessageBox.Show("Page deleted!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (this.multipage.IsChecked == true)
                {
                    int fromindex = Convert.ToInt32(this.from.Text);
                    int toindex = Convert.ToInt32(this.to.Text);
                    string newpath = PDFAction.DeletePage(pdfviewer.PdfPath, fromindex, toindex);
                    pdfviewer.PdfPath = newpath;
                    MessageBox.Show("Pages deleted!", "Quick Pdf Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            this.Close();
        }
    }
}
