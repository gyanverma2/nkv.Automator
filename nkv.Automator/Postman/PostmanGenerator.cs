using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.Automator.Postman
{
    internal static class PostmanGenerator
    {
        public static string ItemTemplate = "{\"name\": \"{NameString}\", \"request\":{ \"auth\":{ \"type\": \"bearer\", \"bearer\": [ { \"key\": \"token\", \"value\": \"{{token}}\", \"type\": \"string\"}]},\"" +
            "method\": \"{MethodString}\", \"header\": [ { \"key\": \"Content-Type\", \"name\": \"Content-Type\", \"value\": \"application/json\", \"type\": \"text\" } ], \"body\": { \"mode\": \"raw\", \"raw\": \"{BodyJson}\", \"options\": { \"raw\": { \"language\": \"json\" } } }, \"url\": { \"raw\": \"{{DEVServerURL}}{URLString}\", \"host\": [ \"{{DEVServerURL}}\" ], \"path\": {PathArray} } }, \"response\": [] }";

        public static string GeneratePostmanJson(List<PostmanModel> postmanModels, out Exception e)
        {
            e = null;
            try
            {
                var item = "";
                foreach (var p in postmanModels)
                {
                    var singleItem = ItemTemplate;
                    singleItem = singleItem.Replace("{NameString}", p.Name);
                    singleItem = singleItem.Replace("{MethodString}", p.Method);
                    singleItem = singleItem.Replace("{URLString}", p.Url);

                    singleItem = singleItem.Replace("{PathArray}", JsonConvert.SerializeObject(p.Path));

                    if (p.Method.ToLower() == "post" && p.Name.ToLower().Contains("search"))
                    {
                        string bodyString = "[";
                        var bodyList = p.Body != null ? p.Body.Distinct().ToList() : new List<PRequestBody>();
                        foreach (var f in bodyList)
                        {
                            bodyString = bodyString + "{\\\"columnName\\\":\\\"\\\"" + ",";
                            bodyString = bodyString + "\\\"columnLogic\\\":\\\"LIKE\\\"" + ",";
                            bodyString = bodyString + "\\\"columnValue\\\":\\\"\\\"" + "},";
                        }
                        bodyString = bodyString.Trim(',');
                        bodyString = bodyString + "]";
                        singleItem = singleItem.Replace("{BodyJson}", bodyString);
                    }
                    else if (p.Body != null && p.Method.ToLower() != "get")
                    {
                        string bodyString = "{";
                        var bodyList = p.Body != null ? p.Body.Distinct().ToList() : new List<PRequestBody>();
                        foreach (var f in bodyList)
                        {
                            bodyString = bodyString + "\\\"" + f.PropName + "\\\":\\\"\\\"" + ",";
                        }
                        bodyString = bodyString.Trim(',');
                        bodyString = bodyString + "}";
                        singleItem = singleItem.Replace("{BodyJson}", bodyString);
                    }
                    else
                    {
                        singleItem = singleItem.Replace("{BodyJson}", "");
                    }
                    item = item + singleItem + Environment.NewLine + ",";
                }
                item = item.Trim(',');
                return item;
            }
            catch (Exception ex)
            {
                e = ex;
            }
            return "";
        }


    }
}
