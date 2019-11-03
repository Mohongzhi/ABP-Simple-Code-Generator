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


            lblName.DataContext = itemForEdit;
            lblCode.DataContext = itemForEdit;
            lblAttributeName.DataContext = itemForEdit;
            lblSummary.DataContext = itemForEdit;
            LoadEntities();
            listEntities.ItemsSource = iocItems;
        }

        #region Basic data management 基础数据管理

        IocItem itemForEdit = new IocItem();

        BindingList<IocItem> iocItems;

        const string iocItems_path = "IocItems.json";

        /// <summary>
        /// 加载实体
        /// </summary>
        public void LoadEntities()
        {
            if (!File.Exists(iocItems_path))
            {
                iocItems = new BindingList<IocItem>();
                return;
            }
            iocItems = JsonConvert.DeserializeObject<BindingList<IocItem>>(File.ReadAllText(iocItems_path));
        }

        private void BtnAddEntity_Click(object sender, RoutedEventArgs e)
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

        #endregion


    }
}
