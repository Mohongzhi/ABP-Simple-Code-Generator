using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WpfABPSimpleCodeGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class IocItem : INotifyPropertyChanged
    {
        private bool _IsChecked { get; set; }

        /// <summary>
        /// to binding name
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                _IsChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }

        //private string _Name { get; set; }

        ///// <summary>
        ///// to binding name
        ///// </summary>
        //public string Name
        //{
        //    get
        //    {
        //        return _Name;
        //    }
        //    set
        //    {
        //        _Name = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
        //    }
        //}

        private string _Summary { get; set; }

        /// <summary>
        /// remark
        /// </summary>
        public string Summary
        {
            get { return _Summary; }
            set
            {
                _Summary = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Summary"));
            }
        }

        private string _AttributeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AttributeName
        {
            get
            {
                return _AttributeName;
            }
            set
            {
                _AttributeName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AttributeName"));

            }
        }

        private string _Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Code"));
            }
        }

        private string _Namespace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Namespace
        {
            get
            {
                return _Namespace;
            }
            set
            {
                _Namespace = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Namespace"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
