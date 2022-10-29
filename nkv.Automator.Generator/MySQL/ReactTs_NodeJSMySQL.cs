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
    public class ReactTs_NodeJSMySQL
    {
        public List<Exception> ExList { get; set; }
        Dictionary<string, List<ReactJSRepo>> ReactRepoDic { get; set; }
        Dictionary<string, FinalQueryData> FinalDataDic { get; set; }
        public string TemplateFolder { get; set; } = null!;
        public string TemplateFolderSeparator { get; set; } = null!;
        public string DestinationFolderSeparator { get; set; } = null!;
        public string DestinationFolder { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public Action<NKVMessage> MessageEvent { get; set; } = null!;
        public Action<NKVMessage> CompletedEvent { get; set; } = null!;
        public ReactTs_NodeJSMySQL(string projectName, string destinationFolder, string destinationFolderSeparator)
        {
            DestinationFolder = destinationFolder;
            ProjectName = projectName;
            ExList = new List<Exception>();
            TemplateFolder = "ReactTsNodeJSTemplate";
            TemplateFolderSeparator = "\\";
            DestinationFolderSeparator = destinationFolderSeparator;
            ReactRepoDic = new Dictionary<string, List<ReactJSRepo>>();
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
                    path = path + DestinationFolderSeparator + p;
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return path;
        }
        static TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        public void CreateRepoDictionary(List<PostmanModel> PostmanModels)
        {
            Dictionary<string, List<ReactJSRepo>> dicRepo = new Dictionary<string, List<ReactJSRepo>>();
            List<ReactJSRepo> list = new List<ReactJSRepo>();
            string prevTableName = "";
            foreach (var p in PostmanModels.OrderBy(i => i.TableName))
            {
                try
                {
                    string tableName = p.TableName;
                    if (prevTableName != tableName)
                    {
                        list = new List<ReactJSRepo>();
                        prevTableName = tableName;
                        dicRepo.Add(tableName, list);
                    }
                    string methodName = p.Method.ToLower();
                    string requestURL = p.Url;
                    string tableModel = ti.ToTitleCase(tableName);
                    string finalURL = "";
                    string paramURL = "";
                    ReactJSRepo repo = new ReactJSRepo();

                    foreach (var path in p.Path)
                    {
                        if (path.Contains("?"))
                        {
                            var mainPath = path.Split('?');
                            finalURL = finalURL + "/" + mainPath[0];
                            foreach (string item in mainPath[1].Split('&'))
                            {
                                string[] parts = item.Replace("?", "").Split('=');
                                paramURL = paramURL + "&" + parts[0] + "=${" + parts[0] + "}";
                                repo.getList.Add(parts[0]);
                            }
                        }
                        else
                        {
                            finalURL = finalURL + "/" + path;
                        }
                    }
                    if (!string.IsNullOrEmpty(paramURL))
                        finalURL = finalURL + "?" + paramURL.Trim('&');
                    repo.methodName = p.Method.ToLower();
                    if (p.Name.ToLower().Contains("- getall"))
                    {
                        repo.functionType = "getall";
                        repo.functionName = "getAll" + tableModel;
                        repo.finalURL = finalURL;
                    }
                    else if (p.Name.ToLower().Contains("- getbyid"))
                    {
                        repo.functionType = "getbyid";
                        repo.functionName = "getOne" + tableModel;
                        repo.finalURL = finalURL;
                    }
                    else if (p.Name.ToLower().Contains("- search") && dicRepo[tableName].Where(i => i.functionType == "search").FirstOrDefault() == null)
                    {
                        repo.functionType = "search";
                        repo.functionName = "search" + tableModel;
                        repo.finalURL = finalURL;
                    }
                    else if (p.Name.ToLower().Contains("- delete"))
                    {
                        //repo.getList.Add(fina);
                        //repo.postParamList = p.Body;
                        repo.functionType = "delete";
                        repo.functionName = "delete" + tableModel;
                        repo.finalURL = finalURL;
                    }
                    else if (p.Name.ToLower().Contains("- add"))
                    {
                        repo.postParamList = new List<PRequestBody>();
                        repo.postParamList.Add(new PRequestBody("data", "object", false, null));
                        repo.functionType = "add";
                        repo.functionName = "add" + tableModel;
                        repo.finalURL = finalURL;
                    }
                    else if (p.Name.ToLower().Contains("- update") && dicRepo[tableName].Where(i => i.functionType == "update").FirstOrDefault() == null)
                    {
                        repo.postParamList = new List<PRequestBody>();
                        repo.postParamList.Add(new PRequestBody("data", "object", false, null));
                        repo.functionType = "update";
                        repo.functionName = "update" + tableModel;
                        repo.finalURL = finalURL;
                    }
                    if (!string.IsNullOrEmpty(repo.functionType))
                        dicRepo[tableName].Add(repo);
                }
                catch (Exception ex)
                {

                }
            }
            ReactRepoDic = dicRepo;
        }
        private string CreateDirectory()
        {
            DestinationFolder = DestinationFolder + "//ReactAPP";
            Directory.CreateDirectory(DestinationFolder);
            CopyDir.Copy(CreateTemplatePath("ReactTsProject"), DestinationFolder, ProjectName, "{projectName}");
            return DestinationFolder;
        }
        private void CreateServiceFile()
        {
            string repoContent = "export const {functionName} = ({paramList}) => {" + Environment.NewLine;
            repoContent = repoContent + "return APIService.api().{methodName}(`{apiURL}`{paramData})" + Environment.NewLine;
            repoContent = repoContent + "}";
            foreach (var r in ReactRepoDic)
            {
                try
                {
                    string tableName = r.Key;
                    string modelName = ti.ToTitleCase(tableName);
                    List<string> repoFunctionList = new List<string>();
                    string path = CreateDestinationPath("src,services," + r.Key + "Service.ts");
                    var tableData = FinalDataDic[tableName];
                    string templateFunction = "";
                    foreach (var f in r.Value)
                    {

                        repoFunctionList.Add(f.functionName);
                        string template = repoContent;
                        template = template.Replace("{functionName}", f.functionName);
                        template = template.Replace("{apiURL}", f.finalURL);
                        template = template.Replace("{methodName}", f.methodName);
                        string paramList = "";
                        string paramData = "";

                        if (f.functionType == "add")
                        {
                            paramList = "data";
                            paramData = ",data";
                        }
                        else if (f.functionType == "update")
                        {
                            template = template.Replace("${" + tableData.PrimaryKeys[0].FieldName + "}", "${data." + tableData.PrimaryKeys[0].FieldName + "}");
                            paramList = "data";
                            paramData = ",data";
                        }
                        else if (f.functionType == "search")
                        {
                            if (f.postParamList.Count() > 0)
                            {
                                foreach (var p in f.postParamList)
                                {
                                    paramList = paramList + "," + p.PropName;
                                    paramData = paramData + "," + p.PropName + ":" + p.PropName;
                                }
                            }
                            else if (f.getList.Count() > 0)
                            {
                                paramData = "";
                                foreach (var p in f.getList)
                                {
                                    paramList = paramList + "," + p;
                                }
                            }
                            paramList = paramList.Trim(',');
                            if (!string.IsNullOrEmpty(paramData))
                            {
                                paramData = ",{" + paramData.Trim(',') + "}";
                            }
                            paramList = "searchKey," + paramList;
                        }
                        else if (f.functionType == "getbyid")
                        {
                            var PrimaryKeyCommaString = tableData.PrimaryKeys[0].FieldName;
                            paramList = PrimaryKeyCommaString;
                            paramList = paramList.Trim(',');
                        }
                        else if (f.functionType == "delete")
                        {
                            var PrimaryKeyCommaString = tableData.PrimaryKeys[0].FieldName;
                            paramList = PrimaryKeyCommaString;
                            paramList = paramList.Trim(',');
                        }
                        else
                        {
                            if (f.postParamList.Count() > 0)
                            {
                                foreach (var p in f.postParamList)
                                {
                                    paramList = paramList + "," + p.PropName;
                                    paramData = paramData + "," + p.PropName + ":" + p.PropName;
                                }
                            }
                            else if (f.getList.Count() > 0)
                            {
                                paramData = "";
                                foreach (var p in f.getList)
                                {
                                    paramList = paramList + "," + p;
                                }
                            }
                            paramList = paramList.Trim(',');
                            if (!string.IsNullOrEmpty(paramData))
                            {
                                paramData = ",{" + paramData.Trim(',') + "}";
                            }
                        }
                        template = template.Replace("{paramList}", paramList);
                        template = template.Replace("{paramData}", paramData);
                        templateFunction = templateFunction + Environment.NewLine + template;
                    }
                    string contents = File.ReadAllText(CreateTemplatePath("service.txt"));
                    using (var txtFile = File.AppendText(path))
                    {
                        contents = contents.Replace("{serviceFunction}", templateFunction);
                        contents = contents.Replace("{tableName}", tableName);
                        contents = contents.Replace("{modelName}", modelName);
                        txtFile.WriteLine(contents);
                    }

                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                    ExList.Add(ex);
                }
            }
        }
        private void CreateSliceFile()
        {
            List<string> ActionList = new List<string>();
            List<string> ReducerList = new List<string>();
            foreach (var r in FinalDataDic)
            {
                try
                {
                    string tableName = r.Key;
                    string modelName = ti.ToTitleCase(tableName);
                    Directory.CreateDirectory(CreateDestinationPath("src,redux,slices," + tableName));
                    string path = CreateDestinationPath("src,redux,slices," + tableName + ",index.ts");
                    string tableInterface = "export interface I" + modelName + " {";
                    foreach (var c in r.Value.SelectQueryData.ColumnList)
                    {
                        string isNull = "?";
                        if (c.IsNull != null && c.IsNull.ToLower() == "no")
                        {
                            isNull = "";
                        }
                        tableInterface = tableInterface + Environment.NewLine + c.Field + isNull + ":" + Helper.GetDataTypeTypeScript(c.TypeName) + ",";
                    }
                    tableInterface = tableInterface.Trim(',');
                    tableInterface = tableInterface + Environment.NewLine + "}";
                    string contents = File.ReadAllText(CreateTemplatePath("slice.txt"));
                    using (var txtFile = File.AppendText(path))
                    {
                        contents = contents.Replace("{tableInterface}", tableInterface);
                        contents = contents.Replace("{tableName}", tableName);
                        contents = contents.Replace("{modelName}", modelName);
                        txtFile.WriteLine(contents);
                        string actionImport = "export { set{modelName}List, reset{modelName}ToInit, set{modelName}Message } from 'redux/slices/{tableName}';" + Environment.NewLine;
                        actionImport = actionImport.Replace("{tableName}", tableName);
                        actionImport = actionImport.Replace("{modelName}", modelName);
                        ActionList.Add(actionImport);
                        ReducerList.Add(tableName);
                    }
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                    ExList.Add(ex);
                }
            }
            string _actionList = "";
            string reducerImport = "";
            string reducerList = "";
            foreach (var a in ActionList)
            {
                _actionList = _actionList + a + Environment.NewLine;
            }

            string path2 = CreateDestinationPath("src,redux,actions.ts");
            string contents2 = File.ReadAllText(CreateTemplatePath("actions.txt"));
            using (var txtFile = File.AppendText(path2))
            {
                contents2 = contents2.Replace("{actionList}", _actionList);
                txtFile.WriteLine(contents2);
            }
            string path3 = CreateDestinationPath("src,redux,reducers.ts");
            foreach (var r in ReducerList)
            {
                string reducer = "import {tableName} from \"redux/slices/{tableName}\";" + Environment.NewLine;
                reducer = reducer.Replace("{tableName}", r);
                reducerImport = reducerImport + reducer;
                if (!string.IsNullOrEmpty(reducerList))
                {
                    reducerList = reducerList + "," + r;
                }
                else
                {
                    reducerList = r;
                }
            }
            string contents3 = File.ReadAllText(CreateTemplatePath("reducers.txt"));
            using (var txtFile = File.AppendText(path3))
            {
                contents3 = contents3.Replace("{reducerImport}", reducerImport);
                contents3 = contents3.Replace("{reducerList}", reducerList);
                txtFile.WriteLine(contents3);
            }
        }
        private void CreateAuthServiceFile()
        {
            string path = CreateDestinationPath("src,services,authService.ts");
            string contents = File.ReadAllText(CreateTemplatePath("auth.txt"));
            string tokenURL = "/login";
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{tokenURL}", tokenURL);
                txtFile.WriteLine(contents);
            }
        }
        private void CreateUIIndexFile()
        {
            foreach (var r in FinalDataDic)
            {
                try
                {
                    string tableName = r.Key;
                    string modelName = ti.ToTitleCase(tableName);
                    Directory.CreateDirectory(CreateDestinationPath("src,components," + tableName));
                    string path = CreateDestinationPath("src,components," + tableName + ",index.tsx");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    string contents = File.ReadAllText(CreateTemplatePath("component.txt"));
                    using (var txtFile = File.AppendText(path))
                    {
                        contents = contents.Replace("{tableName}", tableName);
                        contents = contents.Replace("{modelName}", modelName);
                        txtFile.WriteLine(contents);
                    }
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                    ExList.Add(ex);
                }
            }
        }
        private void CreateTableFile()
        {
            foreach (var r in FinalDataDic)
            {
                try
                {
                    string tableName = r.Key;
                    string modelName = ti.ToTitleCase(tableName);
                    string path = CreateDestinationPath("src,components," + tableName + ",table.tsx");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    string contents = File.ReadAllText(CreateTemplatePath("table.txt"));
                    string tableColumn = "";
                    string primaryKey = FinalDataDic[tableName].PrimaryKeyString;
                    var tableData = FinalDataDic[tableName];
                    foreach (var c in r.Value.SelectQueryData.ColumnList)
                    {
                        if (c.FKDetails != null)
                        {
                            tableColumn = tableColumn + "{name: '" + c.Field + "', selector: row => row." + c.Field + ", sortable: true}," + Environment.NewLine;
                            var fk = r.Value.SelectQueryData.FKColumnData.FirstOrDefault(k => k.LocalField == c.Field && k.TableName2 == c.FKDetails.REFERENCED_TABLE_NAME);
                            if (fk != null)
                            {
                                tableColumn = tableColumn + "{name: '" + fk.FieldName2 + "', selector: row => row." + c.FKDetails.REFERENCED_TABLE_NAME + "." + fk.FieldName2 + ", sortable: true}," + Environment.NewLine;
                            }
                        }
                        else
                        {
                            tableColumn = tableColumn + "{name: '" + c.Field + "', selector: row => row." + c.Field + ", sortable: true}," + Environment.NewLine;
                        }
                    }
                    string primaryKeyList = "";
                    //foreach (var p in tableData.PrimaryKeys)
                    //{
                    //    if (!string.IsNullOrEmpty(primaryKeyList))
                    //    {
                    //        primaryKeyList = primaryKeyList + ",rowData." + p.FieldName;
                    //    }
                    //    else
                    //    {
                    //        primaryKeyList = "rowData." + p.FieldName;
                    //    }
                    //}
                    primaryKeyList = "rowData." + tableData.PrimaryKeys[0].FieldName;
                    primaryKeyList = primaryKeyList.Trim(',');
                    using (var txtFile = File.AppendText(path))
                    {
                        contents = contents.Replace("{tableName}", tableName);
                        contents = contents.Replace("{modelName}", modelName);
                        contents = contents.Replace("{tableColumn}", tableColumn);
                        contents = contents.Replace("{primaryKey}", primaryKey);
                        contents = contents.Replace("{primaryKeyList}", primaryKeyList);

                        txtFile.WriteLine(contents);
                    }
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                    ExList.Add(ex);
                }
            }
        }
        private string CreateFormComponent(ColumnModel c)
        {
            string formGroup = "";
            string dataType = Helper.GetDataTypeTypeYup(c.TypeName);
            formGroup = formGroup + "<Form.Group>" + Environment.NewLine;
            formGroup = formGroup + "<label className=\"form -control-label\">" + c.Field + "</label>" + Environment.NewLine;
            formGroup = formGroup + "<Form.Control type=\"text\" name=\"" + c.Field + "\" className=\"form-control\" value={formik.values." + c.Field + "}" + Environment.NewLine;
            formGroup = formGroup + "onChange={formik.handleChange}" + Environment.NewLine;
            formGroup = formGroup + "onBlur ={formik.handleBlur}" + Environment.NewLine;
            formGroup = formGroup + "isInvalid ={!!formik.touched." + c.Field + " && !!formik.errors." + c.Field + "}" + Environment.NewLine;
            formGroup = formGroup + "isValid ={!!formik.touched." + c.Field + " && !formik.errors." + c.Field + "}" + Environment.NewLine;
            formGroup = formGroup + "></Form.Control>" + Environment.NewLine;
            formGroup = formGroup + "{" + Environment.NewLine;
            formGroup = formGroup + "    formik.errors." + c.Field + " && (" + Environment.NewLine;
            formGroup = formGroup + "    <Form.Control.Feedback type=\"invalid\">" + Environment.NewLine;
            formGroup = formGroup + "        {formik.errors." + c.Field + "}" + Environment.NewLine;
            formGroup = formGroup + "    </Form.Control.Feedback>" + Environment.NewLine;
            formGroup = formGroup + ")}" + Environment.NewLine;
            formGroup = formGroup + "</Form.Group>" + Environment.NewLine;

            return formGroup;
        }
        private string CreateFormFKComponent(FKColumnClass c)
        {
            string tName = c.TableName2;
            string mName = ti.ToTitleCase(tName);
            string formGroup = "";
            string dataType = Helper.GetDataTypeTypeInputText(c.DataType2);
            formGroup = formGroup + "<Form.Group>" + Environment.NewLine;
            formGroup = formGroup + "<label className=\"form -control-label\">" + c.LocalField + "</label>" + Environment.NewLine;
            formGroup = formGroup + "<Form.Control as=\"select\"  name=\"" + c.LocalField + "\" className=\"form-control\" value={formik.values." + c.LocalField + "}" + Environment.NewLine;
            formGroup = formGroup + "onChange={formik.handleChange}" + Environment.NewLine;
            formGroup = formGroup + "onBlur ={formik.handleBlur}" + Environment.NewLine;
            formGroup = formGroup + "isInvalid ={!!formik.touched." + c.LocalField + " && !!formik.errors." + c.LocalField + "}" + Environment.NewLine;
            formGroup = formGroup + "isValid ={!!formik.touched." + c.LocalField + " && !formik.errors." + c.LocalField + "}" + Environment.NewLine;
            formGroup = formGroup + ">" + Environment.NewLine;
            formGroup = formGroup + "<option value={0}>Select " + mName + " </option> " + Environment.NewLine;
            formGroup = formGroup + "{" + Environment.NewLine;
            formGroup = formGroup + tName + "Data.list.map((item, i) => {" + Environment.NewLine;
            formGroup = formGroup + "return <option value={item." + c.FieldName1 + "} key={`" + tName + "-${i}`}>{item." + c.FieldName2 + "}</option>" + Environment.NewLine;
            formGroup = formGroup + "})}" + Environment.NewLine;
            formGroup = formGroup + "</Form.Control>" + Environment.NewLine;
            formGroup = formGroup + "{" + Environment.NewLine;
            formGroup = formGroup + "    formik.errors." + c.LocalField + " && (" + Environment.NewLine;
            formGroup = formGroup + "    <Form.Control.Feedback type=\"invalid\">" + Environment.NewLine;
            formGroup = formGroup + "        {formik.errors." + c.LocalField + "}" + Environment.NewLine;
            formGroup = formGroup + "    </Form.Control.Feedback>" + Environment.NewLine;
            formGroup = formGroup + ")}" + Environment.NewLine;
            formGroup = formGroup + "</Form.Group>" + Environment.NewLine;

            return formGroup;
        }
        private void CreateFormFile()
        {
            foreach (var r in FinalDataDic)
            {
                try
                {
                    string tableName = r.Key;
                    string modelName = ti.ToTitleCase(tableName);
                    string path = CreateDestinationPath("src,components," + tableName + ",form.tsx");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    string ColumnListWithValue = "";
                    string primaryKey = FinalDataDic[tableName].PrimaryKeyString;
                    foreach (var c in r.Value.SelectQueryData.ColumnList)
                    {
                        var dataTypeValue = Helper.GetValueFromDataType(c.TypeName);
                        int intValue = 0;
                        if (!string.IsNullOrEmpty(ColumnListWithValue))
                            ColumnListWithValue = ColumnListWithValue + "," + c.Field + ":" + (int.TryParse(dataTypeValue, out intValue) ? 0 : "''");
                        else
                            ColumnListWithValue = c.Field + ":" + (int.TryParse(dataTypeValue, out intValue) ? 0 : "''");
                    }
                    ColumnListWithValue = ColumnListWithValue.Trim(',');
                    string yupValidationList = "";
                    string formGroupWithValidation = "";
                    foreach (var c in r.Value.SelectQueryData.ColumnList)
                    {
                        if (c.Key != "PRI" && c.FKDetails == null)
                        {
                            string dataType = Helper.GetDataTypeTypeYup(c.TypeName);
                            if (c.IsNull != null && c.IsNull.ToLower() == "no")
                            {
                                yupValidationList = yupValidationList + c.Field + ": yup." + dataType + "().required('" + c.Field + " is required')," + Environment.NewLine;
                            }
                            else
                            {
                                yupValidationList = yupValidationList + c.Field + ": yup." + dataType + "().nullable()," + Environment.NewLine;
                            }
                            formGroupWithValidation = formGroupWithValidation + CreateFormComponent(c);
                        }
                        else if (c.Key == "PRI" && c.Extra != "auto_increment" && c.DefaultValue != "CURRENT_TIMESTAMP" && c.FKDetails == null)
                        {
                            string dataType = Helper.GetDataTypeTypeYup(c.TypeName);
                            if (c.IsNull != null && c.IsNull.ToLower() == "no")
                            {
                                yupValidationList = yupValidationList + c.Field + ": yup." + dataType + "().required('" + c.Field + " is required')," + Environment.NewLine;
                            }
                            else
                            {
                                yupValidationList = yupValidationList + c.Field + ": yup." + dataType + "().nullable()," + Environment.NewLine;
                            }
                            formGroupWithValidation = formGroupWithValidation + CreateFormComponent(c);
                        }
                        else if (c.FKDetails != null)
                        {

                        }
                    }
                    string importFKRedux = "";
                    string importFKService = "";
                    string fkReduxInit = "";
                    string useEffectForFK = "";
                    foreach (var c in r.Value.SelectQueryData.FKColumnData)
                    {
                        string dType = Helper.GetDataTypeTypeYup(c.DataType2);
                        yupValidationList = yupValidationList + c.LocalField + ": yup." + dType + "().required('" + c.LocalField + " is required')," + Environment.NewLine;

                        formGroupWithValidation = formGroupWithValidation + CreateFormFKComponent(c);
                        string colName = c.FieldName2;
                        string tName = c.TableName2;
                        string mName = ti.ToTitleCase(tName);
                        useEffectForFK = useEffectForFK + "if (" + tName + "Data && " + tName + "Data.list && " + tName + "Data.list.length === 0) {" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "            dispatch(reset" + mName + "ToInit());" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "            get" + mName + "(Constant.defaultPageNumber, Constant.defaultDropdownPageSize, '').then((response) => {" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "                if (response && response.records) {" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "                  dispatch(set" + mName + "List({ pageNo: Constant.defaultPageNumber, pageSize: Constant.defaultDropdownPageSize, list: response.records, totalCount: response.total_count, searchKey: '' }));" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "                } else {" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "                  dispatch(set" + modelName + "Message(\"No Record Found For " + mName + "\"));" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "                }" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "              })" + Environment.NewLine;
                        useEffectForFK = useEffectForFK + "        }" + Environment.NewLine;

                        importFKRedux = importFKRedux + "import { reset" + mName + "ToInit, set" + mName + "List } from \"redux/actions\";" + Environment.NewLine;
                        importFKService = importFKService + "import { get" + mName + " } from \"services/" + tName + "Service\";" + Environment.NewLine;
                        fkReduxInit = fkReduxInit + "const " + tName + "Data = useSelector((state: RootState) => state." + tName + ");" + Environment.NewLine;
                    }
                    if (!string.IsNullOrEmpty(useEffectForFK))
                    {
                        useEffectForFK = "useEffect(() => {" + Environment.NewLine + useEffectForFK + "})";
                    }

                    string contents = File.ReadAllText(CreateTemplatePath("form.txt"));
                    using (var txtFile = File.AppendText(path))
                    {
                        contents = contents.Replace("{tableName}", tableName);
                        contents = contents.Replace("{modelName}", modelName);
                        contents = contents.Replace("{useEffectForFK}", useEffectForFK);
                        contents = contents.Replace("{ColumnListWithValue}", ColumnListWithValue);
                        contents = contents.Replace("{yupValidationList}", yupValidationList);
                        contents = contents.Replace("{importFKRedux}", importFKRedux);
                        contents = contents.Replace("{fkReduxInit}", fkReduxInit);

                        contents = contents.Replace("{importFKService}", importFKService);

                        contents = contents.Replace("{formGroupWithValidation}", formGroupWithValidation);
                        txtFile.WriteLine(contents);
                    }
                }
                catch (Exception ex)
                {
                    MessageEvent?.Invoke(new NKVMessage(ex.Message, false));
                    ExList.Add(ex);
                }
            }
        }
        private void CreateMenuFile()
        {
            string menuItem = "";
            foreach (var r in FinalDataDic)
            {
                string tableName = r.Key;
                string modelName = ti.ToTitleCase(tableName);
                menuItem = menuItem + "{ title: '" + modelName + "', path: '/" + tableName + "', icon: 'fas fa-fw fa-table',subMenu: []}," + Environment.NewLine;
            }
            string path = CreateDestinationPath("src,template,MenuItems.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string contents = File.ReadAllText(CreateTemplatePath("menu.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{menuItems}", menuItem);
                txtFile.WriteLine(contents);
            }
        }
        private void CreateRoutesFile()
        {
            string routePathList = "";
            string importComponents = "";
            foreach (var r in FinalDataDic)
            {
                string tableName = r.Key;
                string modelName = ti.ToTitleCase(tableName);
                routePathList = routePathList + "<Route path=\"/" + tableName + "\" element={<AuthenticatedRoute element={<" + modelName + " />} />}></Route>" + Environment.NewLine;
                importComponents = importComponents + modelName + ", ";
            }
            string path = CreateDestinationPath("src,pages,index.tsx");
            importComponents = importComponents.Trim().Trim(',');
            importComponents = "import { " + importComponents + "} from \"components\";";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string contents = File.ReadAllText(CreateTemplatePath("routes.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{routePathList}", routePathList);
                contents = contents.Replace("{importComponents}", importComponents);

                txtFile.WriteLine(contents);
            }
        }
        private void CreateImportComponentFile()
        {
            string importComponent = "";
            foreach (var r in FinalDataDic)
            {
                string tableName = r.Key;
                string modelName = ti.ToTitleCase(tableName);
                importComponent = importComponent + "export { " + modelName + " } from \"./" + tableName + "\";" + Environment.NewLine;
            }
            string path = CreateDestinationPath("src,components,index.ts");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string contents = File.ReadAllText(CreateTemplatePath("import.txt"));
            using (var txtFile = File.AppendText(path))
            {
                contents = contents.Replace("{importComponent}", importComponent);

                txtFile.WriteLine(contents);
            }
        }
        public List<Exception> CreateReactAPP(ReactJSInput<FinalQueryData> reactInput)
        {
            CreateDirectory();
            FinalDataDic = reactInput.FinalDataDic;
            CreateRepoDictionary(reactInput.PostmanJson);
            CreateServiceFile();
            MessageEvent?.Invoke(new NKVMessage("Service api file generated"));
            CreateSliceFile();
            MessageEvent?.Invoke(new NKVMessage("Redux component generated"));
            CreateAuthServiceFile();
            CreateUIIndexFile();
            CreateTableFile();
            MessageEvent?.Invoke(new NKVMessage("List component generated"));
            CreateFormFile();
            MessageEvent?.Invoke(new NKVMessage("Form component generated"));
            CreateMenuFile();
            MessageEvent?.Invoke(new NKVMessage("NavMenu generated"));
            CreateRoutesFile();
            MessageEvent?.Invoke(new NKVMessage("Routes generated"));
            CreateImportComponentFile();
            MessageEvent?.Invoke(new NKVMessage("----- React APP Generated -----"));
            CompletedEvent?.Invoke(new NKVMessage("Thanks for using GetAutomator.com! Please check the generated code at : " + DestinationFolder));
            return ExList;
        }
    }
}
