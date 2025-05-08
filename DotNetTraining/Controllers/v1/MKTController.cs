using System.Threading.Tasks;
using Application.Settings;
using Common.Application.Models;
using BPMaster.Domains.Dtos;
using BPMaster.Domains.Entities;
using BPMaster.Services;
using Common.Application.Exceptions;
using Common.Application.Models;
using Common.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Text.Json;
using Npgsql;

namespace BPMaster.Controllers.v1
{
    //public class MKTController : BaseV1Controller<MKTService, ApplicationSetting>
    //{
    //    private readonly MKTService _mktService;

    //    public MKTController(
    //        IServiceProvider services,
    //        IHttpContextAccessor httpContextAccessor,
    //        ApplicationSetting settings,
    //        MKTService mktService)
    //        : base(services, httpContextAccessor)
    //    {
    //        _mktService = mktService;
    //    }

    //    [HttpGet("getAllStatus")]
    //    public async Task<IActionResult> GetAllBP()
    //    {
    //        try 
    //        {
    //            //using HttpClient client = new HttpClient();

    //            //string url = "http://127.0.0.1:5000/iotgateway/read";
    //            var payload = new
    //            {
    //                tags = new[]
    //        {
    //            new { id = "Channel1.Device1.Tag1" } // Ensure this matches your Kepware tag structure
    //        }
    //            };

    //            string jsonPayload = JsonSerializer.Serialize(payload);
    //            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    //            //HttpResponseMessage response = await client.PostAsync(url, content);

    //            //string result = await response.Content.ReadAsStringAsync();
    //            string result = "Test";
    //            Console.WriteLine($"Response: {result}");
    //            return Success("ok");
    //        }
    //        catch (Exception ex)
    //        {
    //            return Error(ex.Message);
    //        }
    //    }

    //}
}
