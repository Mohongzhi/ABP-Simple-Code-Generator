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

        private string _EntitySummary;

        public string EntitySummary
        {
            get
            {
                return _EntitySummary;
            }
            set
            {
                _EntitySummary = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EntitySummary"));
            }
        }

        private string _EntityNamespace;

        public string EntityNamespace
        {
            get
            {
                return _EntityNamespace;
            }
            set
            {
                _EntityNamespace = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EntityNamespace"));
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

        private bool _CreationAudited;

        public bool CreationAudited
        {
            get
            {
                return _CreationAudited;
            }
            set
            {
                _CreationAudited = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CreationAudited"));
            }
        }

        private bool _ModifyAudited;

        public bool ModifyAudited
        {
            get
            {
                return _ModifyAudited;
            }
            set
            {
                _ModifyAudited = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModifyAudited"));
            }
        }

        private bool _DeletionAudited;

        public bool DeletionAudited
        {
            get
            {
                return _DeletionAudited;
            }
            set
            {
                _DeletionAudited = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeletionAudited"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
