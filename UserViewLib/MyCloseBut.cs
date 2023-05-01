using System.Windows;
using System.Windows.Controls;

namespace ViewLib
{
    public class MyCloseBut:Button
    {
        public MyCloseBut()
        {
            Click += (s, e) => Window.GetWindow(this).Close();
        }
        
    }
}
