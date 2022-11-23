using nkv.Automator.Generator.Models;
using nkv.Automator.Models;
using nkv.Automator.Postman;
using nkv.Automator.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Generator.MySQL
{
    public class AngularTs_NodeJSMySQL
    {
        static TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public List<Exception> ExList { get; set; }
        Dictionary<string, List<AngularTSRepo>> AngularRepoDic { get; set; }
        Dictionary<string, FinalQueryData> FinalDataDic { get; set; }
        public string TemplateFolder { get; set; } = null!;
        public string TemplateFolderSeparator { get; set; } = null!;
        public string DestinationFolderSeparator { get; set; } = null!;
        public string DestinationFolder { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public Action<NKVMessage> MessageEvent { get; set; } = null!;
        public Action<NKVMessage> CompletedEvent { get; set; } = null!;
        public AngularTs_NodeJSMySQL(string projectName, string destinationFolder, string destinationFolderSeparator)
        {
            DestinationFolder = destinationFolder;
            ProjectName = projectName;
            ExList = new List<Exception>();
            TemplateFolder = "AngularTsNodeJSTemplate";
            TemplateFolderSeparator = "\\";
            DestinationFolderSeparator = destinationFolderSeparator;
            AngularRepoDic = new Dictionary<string, List<AngularTSRepo>>();
            ExList = new List<Exception>();
        }
        private string CreateTemplatePath(string filePathString)
        {
            string path = TemplateFolder;
            foreach (var p in filePathString.Split(","))
            {
                if (!string.IsNullOrEmpty(p))
                    path = path + TemplateFolderSeparator + p;
            }
            return path;
        }
        private string CreateDestinationPath(string filePathString)
        {
            string path = DestinationFolder;
            foreach (var p in filePathString.Split(","))
            {
                if (!string.IsNullOrEmpty(p))
                {
                    path = path + DestinationFolderSeparator + p;
                    if (!path.Contains(".") && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }

            return path;
        }
        private string CreateDirectory()
        {
            DestinationFolder = DestinationFolder + "//AngularAPP";
            Directory.CreateDirectory(DestinationFolder);
            CopyDir.Copy(CreateTemplatePath("AngularAppProject"), DestinationFolder, ProjectName, "nkvAngularAdmin");
            return DestinationFolder;
        }
        public List<Exception> CreateAngularApp(ReactJSInput<FinalQueryData> reactInput)
        {
            var angularData = GetAllTableData(reactInput);
            MessageEvent?.Invoke(new NKVMessage("Creating Angular Project"));
            CreateDirectory();
            MessageEvent?.Invoke(new NKVMessage("Creating Angular Routes Files"));
            CreateRouteFile(angularData);
            CreateAuthTSFile("/api/token");
            MessageEvent?.Invoke(new NKVMessage("Creating Interface Files"));
            CreateInterface(angularData);
            MessageEvent?.Invoke(new NKVMessage("Creating Upload Files"));
            CreateUploadService("/upload", "/upload");
            foreach (var t in angularData)
            {
                try
                {
                    MessageEvent?.Invoke(new NKVMessage("Creating Table Files : " + t.TableName));
                    CreateTableDirectory(t);
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage(ex.Message));
                }
            }
            CompletedEvent?.Invoke(new NKVMessage("Thanks for using GetAutomator.com! Please check the generated code at : " + DestinationFolder));
            return ExList;
        }
        public void CreateUploadService(string fileUploadURL, string imageUploadURL)
        {
            string path = CreateDestinationPath("src,app,service,upload.service.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string contents = File.ReadAllText(CreateTemplatePath("upload-service.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{fileUploadURL}", fileUploadURL);
                contents = contents.Replace("{imageUploadURL}", imageUploadURL);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateAuthTSFile(string tokenURL)
        {
            string path = CreateDestinationPath("src,app,api,auth.service.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string contents = File.ReadAllText(CreateTemplatePath("auth.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tokenURL}", tokenURL);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateTableDirectory(AngularTSTableData t)
        {
            string tableDirPath = "src,app,admin," + t.TableName;
            CreateDestinationPath(tableDirPath);
            CreateTableRouting(tableDirPath, t);
            CreateTableModule(tableDirPath, t);

            string listDirPath = tableDirPath + ",list";
            string manageDirPath = tableDirPath + ",manage";
            CreateTableService(t);
            CreateListComponent(listDirPath, t);
            CreateListCSS(listDirPath, t);
            CreateListHTML(listDirPath, t);
            CreateFormCSS(manageDirPath, t);
            CreateFormComponent(manageDirPath, t);
            CreateFormHTML(manageDirPath, t);
        }
        public void CreateFormHTML(string manageDirPath, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(manageDirPath + "," + tableData.TableName + "-form.component.html");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var tableName = tableData.TableName;
            var modelName = ti.ToTitleCase(tableData.TableName);
            var tableColumnNgFormField = "";
            foreach (var c in tableData.TableColumn)
            {
                var columnName = c.ActualColumnName;
                var dataType = Helper.GetDataTypeTypeForUI(c.PropDataType);

                List<string> enumList = new List<string>();

                string output = "-1";
                if (dataType.Contains("("))
                {
                    output = dataType.Split('(', ')')[1];

                    dataType = dataType.Split('(', ')')[0];
                }
                if (dataType.ToLower().Contains("enum") && output != "-1")
                {
                    var oArray = output.Split(',');
                    foreach (var o in oArray)
                    {
                        enumList.Add(o.Replace("\'", ""));
                    }
                }
                if (enumList.Count() > 0)
                {
                    tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "<mat-label>" + columnName + "</mat-label>" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "<select matNativeControl formControlName=\"" + columnName + "\" id=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" class=\"form-control\">" + Environment.NewLine;

                    foreach (var e in enumList)
                    {
                        tableColumnNgFormField = tableColumnNgFormField + "<option value=\"" + e + "\">" + e + "</option>" + Environment.NewLine;
                    }
                    tableColumnNgFormField = tableColumnNgFormField + "</select>" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                }
                else if (string.IsNullOrEmpty(c.RefColumnName))
                {
                    switch (dataType)
                    {

                        case "string":
                            int length = 0;
                            int.TryParse(output, out length);
                            if (length > 250)
                            {
                                tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "<textarea matInput class=\"form-control\" placeholder=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" formControlName=\"" + columnName + "\" id=\"" + columnName + "\"></textarea>" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                            }
                            else
                            {
                                var controlType = "text";
                                if (columnName.ToLower().Contains("password"))
                                {
                                    controlType = "password";
                                }
                                else if (columnName.ToLower().Contains("email"))
                                {
                                    controlType = "email";
                                }
                                tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "<input type=\"" + controlType + "\" matInput formControlName=\"" + columnName + "\" id=\"" + columnName + "\" class=\"form-control\" placeholder=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" />" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                            }
                            break;
                        case "number":
                            tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + "<input type=\"number\" matInput formControlName=\"" + columnName + "\" id=\"" + columnName + "\" class=\"form-control\" placeholder=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" />" + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                            break;
                        case "datetime":
                            tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + "<input matInput formControlName=\"" + columnName + "\" id=\"" + columnName + "\" [matDatepicker]=\"picker\" class=\"form-control\" placeholder=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" />" + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + "<mat-datepicker-toggle matSuffix [for]=\"picker\"></mat-datepicker-toggle>" + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + "<mat-datepicker touchUi #picker></mat-datepicker>" + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                            break;
                        case "boolean":
                            tableColumnNgFormField = tableColumnNgFormField + "<mat-slide-toggle class=\"form-control\" checked=\"checked\" [color]=\"color\" [(ngModel)]=\"data." + columnName + "\" formControlName=\"" + columnName + "\" id=\"" + columnName + "\">" + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + columnName + Environment.NewLine;
                            tableColumnNgFormField = tableColumnNgFormField + "</mat-slide-toggle>" + Environment.NewLine;
                            break;
                        default:
                            int lengthStr = 0;
                            int.TryParse(output, out lengthStr);
                            if (lengthStr > 250)
                            {
                                tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "<textarea matInput class=\"form-control\" placeholder=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" formControlName=\"" + columnName + "\" id=\"" + columnName + "\"></textarea>" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                            }
                            else
                            {
                                var controlType = "text";
                                if (columnName.ToLower().Contains("password"))
                                {
                                    controlType = "password";
                                }
                                else if (columnName.ToLower().Contains("email"))
                                {
                                    controlType = "email";
                                }
                                tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "<input type=\"" + controlType + "\" matInput formControlName=\"" + columnName + "\" id=\"" + columnName + "\" class=\"form-control\" placeholder=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" />" + Environment.NewLine;
                                tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                            }
                            break;
                    }
                }
                else
                {
                    var fkColumnName = c.RefColumnName;
                    var fkDisplayColumn = c.PropName;
                    var fkListVariable = c.RefTableName + "_FK" + c.PropName.ToLower() + "List";
                    tableColumnNgFormField = tableColumnNgFormField + "<mat-form-field class=\"full-width\">" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "<mat-label>" + columnName + "</mat-label>" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "<mat-select formControlName=\"" + columnName + "\" id=\"" + columnName + "\" [(ngModel)]=\"data." + columnName + "\" class=\"form-control\">" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "<mat-option *ngFor=\"let r of " + fkListVariable + "\" [value]=\"r." + fkColumnName + "\">" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "{{r." + fkDisplayColumn + "}}" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "</mat-option>" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "</mat-select>" + Environment.NewLine;
                    tableColumnNgFormField = tableColumnNgFormField + "</mat-form-field>" + Environment.NewLine;
                }

            }
            string contents = File.ReadAllText(CreateTemplatePath("form.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{modelName}", modelName);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{tableColumnNgFormField}", tableColumnNgFormField);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateFormComponent(string manageDirPath, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(manageDirPath + "," + tableData.TableName + "-form.component.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var tableName = tableData.TableName;
            var modelName = ti.ToTitleCase(tableData.TableName);
            var tableServiceInstance = tableName + "Service";
            var tableColumnValidation = "";
            var primaryKeyParamValue = "";
            var primaryKeyParamArgument = "";
            var primaryKeyParamCheck = "";
            var foreignKeyPram = "";
            var foreignKeyPramInit = "";
            string primaryKey = "";
            var fetchForeignKeyData = "";
            var foreignKeyServiceInstance = "";
            var tableAddEditComponent = "AddEdit" + modelName + "Component";
            var foreignKeyServiceImport = "";
            foreach (var p in tableData.PrimaryKey)
            {
                if (!string.IsNullOrEmpty(primaryKeyParamArgument))
                {
                    primaryKeyParamArgument = primaryKeyParamArgument + "," + p.PropName;
                }
                else
                {
                    primaryKeyParamArgument = p.PropName;
                }
                primaryKeyParamValue = primaryKeyParamValue + "const " + p.PropName + "= params['" + p.PropName + "']" + Environment.NewLine;
                primaryKey = p.PropName;
                if (!string.IsNullOrEmpty(primaryKeyParamCheck))
                {
                    primaryKeyParamCheck = primaryKeyParamCheck + " && " + p.PropName;
                }
                else
                {
                    primaryKeyParamCheck = p.PropName + " && " + p.PropName + "!='add'";
                }
            }
            foreach (var c in tableData.TableColumn)
            {
                if (string.IsNullOrEmpty(c.RefColumnName))
                {
                    if (c.IsRequred)
                    {
                        tableColumnValidation = tableColumnValidation + "'" + c.ActualColumnName + "': new FormControl(this.data." + c.ActualColumnName + ", [Validators.required])," + Environment.NewLine;
                    }
                    else
                    {
                        tableColumnValidation = tableColumnValidation + "'" + c.ActualColumnName + "': new FormControl(this.data." + c.ActualColumnName + ", [])," + Environment.NewLine;

                    }
                }
                else
                {

                    foreignKeyPram = foreignKeyPram + c.RefTableName + "_FK" + c.PropName.ToLower() + "List" + ":any[] =[];" + Environment.NewLine;
                    var fkTableName = c.RefTableName;
                    var fkModelName = ti.ToTitleCase(c.RefTableName);

                    fetchForeignKeyData = fetchForeignKeyData + "this." + fkTableName + "Service.get" + fkModelName + "(1, 200, '').then((res: any) => {" + Environment.NewLine;
                    fetchForeignKeyData = fetchForeignKeyData + " if (res.code === 1) {" + Environment.NewLine;
                    fetchForeignKeyData = fetchForeignKeyData + "   this." + c.RefTableName + "_FK" + c.PropName.ToLower() + "List" + " = res.document.records;" + Environment.NewLine;
                    fetchForeignKeyData = fetchForeignKeyData + " } else {" + Environment.NewLine;
                    fetchForeignKeyData = fetchForeignKeyData + "    this." + c.RefTableName + "_FK" + c.PropName.ToLower() + "List" + " = [];" + Environment.NewLine;
                    fetchForeignKeyData = fetchForeignKeyData + "}});" + Environment.NewLine;

                    if (!string.IsNullOrEmpty(foreignKeyServiceInstance) && !foreignKeyServiceInstance.ToLower().Contains(fkTableName.ToLower()))
                    {
                        foreignKeyServiceInstance = foreignKeyServiceInstance + Environment.NewLine + "private " + fkTableName + "Service: " + fkModelName + "Service,";
                    }
                    else
                    {
                        foreignKeyServiceInstance = "private " + fkTableName + "Service: " + fkModelName + "Service,";
                    }
                    if (!string.IsNullOrEmpty(foreignKeyServiceImport) && !foreignKeyServiceImport.ToLower().Contains(fkTableName.ToLower()))
                    {
                        foreignKeyServiceImport = foreignKeyServiceImport + Environment.NewLine + "import { " + fkModelName + "Service } from '../../../service/" + fkTableName + ".service'";
                    }
                    else
                    {
                        foreignKeyServiceImport = "import { " + fkModelName + "Service } from '../../../service/" + fkTableName + ".service'";
                    }
                }


            }

            string contents = File.ReadAllText(CreateTemplatePath("form-component.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{modelName}", modelName);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{tableServiceInstance}", tableServiceInstance);
                contents = contents.Replace("{tableAddEditComponent}", tableAddEditComponent);

                contents = contents.Replace("{foreignKeyPram}", foreignKeyPram);
                contents = contents.Replace("{foreignKeyPramInit}", foreignKeyPramInit);
                contents = contents.Replace("{fetchForeignKeyData}", fetchForeignKeyData);
                contents = contents.Replace("{foreignKeyServiceInstance}", foreignKeyServiceInstance);
                contents = contents.Replace("{foreignKeyServiceImport}", foreignKeyServiceImport);
                contents = contents.Replace("{tableColumnValidation}", tableColumnValidation);
                contents = contents.Replace("{primaryKey}", primaryKey);
                contents = contents.Replace("{primaryKeyParamValue}", primaryKeyParamValue);
                contents = contents.Replace("{primaryKeyParamCheck}", primaryKeyParamCheck);
                contents = contents.Replace("{primaryKeyParamArgument}", primaryKeyParamArgument);

                txtFile.WriteLine(contents);
            }
        }

        public void CreateFormCSS(string manageDirPath, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(manageDirPath + "," + tableData.TableName + "-form.component.scss");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string contents = File.ReadAllText(CreateTemplatePath("form-css.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }
        }

        public void CreateTableRouting(string tableDirPath, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(tableDirPath + "," + tableData.TableName + "-routing.module.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var modelName = ti.ToTitleCase(tableData.TableName);
            var tableRoutingModuleName = modelName + "RoutingModule";
            var tableListComponent = modelName + "Component";
            var tableAddEditComponent = "AddEdit" + modelName + "Component";
            var primaryKeyParam = "";
            foreach (var p in tableData.PrimaryKey)
            {
                if (!string.IsNullOrEmpty(primaryKeyParam))
                {
                    primaryKeyParam = primaryKeyParam + "/:" + p.PropName;
                }
                else
                {
                    primaryKeyParam = ":" + p.PropName;
                }
            }
            string contents = File.ReadAllText(CreateTemplatePath("routing.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableRoutingModuleName}", tableRoutingModuleName);
                contents = contents.Replace("{tableListComponent}", tableListComponent);
                contents = contents.Replace("{tableAddEditComponent}", tableAddEditComponent);
                contents = contents.Replace("{tableName}", tableData.TableName);
                contents = contents.Replace("{primaryKeyParam}", primaryKeyParam);
                contents = contents.Replace("{modelName}", modelName);

                txtFile.WriteLine(contents);
            }
        }
        public void CreateTableModule(string tableDirPath, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(tableDirPath + "," + tableData.TableName + ".module.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var modelName = ti.ToTitleCase(tableData.TableName);
            var tableRoutingModuleName = modelName + "RoutingModule";
            var tableModuleName = modelName + "Module";
            var tableListComponent = modelName + "Component";
            var tableAddEditComponent = "AddEdit" + modelName + "Component";
            var primaryKeyParam = "";
            foreach (var p in tableData.PrimaryKey)
            {
                if (!string.IsNullOrEmpty(primaryKeyParam))
                {
                    primaryKeyParam = primaryKeyParam + "/:" + p.PropName;
                }
                else
                {
                    primaryKeyParam = ":" + p.PropName;
                }
            }
            string contents = File.ReadAllText(CreateTemplatePath("module.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tableRoutingModuleName}", tableRoutingModuleName);
                contents = contents.Replace("{tableListComponent}", tableListComponent);
                contents = contents.Replace("{tableAddEditComponent}", tableAddEditComponent);
                contents = contents.Replace("{tableName}", tableData.TableName);
                contents = contents.Replace("{primaryKeyParam}", primaryKeyParam);
                contents = contents.Replace("{modelName}", modelName);
                contents = contents.Replace("{tableModuleName}", tableModuleName);

                txtFile.WriteLine(contents);
            }
        }
        public void CreateListComponent(string listDirPart, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(listDirPart + "," + tableData.TableName + ".component.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var tableName = tableData.TableName;
            var modelName = ti.ToTitleCase(tableName);
            var modelInterface = "I" + modelName;
            var tableServiceInstance = tableName + "Service";
            var primaryKey = "";
            var primaryKeyURL = "";
            foreach (var p in tableData.PrimaryKey)
            {
                if (!string.IsNullOrEmpty(primaryKey))
                {
                    primaryKey = primaryKey + ",row." + p.PropName;
                }
                else
                {
                    primaryKey = "row." + p.PropName;
                }
                if (!string.IsNullOrEmpty(primaryKeyURL))
                {
                    primaryKeyURL = primaryKeyURL + "+\"/\"+row." + p.PropName;
                }
                else
                {
                    primaryKeyURL = "row." + p.PropName;
                }
            }
            var tableColumnList = "";
            foreach (var p in tableData.PrimaryKey)
            {
                var pDataType = Helper.GetDataTypeTypeScript(p.PropDataType);
                tableColumnList = tableColumnList + "\"" + p.PropName + "\"," + Environment.NewLine;
            }
            foreach (var c in tableData.TableColumn)
            {
                if (!string.IsNullOrEmpty(c.RefColumnName))
                {
                    tableColumnList = tableColumnList + "\"" + c.PropName + "\"," + Environment.NewLine;
                }
                else
                {
                    tableColumnList = tableColumnList + "\"" + c.ActualColumnName + "\"," + Environment.NewLine;
                }
            }
            string contents = File.ReadAllText(CreateTemplatePath("list-component.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{modelName}", modelName);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{modelInterface}", modelInterface);
                contents = contents.Replace("{tableServiceInstance}", tableServiceInstance);
                contents = contents.Replace("{primaryKeyURL}", primaryKeyURL);
                contents = contents.Replace("{primaryKey}", primaryKey);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{tableColumnList}", tableColumnList);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateTableService(AngularTSTableData tableData)
        {
            string path = CreateDestinationPath("src,app,service," + tableData.TableName + ".service.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var tableName = tableData.TableName;
            var modelName = ti.ToTitleCase(tableName);
            var readAllURL = "";
            var readOneURL = "";
            var searchURL = "search";
            var createURL = "";
            var updateURL = "";
            var deleteURL = "";
            var primaryKeyParam = "";
            var primaryKeyURLParam = "";
            foreach (var p in tableData.PrimaryKey)
            {
                if (!string.IsNullOrEmpty(primaryKeyParam))
                {
                    primaryKeyParam = primaryKeyParam + "," + p.PropName;
                }
                else
                {
                    primaryKeyParam = p.PropName;
                }
                if (!string.IsNullOrEmpty(primaryKeyURLParam))
                {
                    primaryKeyURLParam = primaryKeyURLParam + "\"/\"" + "+" + p.PropName;
                }
                else
                {
                    primaryKeyURLParam = "+" + p.PropName;
                }
            }
            string contents = File.ReadAllText(CreateTemplatePath("service.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{modelName}", modelName);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{readAllURL}", readAllURL);
                contents = contents.Replace("{searchURL}", searchURL);
                contents = contents.Replace("{readOneURL}", readOneURL);
                contents = contents.Replace("{createURL}", createURL);
                contents = contents.Replace("{updateURL}", updateURL);
                contents = contents.Replace("{deleteURL}", deleteURL);
                contents = contents.Replace("{primaryKeyParam}", primaryKeyParam);
                contents = contents.Replace("{primaryKeyURLParam}", primaryKeyURLParam);
                contents = contents.Replace("{primaryKeyParam}", primaryKeyParam);

                txtFile.WriteLine(contents);
            }
        }
        public void CreateListCSS(string listDirPart, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(listDirPart + "," + tableData.TableName + ".component.scss");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string contents = File.ReadAllText(CreateTemplatePath("list-css.txt"));
            using (var txtFile = File.AppendText(path))
            {
                txtFile.WriteLine(contents);
            }
        }
        public void CreateListHTML(string listDirPart, AngularTSTableData tableData)
        {
            string path = CreateDestinationPath(listDirPart + "," + tableData.TableName + ".component.html");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var tableName = tableData.TableName;
            var modelName = ti.ToTitleCase(tableName);
            var tableColumnNgContainer = "";
            foreach (var p in tableData.PrimaryKey)
            {
                tableColumnNgContainer = tableColumnNgContainer + "<ng-container matColumnDef='" + p.PropName + "'>" + Environment.NewLine;
                tableColumnNgContainer = tableColumnNgContainer + "<th mat-header-cell *matHeaderCellDef mat-sort-header> " + p.PropName + " </th>" + Environment.NewLine;
                tableColumnNgContainer = tableColumnNgContainer + "<td mat-cell *matCellDef='let row'> {{row." + p.PropName + "}} </td>" + Environment.NewLine;
                tableColumnNgContainer = tableColumnNgContainer + "</ng-container>" + Environment.NewLine;
            }
            foreach (var c in tableData.TableColumn)
            {
                if (!string.IsNullOrEmpty(c.RefColumnName))
                {
                    tableColumnNgContainer = tableColumnNgContainer + "<ng-container matColumnDef='" + c.PropName + "'>" + Environment.NewLine;
                    tableColumnNgContainer = tableColumnNgContainer + "<th mat-header-cell *matHeaderCellDef mat-sort-header> " + c.PropName + " </th>" + Environment.NewLine;
                    tableColumnNgContainer = tableColumnNgContainer + "<td mat-cell *matCellDef='let row'> {{row." + c.PropName + "}} </td>" + Environment.NewLine;
                    tableColumnNgContainer = tableColumnNgContainer + "</ng-container>" + Environment.NewLine;
                }
                else
                {
                    tableColumnNgContainer = tableColumnNgContainer + "<ng-container matColumnDef='" + c.ActualColumnName + "'>" + Environment.NewLine;
                    tableColumnNgContainer = tableColumnNgContainer + "<th mat-header-cell *matHeaderCellDef mat-sort-header> " + c.ActualColumnName + " </th>" + Environment.NewLine;
                    tableColumnNgContainer = tableColumnNgContainer + "<td mat-cell *matCellDef='let row'> {{row." + c.ActualColumnName + "}} </td>" + Environment.NewLine;
                    tableColumnNgContainer = tableColumnNgContainer + "</ng-container>" + Environment.NewLine;
                }
            }
            string contents = File.ReadAllText(CreateTemplatePath("list.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{modelName}", modelName);
                contents = contents.Replace("{tableName}", tableName);
                contents = contents.Replace("{tableColumnNgContainer}", tableColumnNgContainer);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateRouteFile(List<AngularTSTableData> tableAllData)
        {
            string path = CreateDestinationPath("src,app,admin,child-routes.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var childRouteList = "";
            foreach (var t in tableAllData)
            {
                var modelName = ti.ToTitleCase(t.TableName);
                childRouteList = Environment.NewLine + childRouteList + "{";
                childRouteList = Environment.NewLine + childRouteList + "   path: '" + t.TableName + "',";
                childRouteList = Environment.NewLine + childRouteList + "   loadChildren: () =>";
                childRouteList = Environment.NewLine + childRouteList + "     import('./" + t.TableName + "/" + t.TableName + ".module').then(m => m." + modelName + "Module),";
                childRouteList = Environment.NewLine + childRouteList + "   data: { icon: 'table_chart', text: '" + modelName + "' }";
                childRouteList = Environment.NewLine + childRouteList + "},";
            }
            string contents = File.ReadAllText(CreateTemplatePath("routes.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{childRouteList}", childRouteList);
                txtFile.WriteLine(contents);
            }
        }
        public void CreateInterface(List<AngularTSTableData> tableAllData)
        {


            foreach (var t in tableAllData)
            {

                var modelName = ti.ToTitleCase(t.TableName);
                var modelInterface = "I" + modelName;
                var tableColumnForInterface = string.Empty;
                string path = CreateDestinationPath("src,app,interface,I" + modelName + ".ts");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                foreach (var p in t.PrimaryKey)
                {
                    var pDataType = Helper.GetDataTypeTypeScript(p.PropDataType);
                    tableColumnForInterface = tableColumnForInterface + p.PropName + ":" + " " + pDataType + ";" + Environment.NewLine;
                }
                foreach (var c in t.TableColumn)
                {
                    if (!string.IsNullOrEmpty(c.RefColumnName))
                    {
                        var dataType = Helper.GetDataTypeTypeScript(c.PropDataType);
                        tableColumnForInterface = tableColumnForInterface + c.PropName + ":" + " " + dataType + ";" + Environment.NewLine;
                    }
                    else
                    {
                        var dataType = Helper.GetDataTypeTypeScript(c.PropDataType);
                        tableColumnForInterface = tableColumnForInterface + c.ActualColumnName + ":" + " " + dataType + ";" + Environment.NewLine;
                    }
                }

                string contents = File.ReadAllText(CreateTemplatePath("interface.txt"));
                using (var txtFile = File.AppendText(path))
                {
                    contents = contents.Replace("{modelInterface}", modelInterface);
                    contents = contents.Replace("{tableColumnForInterface}", tableColumnForInterface);
                    txtFile.WriteLine(contents);
                }
            }

        }
        public List<AngularTSTableData> GetAllTableData(ReactJSInput<FinalQueryData> reactInput)
        {
            List<AngularTSTableData> rjTable = new List<AngularTSTableData>();
            foreach (var t in reactInput.FinalDataDic)
            {
                string tableName = t.Key;
                var data = t.Value;
                AngularTSTableData rjsTableData = new AngularTSTableData();
                rjsTableData.TableName = tableName;
                foreach (var c in data.SelectQueryData.ColumnList)
                {
                    rjsTableData.TableColumn.Add(new PRequestRefBody(c.Field, tableName, c.Field, c.TypeName, "", "", (c.IsNull == "NO" ? true : false), c.DefaultValue));
                }
                foreach (var c in data.SelectQueryData.JoinQueryData)
                {
                    var refColumnName = c.Column2Data;
                    rjsTableData.TableColumn.Add(new PRequestRefBody(c.FieldName1, tableName, refColumnName.Field, refColumnName.TypeName, c.FieldName2, c.TableName2, (refColumnName.IsNull == "NO" ? true : false), refColumnName.DefaultValue));
                }
                if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count > 1)
                {
                    foreach (var p in data.SelectQueryData.PrimaryKeys)
                    {
                        rjsTableData.PrimaryKey.Add(new PRequestBody(p.FieldName, p.DataType, false, "auto_increment"));
                    }

                }
                else if (data.SelectQueryData.PrimaryKeys != null && data.SelectQueryData.PrimaryKeys.Count == 1)
                {
                    var p = data.SelectQueryData.PrimaryKeys[0];
                    rjsTableData.PrimaryKey.Add(new PRequestBody(p.FieldName, p.DataType, false, "auto_increment"));
                }
                else
                {
                    var p = data.SelectQueryData.ColumnList[0];
                    rjsTableData.PrimaryKey.Add(new PRequestBody(p.Field, p.TypeName, false, "auto_increment"));
                }

                rjsTableData.TableColumn.RemoveAll(item => rjsTableData.PrimaryKey.Select(y => y.PropName).Contains(item.PropName));
                rjTable.Add(rjsTableData);
            }

            return rjTable;
        }

    }
}
