using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WpfABPSimpleCodeGenerator
{
    public class EntityFieldItem : INotifyPropertyChanged
    {

        private string _FieldName;

        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                _FieldName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FieldName"));
            }
        }

        private string _DataType;

        public string DataType
        {
            get
            {
                return _DataType;
            }
            set
            {
                _DataType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DataType"));
            }
        }

        private string _FieldSummary;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FieldSummary
        {
            get
            {
                return _FieldSummary;
            }
            set
            {
                _FieldSummary = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FieldSummary"));
            }
        }
    }
}
