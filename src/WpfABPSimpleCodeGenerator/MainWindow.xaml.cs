using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
        private const string DefaultNamespace = "FengWo";

        private const string DefaultPagenationInputDto = "PagedSortedAndSearchInputDto";

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
            txtDataType.Items.Add("int?");
            txtDataType.Items.Add("decimal");
            txtDataType.Items.Add("decimal?");
            txtDataType.Items.Add("double");
            txtDataType.Items.Add("double?");
            txtDataType.Items.Add("bool");
            txtDataType.Items.Add("long");
            txtDataType.Items.Add("long?");
            txtDataType.Items.Add("DateTime");
            txtDataType.Items.Add("DateTime?");

            listFields.ItemsSource = entityFieldItems;
            txtEntityName.DataContext = itemForGenerator;
            txtDataType.DataContext = itemForGenerator;
            txtFieldName.DataContext = itemForGenerator;
            txtFieldSummary.DataContext = itemForGenerator;
            checkGenerateAPI.DataContext = itemForGenerator;
            checkGenerateDto.DataContext = itemForGenerator;
            checkGenerateHTML.DataContext = itemForGenerator;
            checkInheritEntity.DataContext = itemForGenerator;
            txtEntitySummary.DataContext = itemForGenerator;
            checkCreationAudited.DataContext = itemForGenerator;
            checkDeletionAudited.DataContext = itemForGenerator;
            checkModifyAudited.DataContext = itemForGenerator;

            #endregion

            #region Basic data management
            lblCode.DataContext = itemForEdit;
            lblAttributeName.DataContext = itemForEdit;
            lblSummary.DataContext = itemForEdit;
            lblNamespace.DataContext = itemForEdit;
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
                //Name = itemForEdit.Name,
                Summary = itemForEdit.Summary,
                Namespace = itemForEdit.Namespace
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
                iocItems.Remove(iocItems.First(p => p.Code == name));
            }
            File.WriteAllText(iocItems_path, JsonConvert.SerializeObject(iocItems));
        }

        #endregion

        #region Generator window

        GeneratorItem itemForGenerator = new GeneratorItem() { DataType = "string", InheritEntity = true, GenerateAPI = true, GenerateDto = true, GenerateHTML = true, CreationAudited = true };

        BindingList<EntityFieldItem> entityFieldItems = new BindingList<EntityFieldItem>();

        public void BtnAddEntity_Click(object sender, RoutedEventArgs e)
        {
            entityFieldItems.Add(new EntityFieldItem() { DataType = itemForGenerator.DataType, FieldName = itemForGenerator.FieldName, FieldSummary = itemForGenerator.FieldSummary });
        }

        public void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            #region Entity generate
            StringBuilder sbForGenerateEntity = new StringBuilder();
            {
                sbForGenerateEntity.AppendLine("using Abp.Domain.Entities;");
                sbForGenerateEntity.AppendLine("using Abp.Domain.Entities.Auditing;");
                sbForGenerateEntity.AppendLine("using System;");
                sbForGenerateEntity.AppendLine($"namespace {itemForGenerator.EntityNamespace}");
                sbForGenerateEntity.AppendLine("{");//namespace start

                sbForGenerateEntity.AppendLine("/// <summary>");
                sbForGenerateEntity.AppendLine($"/// {itemForGenerator.EntitySummary}");
                sbForGenerateEntity.AppendLine("/// </summary>");

                var classItem = $"public class {itemForGenerator.EntityName}:";
                if (itemForGenerator.InheritEntity)
                    classItem += "Entity,";
                if (itemForGenerator.CreationAudited)
                    classItem += "ICreationAudited,";
                if (itemForGenerator.ModifyAudited)
                    classItem += "IModificationAudited,";
                if (itemForGenerator.DeletionAudited)
                    classItem += "IDeletionAudited,";

                sbForGenerateEntity.AppendLine(classItem.TrimEnd(':').TrimEnd(','));
                sbForGenerateEntity.AppendLine("{");//class start

                foreach (var item in entityFieldItems)
                {
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// {item.FieldSummary}");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine($"public {item.DataType} {item.FieldName} {{ get;set;}}");
                    sbForGenerateEntity.AppendLine("");
                }

                if (itemForGenerator.CreationAudited)
                {
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// 创建者ID");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine("public long? CreatorUserId { get; set; }");
                    sbForGenerateEntity.AppendLine("");
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// 创建者时间");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine("public DateTime CreationTime { get; set; }");
                    sbForGenerateEntity.AppendLine("");
                }
                if (itemForGenerator.ModifyAudited)
                {
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// 最后一次修改者ID");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine("public long? LastModifierUserId { get; set; }");
                    sbForGenerateEntity.AppendLine("");
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// 最后一次修改时间");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine("public DateTime? LastModificationTime { get; set; }");
                }
                if (itemForGenerator.DeletionAudited)
                {
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// 删除者ID");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine("public long? DeleterUserId { get; set; }");
                    sbForGenerateEntity.AppendLine("");
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// 删除时间");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine("public DateTime? DeletionTime { get; set; }");
                    sbForGenerateEntity.AppendLine("");
                    sbForGenerateEntity.AppendLine("/// <summary>");
                    sbForGenerateEntity.AppendLine($"/// 是否已删除");
                    sbForGenerateEntity.AppendLine("/// </summary>");
                    sbForGenerateEntity.AppendLine("public bool IsDeleted { get; set; }");
                }

                sbForGenerateEntity.AppendLine("}");//class end
                sbForGenerateEntity.AppendLine("}");//namespace end

                //saveFileDialog.FileName = itemForGenerator.EntityName;
                //if (saveFileDialog.ShowDialog() == true)
                //{
                //    File.WriteAllText(saveFileDialog.FileName, sbForGenerateEntity.ToString());
                //    MessageBox.Show("已保存实体文件");
                //}
            }
            #endregion

            #region Dto generate
            StringBuilder sbForDto = new StringBuilder();
            {
                if (itemForGenerator.GenerateDto)
                {
                    sbForDto.AppendLine("using Abp.Application.Services.Dto;");
                    sbForDto.AppendLine("using Abp.AutoMapper;");
                    sbForDto.AppendLine("using Abp.Domain.Entities.Auditing;");
                    sbForDto.AppendLine("using System;");
                    sbForDto.AppendLine($"namespace {DefaultNamespace}");
                    sbForDto.AppendLine("{");//namespace start

                    sbForDto.AppendLine("/// <summary>");
                    sbForDto.AppendLine($"/// {itemForGenerator.EntitySummary}");
                    sbForDto.AppendLine("/// </summary>");
                    sbForDto.AppendLine($"[AutoMap(typeof({itemForGenerator.EntityName}))]");
                    var classDtoItem = $"public class {itemForGenerator.EntityName}Dto:";
                    if (itemForGenerator.InheritEntity)
                        classDtoItem += "Entity,";
                    if (itemForGenerator.CreationAudited)
                        classDtoItem += "ICreationAudited,";
                    if (itemForGenerator.ModifyAudited)
                        classDtoItem += "IModificationAudited,";
                    if (itemForGenerator.DeletionAudited)
                        classDtoItem += "IDeletionAudited,";

                    sbForDto.AppendLine(classDtoItem.TrimEnd(':').TrimEnd(','));
                    sbForDto.AppendLine("{");//class start

                    foreach (var item in entityFieldItems)
                    {
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// {item.FieldSummary}");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine($"public {item.DataType} {item.FieldName} {{ get;set;}}");
                        sbForDto.AppendLine("");
                    }

                    if (itemForGenerator.CreationAudited)
                    {
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// 创建者ID");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine("public long? CreatorUserId { get; set; }");
                        sbForDto.AppendLine("");
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// 创建者时间");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine("public DateTime CreationTime { get; set; }");
                        sbForDto.AppendLine("");
                    }
                    if (itemForGenerator.ModifyAudited)
                    {
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// 最后一次修改者ID");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine("public long? LastModifierUserId { get; set; }");
                        sbForDto.AppendLine("");
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// 最后一次修改时间");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine("public DateTime? LastModificationTime { get; set; }");
                    }
                    if (itemForGenerator.DeletionAudited)
                    {
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// 删除者ID");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine("public long? DeleterUserId { get; set; }");
                        sbForDto.AppendLine("");
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// 删除时间");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine("public DateTime? DeletionTime { get; set; }");
                        sbForDto.AppendLine("");
                        sbForDto.AppendLine("/// <summary>");
                        sbForDto.AppendLine($"/// 是否已删除");
                        sbForDto.AppendLine("/// </summary>");
                        sbForDto.AppendLine("public bool IsDeleted { get; set; }");
                    }

                    sbForDto.AppendLine("}");//class end
                    sbForDto.AppendLine("}");//namespace end

                    //saveFileDialog.FileName = itemForGenerator.EntityName + "Dto";
                    //if (saveFileDialog.ShowDialog() == true)
                    //{
                    //    File.WriteAllText(saveFileDialog.FileName, sbForDto.ToString());
                    //    MessageBox.Show("已保存Dto文件");
                    //}
                }
            }
            #endregion

            #region AppService
            StringBuilder sbForPagenationDto = new StringBuilder();
            StringBuilder sbForIAppService = new StringBuilder();
            StringBuilder sbForAppService = new StringBuilder();
            if (itemForGenerator.GenerateAPI)
            {
                #region Pagenation dto
                {
                    sbForPagenationDto.AppendLine("using System.Collections.Generic;");
                    sbForPagenationDto.AppendLine($"namespace {DefaultNamespace}");
                    sbForPagenationDto.AppendLine("{");//namespace start

                    sbForPagenationDto.AppendLine("/// <summary>");
                    sbForPagenationDto.AppendLine($"/// {itemForGenerator.EntitySummary}分页返回Dto");
                    sbForPagenationDto.AppendLine("/// </summary>");
                    var classDtoItem = $"public class {itemForGenerator.EntityName}PagenationOutputDto";
                    sbForPagenationDto.AppendLine(classDtoItem);
                    sbForPagenationDto.AppendLine("{");//class start

                    sbForPagenationDto.AppendLine("/// <summary>");
                    sbForPagenationDto.AppendLine($"/// {itemForGenerator.EntitySummary}数据列表");
                    sbForPagenationDto.AppendLine("/// </summary>");
                    sbForPagenationDto.AppendLine($"public List<{itemForGenerator.EntityName}Dto> Rows {{ get; set; }}");
                    sbForPagenationDto.AppendLine("");
                    sbForPagenationDto.AppendLine("/// <summary>");
                    sbForPagenationDto.AppendLine($"/// 总行数");
                    sbForPagenationDto.AppendLine("/// </summary>");
                    sbForPagenationDto.AppendLine($"public int Total {{ get; set; }}");

                    sbForPagenationDto.AppendLine("}");//class end
                    sbForPagenationDto.AppendLine("}");//namespace end
                }
                #endregion

                #region IApplicationService interface
                {
                    sbForIAppService.AppendLine("using Abp.Application.Services;");
                    sbForIAppService.AppendLine("using System.Collections.Generic;");
                    sbForIAppService.AppendLine("using System.Threading.Tasks;");
                    sbForIAppService.AppendLine($"namespace {DefaultNamespace}");
                    sbForIAppService.AppendLine("{");//namespace start

                    sbForIAppService.AppendLine("/// <summary>");
                    sbForIAppService.AppendLine($"/// {itemForGenerator.EntitySummary} Services");
                    sbForIAppService.AppendLine("/// </summary>");
                    var classDtoItem = $"public interface I{itemForGenerator.EntityName}AppService: IApplicationService";
                    sbForIAppService.AppendLine(classDtoItem);
                    sbForIAppService.AppendLine("{");//class start

                    sbForIAppService.AppendLine($"{itemForGenerator.EntityName}PagenationOutputDto Get{itemForGenerator.EntityName}ByPagenation ({DefaultPagenationInputDto} pagedSortedAndSearchInputDto);");
                    sbForIAppService.AppendLine();
                    sbForIAppService.AppendLine($"Task<{itemForGenerator.EntityName}Dto> Get{itemForGenerator.EntityName}ById(int Id);");
                    sbForIAppService.AppendLine();
                    sbForIAppService.AppendLine($"Task<int> CreateOrUpdate{itemForGenerator.EntityName}({itemForGenerator.EntityName}Dto dto);");
                    sbForIAppService.AppendLine();
                    sbForIAppService.AppendLine($"Task Delete{itemForGenerator.EntityName}ById(int id);");

                    sbForIAppService.AppendLine("}");//class end
                    sbForIAppService.AppendLine("}");//namespace end
                }
                #endregion

                #region ApplicationService
                {
                    var checkedIocItems = iocItems.Where(p => p.IsChecked);
                    sbForAppService.AppendLine("using System.Collections.Generic;");
                    sbForAppService.AppendLine("using System.Threading.Tasks;");
                    sbForAppService.AppendLine("using Abp.Domain.Repositories;");
                    sbForAppService.AppendLine("using System.Linq.Dynamic.Core;");
                    sbForAppService.AppendLine("using System.Linq;");
                    sbForAppService.AppendLine("using Abp.ObjectMapping;");
                    sbForAppService.AppendLine("using Newtonsoft.Json;");
                    sbForAppService.AppendLine("using Newtonsoft.Json.Linq;");
                    foreach (var item in checkedIocItems)//IOC namespace
                    {
                        sbForAppService.AppendLine($"using {item.Namespace};");
                    }
                    sbForAppService.AppendLine($"namespace {DefaultNamespace}");
                    sbForAppService.AppendLine("{");//namespace start

                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// {itemForGenerator.EntitySummary} Services");
                    sbForAppService.AppendLine("/// </summary>");
                    var classDtoItem = $"public class {itemForGenerator.EntityName}AppService: ApplicationService, I{itemForGenerator.EntityName}AppService";
                    sbForAppService.AppendLine("{");//class start

                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// AutoMapper");
                    sbForAppService.AppendLine("/// </summary>");
                    sbForAppService.AppendLine("private readonly IObjectMapper _objectMapper;");
                    sbForAppService.AppendLine("");
                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// {itemForGenerator.EntityName} repository");
                    sbForAppService.AppendLine("/// </summary>");
                    sbForAppService.AppendLine($"private readonly IRepository<{itemForGenerator.EntityName}> _{itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository");
                    sbForAppService.AppendLine("");
                    foreach (var item in checkedIocItems)
                    {//Ioc
                        sbForAppService.AppendLine("/// <summary>");
                        sbForAppService.AppendLine($"/// {item.Summary}");
                        sbForAppService.AppendLine("/// </summary>");
                        sbForAppService.AppendLine($"private readonly {item.Code} _{item.AttributeName.Substring(0, 1).ToLower()}{item.AttributeName.Substring(1)};");
                        sbForAppService.AppendLine("");
                    }
                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// IOC");
                    sbForAppService.AppendLine("/// </summary>");
                    sbForAppService.AppendLine($"public {itemForGenerator.EntityName}AppService(IObjectMapper objectMapper,");
                    StringBuilder sbIOCList = new StringBuilder();
                    sbIOCList.AppendLine($"IRepository<{itemForGenerator.EntityName}> {itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository,");
                    foreach (var item in checkedIocItems)
                    {
                        sbIOCList.AppendLine($"{item.Code} {item.AttributeName.Substring(0, 1).ToLower()}{item.AttributeName.Substring(1)},");
                    }
                    sbForAppService.AppendLine($"{sbIOCList.ToString().TrimEnd(',')})");
                    sbForAppService.AppendLine("{");//construction start
                    sbForAppService.AppendLine("_objectMapper = objectMapper;");
                    sbForAppService.AppendLine($"_{itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository = {itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository;");
                    foreach (var item in checkedIocItems)
                    {
                        sbForAppService.AppendLine($"_{item.AttributeName.Substring(0, 1).ToLower()}{item.AttributeName.Substring(1)} = {item.AttributeName.Substring(0, 1).ToLower()}{item.AttributeName.Substring(1)};");
                    }
                    sbForAppService.AppendLine("}");//construction end

                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// 根据Id获取{itemForGenerator.EntityName}");
                    sbForAppService.AppendLine("/// </summary>");
                    sbForAppService.AppendLine("/// <param name=\"Id\">Id key</param>");
                    sbForAppService.AppendLine("/// <returns></returns>");
                    sbForAppService.AppendLine($"public async Task<{itemForGenerator.EntityName}Dto> Get{itemForGenerator.EntityName}ById(int Id)");
                    sbForAppService.AppendLine("{");//get by id start
                    sbForAppService.AppendLine($"return _objectMapper.Map<{itemForGenerator.EntityName}Dto>(_{itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository.Get(Id));");
                    sbForAppService.AppendLine("}");//get by id end

                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// 分页获取{itemForGenerator.EntityName}");
                    sbForAppService.AppendLine("/// </summary>");
                    sbForAppService.AppendLine("/// <param name=\"pagedSortedAndSearchInputDto\">分页过滤排序dto</param>");
                    sbForAppService.AppendLine("/// <returns></returns>");
                    sbForAppService.AppendLine($"public {itemForGenerator.EntityName}PagenationOutputDto Get{itemForGenerator.EntityName}ByPagenation({DefaultPagenationInputDto} pagedSortedAndSearchInputDto)");
                    sbForAppService.AppendLine("{");//pagenation start
                    sbForAppService.AppendLine($"var all = _{itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository.GetAll();");
                    sbForAppService.AppendLine("if (pagedSortedAndSearchInputDto.Search != null && pagedSortedAndSearchInputDto.Search.Trim().Length > 0)");
                    sbForAppService.AppendLine("{");//if start
                    sbForAppService.AppendLine("all = all.Where(item=>item.Name.Contains(pagedSortedAndSearchInputDto.Search));");
                    sbForAppService.AppendLine("}");//if end
                    sbForAppService.AppendLine("var total = all.Count();");
                    sbForAppService.AppendLine("if (pagedSortedAndSearchInputDto.Sort.Trim().Length > 0)");
                    sbForAppService.AppendLine("{");//if start
                    sbForAppService.AppendLine("all = all.OrderBy(string.Format(\"{0} {1}\", pagedSortedAndSearchInputDto.Sort, pagedSortedAndSearchInputDto.Order));");
                    sbForAppService.AppendLine("}");//if end
                    sbForAppService.AppendLine($"var list = all.Skip(pagedSortedAndSearchInputDto.Offset).Take(pagedSortedAndSearchInputDto.Limit).ToDynamicList<{itemForGenerator.EntityName}>();");
                    sbForAppService.AppendLine($"var listDto = _objectMapper.Map<List<{itemForGenerator.EntityName}Dto>>(list);");
                    sbForAppService.AppendLine($"{itemForGenerator.EntityName}PagenationOutputDto result = new {itemForGenerator.EntityName}PagenationOutputDto() {{ Rows = listDto,Total = total }};");
                    sbForAppService.AppendLine("return result;");
                    sbForAppService.AppendLine("}");//pagenation end

                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// 创建或更新{itemForGenerator.EntityName}");
                    sbForAppService.AppendLine("/// </summary>");
                    sbForAppService.AppendLine("/// <param name=\"dto\"></param>");
                    sbForAppService.AppendLine("/// <returns></returns>");
                    sbForAppService.AppendLine($"public Task<int> CreateOrUpdate{itemForGenerator.EntityName}({itemForGenerator.EntityName}Dto dto)");
                    sbForAppService.AppendLine("{");//create start
                    sbForAppService.AppendLine($"var entity = _objectMapper.Map<{itemForGenerator.EntityName}>(dto);");
                    sbForAppService.AppendLine($"return _{itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository.InsertOrUpdateAndGetIdAsync(entity);");
                    sbForAppService.AppendLine("}");//create end

                    sbForAppService.AppendLine("/// <summary>");
                    sbForAppService.AppendLine($"/// 删除{itemForGenerator.EntityName}");
                    sbForAppService.AppendLine("/// </summary>");
                    sbForAppService.AppendLine("/// <param name=\"id\"></param>");
                    sbForAppService.AppendLine("/// <returns></returns>");
                    sbForAppService.AppendLine($"public Task Delete{itemForGenerator.EntityName}ById(int id)");
                    sbForAppService.AppendLine("{");//delete start
                    sbForAppService.AppendLine($"var entity =  _{itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository.Get(id);");
                    sbForAppService.AppendLine("return _{itemForGenerator.EntityName.Substring(0, 1).ToLower()}{itemForGenerator.EntityName.Substring(1)}Repository.DeleteAsync(entity);");
                    sbForAppService.AppendLine("}");//delete end
                    sbForAppService.AppendLine("}");//class end
                    sbForAppService.AppendLine("}");//namespace end
                }
                #endregion
            }
            #endregion

            #region Html
            StringBuilder sbForHtml = new StringBuilder();
            if (itemForGenerator.GenerateHTML)
            {
                sbForHtml.AppendLine(@"@using System.Globalization
@using FengWo.Web.Startup
@{
    ViewBag.CurrentPageName = null; // The menu item will be active for this page.//请修改页名
}
<script src=""~/lib/jquery/dist/jquery.min.js""></script>
<link href=""~/lib/bootstrap-table/dist/bootstrap-table.css"" rel=""stylesheet"" />
<script src=""~/lib/bootstrap-table/dist/bootstrap-table.js""></script>
<script src=""~/lib/bootstrap-table/dist/bootstrap-table-locale-all.min.js""></script>
<script src=""~/lib/vue/dist/vue.min.js""></script>
");
                sbForHtml.AppendLine($@"
<div class=""row clearfix"">
    < div class=""col-lg-12 col-md-12 col-sm-12 col-xs-12"">
        <div class=""card"">
            <div class=""header"">
                <h2>
                    请修改标题
                </h2>
            </div>
            <div class=""body table-responsive"">

                <table id=""table"" ></table>

                <button type=""button"" class=""btn btn-primary waves-effect waves-float pull-right"" onclick=""Create()"" data-toggle=""modal"">
                    <i class=""material-icons"">add</i>新增</button>
            </div>
        </div>
    </div>
</div>
");//table

                StringBuilder sbEditModalInputs = new StringBuilder();
                foreach (var item in entityFieldItems)
                {
                    sbEditModalInputs.AppendLine($@"
                            <div class=""col-sm-12"">
                                <div class=""form-group form-float"">
                                    <div class=""form-line"">
                                        <input class=""form-control"" type=""text"" id=""{item.FieldName}"" v-model=""{item.FieldName}"" required minlength=""2"">
                                        <label class=""form-label"">{item.FieldSummary}</label>
                                    </div>
                                </div>
                            </div>");

                }

                sbForHtml.AppendLine($@"
<div class=""modal fade"" id=""CreateModalLabel"" tabindex=""-1"" role=""dialog"" aria-labelledby=""CreateModalLabel"" data-backdrop=""static"">
    <div class=""modal-dialog"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h4 class=""modal-title"">
                    <span>@L(""CreateSurvey"")</span>
                </h4>
            </div>
            <div id=""ForEditDiv"" class=""modal-body"">
                <div class=""tab-content"">
                    <div role=""tabpanel"" class=""tab-pane animated fadeIn active"" id=""create-tab"">
                        <div class=""row clearfix"" style=""margin-top:10px;"">
                            {sbEditModalInputs.ToString()}
                        </div>
                    </div>
                </div>
                <div class=""modal-footer"">
                    <button type=""button"" class=""btn btn-default waves-effect"" data-dismiss=""modal"">@L(""Cancel"")</button>
                    <button type=""submit"" id=""BtnSave"" onclick=""Save()"" class=""btn btn-primary waves-effect"">@L(""Save"")</button>
                </div>
            </div>
        </div>
    </div>
</div>
");//editModal
                sbForHtml.AppendLine("<script>");//script start
                sbForHtml.AppendLine("var obj = {};");
                sbForHtml.AppendLine("var modalVue = null;");
                sbForHtml.AppendLine("");
                sbForHtml.AppendLine("$(function () {");//$function start
                sbForHtml.AppendLine("obj.id=null;");
                foreach (var item in entityFieldItems)
                    sbForHtml.AppendLine($"obj.{item.FieldName}='';");
                sbForHtml.AppendLine("modalVue = new Vue({ el: '#ForEditDiv',data: obj});");
                sbForHtml.AppendLine("});");//$function end
                sbForHtml.AppendLine("var locale = '@CultureInfo.CurrentUICulture.Name';");
                sbForHtml.AppendLine("if (locale == 'zh-Hans') {locale = \"zh-CN\";}");
                sbForHtml.AppendLine("var table = $('#table');");
                sbForHtml.AppendLine("table.bootstrapTable({");//table start
                sbForHtml.AppendLine($"url: abp.appPath + 'api/services/app/{itemForGenerator.EntityName}/Get{itemForGenerator.EntityName}ByPagenation',");
                sbForHtml.AppendLine("locale:locale,");
                sbForHtml.AppendLine("queryParamsType: 'limit',sidePagination: 'server',silentSort: false,");
                sbForHtml.AppendLine("pagination: true,search: true,sortable: true,sortName: 'id',");
                sbForHtml.AppendLine("pageSize: 10,");

                StringBuilder sbForColumns = new StringBuilder();
                sbForColumns.AppendLine("columns: [");//columns start
                sbForColumns.AppendLine("{field: 'id',title: 'ID',visible: false,},");
                sbForColumns.AppendLine("{");//field start
                sbForColumns.AppendLine($"title: '@L(\"Actions\")',field:'actions',");
                sbForColumns.AppendLine("formatter: function (val, row, index) {");//formatter start
                sbForColumns.AppendLine("return '<a onclick=\"Update(\'' + row.id + '\')\" role=\"button\" ><i class=\"material-icons\">edit</i></a><a href=\"#\" onclick=\"DeleteById(\'' + row.id + '\')\" data-menu-id=\"' + row.id + '\"><i class=\"material-icons\">delete_sweep</i></a>';");
                sbForColumns.AppendLine("}");//formatter end
                sbForColumns.AppendLine("},");//field end

                foreach (var item in entityFieldItems)
                {
                    sbForColumns.AppendLine("{");
                    sbForColumns.AppendLine($"field: '{item.FieldName.ToLower()}', sortable: true, title: '@L(\"{item.FieldName}\")'");
                    sbForColumns.AppendLine("},");
                }
                sbForColumns.AppendLine("],");
                sbForHtml.AppendLine(sbForColumns.ToString());
                sbForHtml.AppendLine("responseHandler: function (res) {return res.result;}");
                sbForHtml.AppendLine("});");//table end
                sbForHtml.AppendLine("");
                sbForHtml.AppendLine("function Save() {");//save start
                sbForHtml.AppendLine($"abp.services.app.{itemForGenerator.EntityName.ToLower()}.createOrUpdate{itemForGenerator.EntityName}(obj, null).done(function () {{");
                sbForHtml.AppendLine("$('#CreateModalLabel').modal('hide');");
                sbForHtml.AppendLine("table.bootstrapTable('refresh');");
                sbForHtml.AppendLine("}).fail(function (data) {");
                sbForHtml.AppendLine("abp.message.error('保存失败! 请重试...', '提示', false);");
                sbForHtml.AppendLine("});");
                sbForHtml.AppendLine("}");//save end
                sbForHtml.AppendLine("");
                sbForHtml.AppendLine("function DeleteById(id) {");//delete start
                sbForHtml.AppendLine("abp.message.confirm('是否要删除此行数据?', '提示', function (data) {");
                sbForHtml.AppendLine("if (data) {");
                sbForHtml.AppendLine($"abp.services.app.{itemForGenerator.EntityName.ToLower()}.delete{itemForGenerator.EntityName}ById(id, null).done(function () {{");
                sbForHtml.AppendLine("table.bootstrapTable('refresh');");
                sbForHtml.AppendLine("abp.message.success('删除成功', '', false);");
                sbForHtml.AppendLine("}).fail(function () {");
                sbForHtml.AppendLine("table.bootstrapTable('refresh');");
                sbForHtml.AppendLine("abp.message.error('删除失败', '', false);");
                sbForHtml.AppendLine(" });");
                sbForHtml.AppendLine("}");
                sbForHtml.AppendLine("}, false);");
                sbForHtml.AppendLine("}");//delete end
                sbForHtml.AppendLine("");
                sbForHtml.AppendLine("function Create() {");//create start
                sbForHtml.AppendLine("obj.id=null;");
                foreach (var item in entityFieldItems)
                    sbForHtml.AppendLine($"obj.{item.FieldName}='';");
                sbForHtml.AppendLine("delete obj.id;");
                sbForHtml.AppendLine("$('#CreateModalLabel').modal('show');");
                sbForHtml.AppendLine("}");//create end
                sbForHtml.AppendLine("");
                sbForHtml.AppendLine("function Update(id) {");//update start
                sbForHtml.AppendLine($"abp.services.app.{itemForGenerator.EntityName.ToLower()}.get{itemForGenerator.EntityName}ById(id, null).done(function (data) {{");
                sbForHtml.AppendLine("obj.id = data.id;");
                foreach (var item in entityFieldItems)
                    sbForHtml.AppendLine($"obj.{item.FieldName} = data.{item.FieldName.Substring(0, 1).ToLower() + item.FieldName.Substring(1)};");
                sbForHtml.AppendLine("$('#CreateModalLabel').modal('show');");
                sbForHtml.AppendLine("}).fail(function (data) {");
                sbForHtml.AppendLine("abp.message.error('修改失败', '', false);");
                sbForHtml.AppendLine("});");
                sbForHtml.AppendLine("}");//update end
                sbForHtml.AppendLine("</script>");//script start
            }
            #endregion

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                #region 添加IOC
                var item = new IocItem()
                {
                    AttributeName = itemForGenerator.EntityName.Substring(0, 1).ToLower() + itemForGenerator.EntityName.Substring(1) + $"Repository",
                    Code = $"IRepository<{itemForGenerator.EntityName }>",
                    Summary = itemForGenerator.EntitySummary,
                    Namespace = itemForGenerator.EntityNamespace
                };

                iocItems.Add(item);

                File.WriteAllText(iocItems_path, JsonConvert.SerializeObject(iocItems));
                #endregion

                var folder = dialog.FileName;
                File.WriteAllText($"{folder}/{itemForGenerator.EntityName}.cs", sbForGenerateEntity.ToString());

                if (itemForGenerator.GenerateDto)
                    File.WriteAllText($"{folder}/{itemForGenerator.EntityName}Dto.cs", sbForDto.ToString());

                if (itemForGenerator.GenerateAPI)
                {
                    File.WriteAllText($"{folder}/{itemForGenerator.EntityName}PagenationOutputDto.cs", sbForPagenationDto.ToString());
                    File.WriteAllText($"{folder}/I{itemForGenerator.EntityName}AppService.cs", sbForIAppService.ToString());
                    File.WriteAllText($"{folder}/{itemForGenerator.EntityName}AppService.cs", sbForAppService.ToString());
                }
                if (itemForGenerator.GenerateHTML)
                    File.WriteAllText($"{folder}/Index.cshtml", sbForHtml.ToString());
                MessageBox.Show("保存完成");
                try
                {
                    System.Diagnostics.Process.Start(folder);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// API checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkGenerateAPI_Checked(object sender, RoutedEventArgs e)
        {
            listIocInject.IsEnabled = checkGenerateAPI.IsChecked ?? false;
            if (listIocInject.IsEnabled)
                itemForGenerator.GenerateDto = true;//API must be generate the dto
        }
        #endregion
    }
}
