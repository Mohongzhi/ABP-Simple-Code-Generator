using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfABPSimpleCodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            #region Load languages from the resources
            CultureInfo Culture = CultureInfo.CurrentCulture;
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format(@"Resources\StringResource.{0}.xaml", Culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            if (resourceDictionary == null)
            {
                requestedCulture = @"Resources\StringResource.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            }
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            #endregion

            #region Generator window
            txtDataType.Items.Add("string");
            txtDataType.Items.Add("int");
            txtDataType.Items.Add("decimal");
            txtDataType.Items.Add("double");
            txtDataType.Items.Add("bool");

            listFields.ItemsSource = entityFieldItems;
            txtEntityName.DataContext = itemForGenerator;
            txtDataType.DataContext = itemForGenerator;
            txtFieldName.DataContext = itemForGenerator;
            txtFieldSummary.DataContext = itemForGenerator;
            checkGenerateAPI.DataContext = itemForGenerator;
            checkGenerateDto.DataContext = itemForGenerator;
            checkGenerateHTML.DataContext = itemForGenerator;
            checkInheritEntity.DataContext = itemForGenerator;
            #endregion

            #region Basic data management
            lblName.DataContext = itemForEdit;
            lblCode.DataContext = itemForEdit;
            lblAttributeName.DataContext = itemForEdit;
            lblSummary.DataContext = itemForEdit;
            LoadIoc();
            listEntities.ItemsSource = iocItems; 
            listIocInject.ItemsSource = iocItems;
            #endregion
        }

        #region Basic data management 基础数据管理

        IocItem itemForEdit = new IocItem();

        BindingList<IocItem> iocItems;

        const string iocItems_path = "IocItems.json";

        /// <summary>
        /// 
        /// </summary>
        public void LoadIoc()
        {
            if (!File.Exists(iocItems_path))
            {
                iocItems = new BindingList<IocItem>();
                return;
            }
            iocItems = JsonConvert.DeserializeObject<BindingList<IocItem>>(File.ReadAllText(iocItems_path));
        }

        private void BtnAddIoc_Click(object sender, RoutedEventArgs e)
        {
            var item = new IocItem()
            {
                AttributeName = itemForEdit.AttributeName,
                Code = itemForEdit.Code,
                Name = itemForEdit.Name,
                Summary = itemForEdit.Summary
            };

            iocItems.Add(item);

            File.WriteAllText(iocItems_path, JsonConvert.SerializeObject(iocItems));
        }


        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var tb = sender as TextBlock;
                var name = tb.Text;
                iocItems.Remove(iocItems.First(p => p.Name == name));
            }
            File.WriteAllText(iocItems_path, JsonConvert.SerializeObject(iocItems));
        }

        #endregion

        #region Generator window

        GeneratorItem itemForGenerator = new GeneratorItem() { DataType = "string" , InheritEntity=true, GenerateAPI=true, GenerateDto=true, GenerateHTML=true};

        BindingList<EntityFieldItem> entityFieldItems = new BindingList<EntityFieldItem>();

        public void BtnAddEntity_Click(object sender, RoutedEventArgs e)
        {
            entityFieldItems.Add(new EntityFieldItem() { DataType = itemForGenerator.DataType, FieldName = itemForGenerator.FieldName, FieldSummary = itemForGenerator.FieldSummary });
        }

        #endregion

    }
}
