using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace nkv.Automator.Utility
{
    public static class Helper
    {
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }
        public static string GetTableCharacter(List<string> characterUsed, string tableName, int size = 2)
        {
            var bannedKey = new List<string>() { "as", "on", "t" };
            var random = new Random();
            var key = tableName[0].ToString();
            while (key.Length < size)
            {
                key += tableName[random.Next(0, tableName.Length - 1)];
            }
            if (characterUsed.IndexOf(key) < 0 && bannedKey.IndexOf(key) < 0)
            {
                return key;
            }
            else
            {
                return GetTableCharacter(characterUsed, tableName, size + 1);
            }
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                // Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static string MongodbDataTypeToJIOValidation(string input)
        {
            switch (input.ToLower())
            {
                case "objectid":
                    return "string";
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "string";
                case "time":
                    return "string";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    return "number";
                case "bigint":
                case "int64":
                    return "number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "string";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "string";
                default:
                    return "string";
            }
        }
        public static string MongodbDataTypeToTypeScriptReact(string input)
        {
            switch (input.ToLower())
            {
                case "objectid":
                    return "string";
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "Date";
                case "time":
                    return "string";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    return "number";
                case "bigint":
                case "int64":
                    return "Number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "string";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "string";
                default:
                    return "string";
            }
        }
        public static string MongodbDataTypeToTypeScript(string input, bool isSchema = false)
        {
            switch (input.ToLower())
            {
                case "objectid":
                    if (isSchema)
                    {
                        return "Types.ObjectId";
                    }
                    return "ObjectId";
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "Boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "Date";
                case "time":
                    return "String";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    return "Number";
                case "bigint":
                case "int64":
                    return "Number";
                case "decimal":
                    return "Number";
                case "double":
                case "float":
                case "real":
                    return "Number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "String";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "String";
                default:
                    return "String";
            }
        }
        public static string GetDataTypeTypeInputTextMongoDB(string input)
        {

            switch (input.ToLower())
            {
                case "objectid":
                    return "text";
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "date";
                case "time":
                    return "text";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    return "number";
                case "bigint":
                case "int64":
                    return "number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "Number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "text";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "text";
                default:
                    return "text";
            }
        }
        public static string GetDataTypeTypeInputText(string input)
        {
            string subDataType = "";
            string dataType = input;
            if (input.Contains("unsigned"))
            {
                subDataType = "unsigned";
            }
            if (input.Contains("("))
            {
                dataType = input.Split('(', ')')[0];
            }
            switch (dataType.ToLower())
            {
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "date";
                case "time":
                    return "text";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "number";
                case "bigint":
                case "int64":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "text";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "text";
                default:
                    return "text";
            }
        }
        public static string GetValueFromDataType(string input)
        {
            string subDataType = "";
            string dataType = input;
            if (input.Contains("unsigned"))
            {
                subDataType = "unsigned";
            }
            if (input.Contains("("))
            {
                dataType = input.Split('(', ')')[0];
            }
            switch (dataType.ToLower())
            {
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "true";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "";
                case "time":
                    return "";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "0";
                case "bigint":
                case "int64":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "0";
                case "decimal":
                    return "0";
                case "double":
                case "float":
                case "real":
                    return "0";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "";
                default:
                    return "";
            }
        }
        public static string GetDataTypeTypeYup(string input)
        {
            string subDataType = "";
            string dataType = input;
            if (input.Contains("unsigned"))
            {
                subDataType = "unsigned";
            }
            if (input.Contains("("))
            {
                dataType = input.Split('(', ')')[0];
            }
            switch (dataType.ToLower())
            {
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "date";
                case "time":
                    return "string";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "number";
                case "bigint":
                case "int64":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "string";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "string";
                default:
                    return "string";
            }
        }
        public static string GetMongodbDefaultValue(string columnName, string input)
        {
            switch (input.ToLower())
            {
                case "objectid":
                    return columnName + ":''";
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return columnName + ":true";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return columnName + ":''";
                case "time":
                    return columnName + ":''";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    return columnName + ":0";
                case "bigint":
                case "int64":
                    return columnName + ":0";
                case "decimal":
                    return columnName + ":0";
                case "double":
                case "float":
                case "real":
                    return columnName + ":0";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return columnName + ":''";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return columnName + ":''";
                default:
                    return columnName + ":''";
            }
        }
        public static string GetDataTypeTypeYupMongodb(string input)
        {
            switch (input.ToLower())
            {
                case "objectid":
                    return "string";
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "date";
                case "time":
                    return "string";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    return "number";
                case "bigint":
                case "int64":
                    return "number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "string";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "string";
                default:
                    return "string";
            }
        }
        public static string GetDataTypeTypeForUI(string input)
        {
            string subDataType = "";
            string dataType = input;
            if (input.Contains("unsigned"))
            {
                subDataType = "unsigned";
            }
            if (input.Contains("("))
            {
                dataType = input.Split('(', ')')[0];
            }
            switch (dataType.ToLower())
            {
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "datetime";
                case "time":
                    return "string";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "integer";
                    }
                    return "number";
                case "bigint":
                case "int64":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "integer";
                    }
                    return "number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "string";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "string";
                default:
                    return "string";
            }
        }
        public static string GetDataTypeTypeScript(string input)
        {
            string subDataType = "";
            string dataType = input;
            if (input.Contains("unsigned"))
            {
                subDataType = "unsigned";
            }
            if (input.Contains("("))
            {
                dataType = input.Split('(', ')')[0];
            }
            switch (dataType.ToLower())
            {
                case "binary":
                case "bit":
                case "boolean":
                case "tinyint":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "Date";
                case "time":
                    return "string";
                case "int":
                case "int32":
                case "int16":
                case "mediumint":
                case "smallint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "number";
                case "bigint":
                case "int64":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "number";
                    }
                    return "number";
                case "decimal":
                    return "number";
                case "double":
                case "float":
                case "real":
                    return "number";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "string";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "string";
                default:
                    return "string";
            }
        }
        public static string GetDataTypePHP(string input)
        {
            string subDataType = "";
            string dataType = input;
            if (input.Contains("unsigned"))
            {
                subDataType = "unsigned";
            }
            if (input.Contains("("))
            {
                dataType = input.Split('(', ')')[0];
            }
            switch (dataType.ToLower())
            {
                case "binary":
                case "bit":
                case "boolean":
                    return "boolean";
                case "datetime2":
                case "datetime":
                case "date":
                case "timestamp":
                    return "datetime";
                case "time":
                    return "string";
                case "int":
                case "int32":
                case "int16":
                case "tinyint":
                case "mediumint":
                case "smallint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "integer";
                    }
                    return "integer";
                case "bigint":
                case "int64":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "integer";
                    }
                    return "integer";
                case "decimal":
                    return "float";
                case "double":
                case "float":
                case "real":
                    return "float";
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    return "string";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "string";
                default:
                    return "string";
            }
        }
        public static string GetDataTypeCSharpSQL(string input)
        {
            string subDataType = "";
            string dataType = input;
            if (input.Contains("unsigned"))
            {
                subDataType = "unsigned";
            }
            if (input.Contains("("))
            {
                dataType = input.Split('(', ')')[0];
            }
            switch (dataType.ToLower())
            {
                case "binary":
                case "bit":
                case "boolean":
                    return "Boolean";
                case "datetime":
                case "date":
                    return "Date";
                case "timestamp":
                    return "DateTime";
                case "time":
                    return "TimeSpan";
                case "tinyint":
                    return "Int16";
                case "int":
                case "mediumint":
                case "smallint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "UInt32";
                    }
                    return "Int32";
                case "bigint":
                    switch (subDataType)
                    {
                        case "unsigned":
                            return "UInt64";
                    }
                    return "Int64";
                case "double":
                case "decimal":
                case "float":
                case "real":
                    return "Double";
                case "varchar":
                case "char":
                    return "String";
                case "text":
                case "tinytext":
                case "tinyblob":
                case "longtext":
                case "mediumblob":
                case "longblob":
                case "blob":
                case "mediumtext":
                    return "String";
                default:
                    return "String";
            }
        }

        public static string ToPascalCase(string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }
    }
}
