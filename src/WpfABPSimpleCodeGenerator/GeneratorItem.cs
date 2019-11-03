using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WpfABPSimpleCodeGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class GeneratorItem : INotifyPropertyChanged
    {
        private string _EntityName;

        public string EntityName
        {
            get
            {
                return _EntityName;
            }
            set
            {
                _EntityName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EntityName"));
            }
        }

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

        private bool _InheritEntity;

        public bool InheritEntity
        {
            get
            {
                return _InheritEntity;
            }
            set
            {
                _InheritEntity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InheritEntity"));
            }
        }

        private bool _GenerateDto;

        public bool GenerateDto
        {
            get
            {
                return _GenerateDto;
            }
            set
            {
                _GenerateDto = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GenerateDto"));
            }
        }

        private bool _GenerateAPI;

        public bool GenerateAPI
        {
            get
            {
                return _GenerateAPI;
            }
            set
            {
                _GenerateAPI = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GenerateAPI"));
            }
        }

        private bool _GenerateHTML;

        public bool GenerateHTML
        {
            get
            {
                return _GenerateHTML;
            }
            set
            {
                _GenerateHTML = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GenerateHTML"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
