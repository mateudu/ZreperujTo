using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZreperujTo.UWP.Models.BidModels;
using ZreperujTo.UWP.Models.CategoryModels;
using ZreperujTo.UWP.Models.FailModels;

namespace ZreperujTo.UWP.Helpers
{
    class ZreperujToHelper
    {
        private readonly string _apiUrl = @"https://zreperujto.azurewebsites.net/api/";
        private readonly HttpClient _client = new HttpClient();

        public async Task<List<CategoryReadModel>> GetCategories()
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

        public async Task<bool> AddCategories(CategoryWriteModel categoryWriteModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Categories");
                string output = JsonConvert.SerializeObject(categoryWriteModel);
                var content = new StringContent(output, Encoding.UTF8, "application/json");
                var result = await _client.PostAsync(url, content);
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddCategories(CategoryReadModel categoryReadModel, string name)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Categories/{categoryReadModel.Id}");
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("name", $"{name}") });
                var result = await _client.PostAsync(url, content);
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<FailMetaModel>> BrowseFails()
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

        public async Task<List<FailMetaModel>> BrowseFails(CategoryReadModel categoryReadModel)
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
        public async Task<List<FailMetaModel>> BrowseFails(CategoryReadModel categoryReadModel, SubcategoryReadModel subcategoryReadModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Browse/{categoryReadModel.Id}/{categoryReadModel.Id}");
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

        public async Task<FailReadModel> GetFailDetail(FailMetaModel failMetaModel) //ToDo something is not clear
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Details/{failMetaModel.FailId}");
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
        public async Task<bool> AddFail(FailWriteModel failWriteModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Create");
                string output = JsonConvert.SerializeObject(failWriteModel);
                var content = new StringContent(output, Encoding.UTF8, "application/json");
                var result = await _client.PostAsync(url, content);
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> MakeBid(FailReadModel failReadModel, BidWriteModel bidWriteModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Details/{failReadModel.FailId}/Bids/MakeBid");
                var output = JsonConvert.SerializeObject(bidWriteModel);
                var content = new StringContent(output, Encoding.UTF8, "application/json");
                var result = await _client.PostAsync(url, content);
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<BidReadModel>> GetBidsForFail(string id) //ToDo something is not clear
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
        public async Task<bool> AcceptBid(FailReadModel failReadModel, BidReadModel bidReadModel)
        {
            try
            {
                var url = new Uri($"{_apiUrl}/Fails/Details/{failReadModel.FailId}/Bids/{bidReadModel.Id}/Accept");
                var result = await _client.PostAsync(url,new StringContent(""));//ToDo for sure?
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //ToDo profile?
    }
}
