using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WpfABPSimpleCodeGenerator
{
    public class Setting : INotifyPropertyChanged
    {
        private string _DbContextPath;

        public string DbContextPath
        {
            get
            {
                return _DbContextPath;
            }
            set
            {
                _DbContextPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DbContextPath"));
            }
        }

        private string _CoreProjectPath;

        public string CoreProjectPath
        {
            get
            {
                return _CoreProjectPath;
            }
            set
            {
                _CoreProjectPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CoreProjectPath"));
            }
        }

        private string _ApplicationProjectPath;

        public string ApplicationProjectPath
        {
            get
            {
                return _ApplicationProjectPath;
            }
            set
            {
                _ApplicationProjectPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApplicationProjectPath"));
            }
        }

        private string _MVCProjectPath;

        public string MVCProjectPath
        {
            get
            {
                return _MVCProjectPath;
            }
            set
            {
                _MVCProjectPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MVCProjectPath"));
            }
        }

        private string _LocalizationPath;

        public string LocalizationPath
        {
            get
            {
                return _LocalizationPath;
            }
            set
            {
                _LocalizationPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocalizationPath"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
