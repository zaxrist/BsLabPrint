using BsLabPrint.Modules;
using System;
using System.Windows.Controls;

namespace BsLabPrint.Views
{
    /// <summary>
    /// Interaction logic for BarcodeControlView.xaml
    /// </summary>
    public partial class BarcodeControlView : UserControl
    {

        public BarcodeControlView()
        {
            InitializeComponent();
            
        }

        public void SetBarcode(string TextBarcode)
        {
            if (TextBarcode == "" || TextBarcode == null)
            {
                throw new Exception("Text cannot be empty");
            }
            BarcodeInsert.Source = Barcode.BitmapToImageSource(TextBarcode);
            TextInput.Text = TextBarcode;
        }
    }
}
