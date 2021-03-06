﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Xunit;
using System;

namespace QueueServiceAPI.Tests
{
    public class ClientsTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public ClientsTests()
        {
            _server = new TestServer(new WebHostBuilder()
               .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Authentication()
        {
            //array
            var existuser = "евсей";
            var noexistuser = "евсей" + DateTime.Now.Millisecond * DateTime.Now.Millisecond;

            //act
            var response1 = await _client.PostAsync(
               requestUri: "/api/clients/auth",
               content: new StringContent(
                   content: "{\"fio\":\"" + existuser + "\"}",
                   encoding: Encoding.UTF8,
                   mediaType: "application/json"
               ));
            var responseContent1 = await response1.Content.ReadAsStringAsync();

            var response2 = await _client.PostAsync(
               requestUri: "/api/clients/auth",
               content: new StringContent(
                   content: "{\"fio\":\"" + noexistuser + "\"}",
                   encoding: Encoding.UTF8,
                   mediaType: "application/json"
               ));
            var responseContent2 = await response2.Content.ReadAsStringAsync();

            //assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            Assert.Contains($"{{\"id\":1,\"fio\":\"{existuser}\"", responseContent1);
            Assert.Contains($"\"fio\":\"{noexistuser}\"", responseContent2);
        }

        [Fact]
        public async Task GetUsers()
        {
            //array
            var name = "евсей";
            var id = 1;

            //act
            //all
            var response1 = await _client.GetAsync("/api/clients");
            var responseContent1 = await response1.Content.ReadAsStringAsync();
            //by name
            var response2 = await _client.GetAsync("/api/clients/?fio=" + name);
            var responseContent2 = await response2.Content.ReadAsStringAsync();
            //by id
            var response3 = await _client.GetAsync("/api/clients/" + id);
            var responseContent3 = await response3.Content.ReadAsStringAsync();


            //assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            response3.EnsureSuccessStatusCode();
            Assert.Contains($"{{\"id\":{id},\"fio\":\"{name}\"}}", responseContent1);
            Assert.Contains($"{{\"id\":{id},\"fio\":\"{name}\"}}", responseContent2);
            Assert.Contains($"{{\"id\":{id},\"fio\":\"{name}\"}}", responseContent3);
        }

        [Fact]
        public async Task GetUsersFail()
        {
            //array
            var name = "евсей" + DateTime.Now.Millisecond * DateTime.Now.Millisecond * DateTime.Now.Millisecond;
            var id = -1;

            //act
            //by name
            var response2 = await _client.GetAsync("/api/clients/?fio=" + name);
            //by id
            var response3 = await _client.GetAsync("/api/clients/" + id);


            //assert
            Assert.Equal("NotFound", response2.StatusCode.ToString());
            Assert.Equal("NotFound", response3.StatusCode.ToString());
        }
    }
}
