using Microsoft.Win32;
using System;
using System.IO;

namespace Views.Windows
{
    public static class OpenSaveViewHelper
    {
        private static OpenFileDialog _openFdialog = new()
        {
            Filter = "Список сотрудников(*.json)|*.json| Все файлы (*.*)|*.*",
            FileName = "Список сотрудников",
            DefaultExt = ".json",
            InitialDirectory = ListFolder
        };
        private static SaveFileDialog _saveFdialog = new()
        {
            Filter = "Список сотрудников(*.json)|*.json| Все файлы (*.*)|*.*",
            FileName = "Список сотрудников",
            DefaultExt = ".json",
            InitialDirectory = ListFolder
        };
        private readonly static string ListFolder = 
            (Directory.CreateDirectory(Environment.CurrentDirectory + @"\Списки\")).ToString();
       

        public static string OpenDial()
        {
            if (_openFdialog.ShowDialog() == true)
            {
               return _openFdialog.FileName;
               
            }
            return string.Empty;
        }


        public static string SaveDial()
        {
            if (_saveFdialog.ShowDialog() == true)
            {
                return _saveFdialog.FileName;
            }
            return string.Empty;
        }

    }
}

