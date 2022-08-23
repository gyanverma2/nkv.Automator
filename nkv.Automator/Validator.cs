using Newtonsoft.Json;
using nkv.Automator.Models;
using ServiceStack;
using System;
using System.Collections.Generic;

namespace nkv.Automator
{
    internal class Validator
    {
        private const string baseURL = "http://localhost:82/getautomator";
        
        public List<ProductModel> GetAllProduct()
        {
            try
            {
                var client = new JsonServiceClient(baseURL);
                var response = client.Get<string>("/api/products/read.php?APIKEY=3f7569f151994c19894a139257b049e6");
                if (response != null)
                {
                    var apiResponse = JsonConvert.DeserializeObject<APIResponse<APIResponseRecord<List<ProductModel>>>>(response);
                    if (apiResponse != null && apiResponse.code == 1)
                    {
                        return apiResponse.document.records;
                    }
                }
                return new List<ProductModel>() { new ProductModel() { ProductID = 0, ProductNumber = "0", ProductTitle = "No Product Found" } };
            }catch(Exception ex)
            {
                return new List<ProductModel>();
            }
        }
        public List<LicenceProductModel> GetAllLicence(LoginModel login)
        {
            try
            {
                var client = new JsonServiceClient(baseURL);
                client.AddHeader("Content-Type", "application/json");
                login.APIKEY = "3f7569f151994c19894a139257b049e6";
                string json = JsonConvert.SerializeObject(login);
                var response = client.Post<string>("/api/licence_check/getallbyuser.php", json);
                if (response != null)
                {
                    var apiResponse = JsonConvert.DeserializeObject<APIResponse<APIResponseRecord<List<LicenceProductModel>>>>(response);
                    if (apiResponse != null && apiResponse.code == 1)
                    {
                        return apiResponse.document.records;
                    }
                }
                return new List<LicenceProductModel>();
            }
            catch(Exception ex)
            {
                return new List<LicenceProductModel>();
            }
            
        }
        public bool Register(RegisterModel register)
        {
            try
            {
                var response = RegisterAPI(register);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private APIResponse<string> RegisterAPI(RegisterModel register)
        {
            var client = new JsonServiceClient(baseURL);
            var response = client.Post<string>("/api/licence_check/register.php", register);
            if (response != null)
            {
                var apiResponse = JsonConvert.DeserializeObject<APIResponse<string>>(response);
                if (apiResponse != null)
                {
                    return apiResponse;
                }
            }
            return null;
        }
        public bool ClickCounter(string publicID)
        {
            try
            {
                var client = new JsonServiceClient(baseURL);
                var param = new { LicencePublicID = publicID };

                var response = client.Post<string>("/api/licence_check/validate_click.php", param);
                if (response != null)
                {
                    var apiResponse = JsonConvert.DeserializeObject<APIResponse<string>>(response);
                    if (apiResponse != null && apiResponse.code == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
