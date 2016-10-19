using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZreperujTo.UWP.Models.BidModels;
using ZreperujTo.UWP.Models.CategoryModels;
using ZreperujTo.UWP.Models.FailModels;
using ZreperujTo.UWP.Models.UserInfoModels;

namespace ZreperujTo.UWP.Helpers
{
    class ZreperujToHelper
    {
        private readonly string _apiUrl = @"https://zreperujto.azurewebsites.net/api/";
        private readonly HttpClient _client = new HttpClient();

        public static string Token { get; set; }
        public ZreperujToHelper(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        public ZreperujToHelper()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }
        private HttpContent SerializeObject(object obj)
        {
            var output = JsonConvert.SerializeObject(obj);
            return new StringContent(output, Encoding.UTF8, "application/json");
        }
        public async Task<List<CategoryReadModel>> GetCategoriesAsync()
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Categories");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CategoryReadModel>>(response);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> AddCategoriesAsync(CategoryWriteModel categoryWriteModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Categories");          
                var result = await _client.PostAsync(url, SerializeObject(categoryWriteModel));
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> AddSubCategoriesAsync(CategoryReadModel categoryReadModel, SubcategoryWriteModel subcategoryWriteModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Categories/{categoryReadModel.Id}");
                var result = await _client.PostAsync(url, SerializeObject(subcategoryWriteModel));
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<FailMetaModel>> BrowseFailsAsync()
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Browse");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<FailMetaModel>>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<FailMetaModel>> BrowseFailsAsync(CategoryReadModel categoryReadModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Browse/{categoryReadModel.Id}");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<FailMetaModel>>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<FailMetaModel>> BrowseFailsAsync(CategoryReadModel categoryReadModel, SubcategoryReadModel subcategoryReadModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Browse/{categoryReadModel.Id}/{subcategoryReadModel.Id}");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<FailMetaModel>>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<FailReadModel> GetFailDetailAsync(FailMetaModel failMetaModel) //ToDo something is not clear
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Details/{failMetaModel.Id}");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<FailReadModel>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> AddFailAsync(FailWriteModel failWriteModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Create");
                var result = await _client.PostAsync(url, SerializeObject(failWriteModel));
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> MakeBidAsync(FailReadModel failReadModel, BidWriteModel bidWriteModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Details/{failReadModel.Id}/Bids/MakeBid");
                var result = await _client.PostAsync(url, SerializeObject(bidWriteModel));
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<BidReadModel>> GetBidsForFailAsync(string id) //ToDo something is not clear
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Details/{id}/Bids");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<BidReadModel>>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> AcceptBidAsync(FailReadModel failReadModel, BidReadModel bidReadModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Details/{failReadModel.Id}/Bids/{bidReadModel.Id}/Accept");
                var result = await _client.PostAsync(url,new StringContent(""));
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<UserInfoReadModel> GetProfileInfoAsync()
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Profile/Info");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<UserInfoReadModel>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<BidReadModel>> GetProfileBidsAsync()
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Profile/Info/Bids");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<BidReadModel>>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<FailMetaModel>> GetProfileFailsAsync()
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Profile/Info/Fails");
                var result = await _client.GetAsync(url);
                var response = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<List<FailMetaModel>>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
