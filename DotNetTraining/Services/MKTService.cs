using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Application.Settings;
using Common.Application.Models;
using BPMaster.Domains.Dtos;
using BPMaster.Domains.Entities;
using Common.Application.CustomAttributes;
using Common.Application.Exceptions;
using Common.Services;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace BPMaster.Services
{
    [ScopedService]
    public class MKTService : BaseService
    {
        private readonly ILogger<MKTService> _logger;
        private readonly string _kepwareHost;
        private readonly int _kepwarePort;
        private readonly HttpClient _httpClient;
        private CancellationTokenSource _monitoringCts;
        private bool _isMonitoring;

        public MKTService(
            IServiceProvider services,
            ApplicationSetting setting,
            ILogger<MKTService> logger) : base(services)
        {
            _logger = logger;
            _kepwareHost = "127.0.0.1"; // Configure from settings
            _kepwarePort = 5000; // Configure from settings
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://{_kepwareHost}:{_kepwarePort}")
            };
        }

      

        public async Task WriteTagValue(string tagName, object value)
        {
            try
            {
                var writeRequest = new
                {
                    id = tagName,
                    v = value
                };

                var response = await _httpClient.PostAsync("/iotgateway/write",
                    new StringContent(JsonConvert.SerializeObject(writeRequest), Encoding.UTF8, "application/json"));
                
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error writing tag value: {ex.Message}");
               
            }
        }

        public async Task WriteMultipleTagValues(Dictionary<string, object> tags)
        {
            try
            {
                var writeRequests = new List<object>();
                foreach (var tag in tags)
                {
                    writeRequests.Add(new
                    {
                        id = tag.Key,
                        v = tag.Value
                    });
                }

                var response = await _httpClient.PostAsync("/iotgateway/write",
                    new StringContent(JsonConvert.SerializeObject(writeRequests), Encoding.UTF8, "application/json"));
                
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error writing multiple tag values: {ex.Message}");
               
            }
        }

        public async Task<Dictionary<string, object>> GetTagValues()
        {
            try
            {
                var tags = new[] { "tag_Bool", "tag_Integer", "tag_Real" };

                var tagList = new List<object>();
                foreach (var tag in tags)
                {
                    tagList.Add(new
                    {
                        id = $"Channel1.Device1.{tag}",
                        s = true, // Subscribe
                        r = true  // Read
                    });
                }

                var requestContent = new StringContent(
                    JsonConvert.SerializeObject(tagList),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/iotgateway/read", requestContent);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var tagResponses = JsonConvert.DeserializeObject<List<TagResponse>>(result);

                var tagValues = new Dictionary<string, object>();
                foreach (var tagResponse in tagResponses)
                {
                    var tagName = tagResponse.ID.Split('.').Last();
                    tagValues[tagName] = tagResponse.V;
                }

                return tagValues;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading tag values: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }

        private class TagResponse
        {
            public string ID { get; set; } // Tag ID
            public object V { get; set; }  // Value
            public string T { get; set; } // Timestamp
            public string Q { get; set; } // Quality
        }

        public Task StopTagMonitoring()
        {
            if (!_isMonitoring)
                return Task.CompletedTask;

            _monitoringCts?.Cancel();
            _isMonitoring = false;
            return Task.CompletedTask;
        }
    }
}
