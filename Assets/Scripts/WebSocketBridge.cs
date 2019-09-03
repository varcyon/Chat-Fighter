// using System;
// using System.Net;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Threading.Tasks;
// using System.Windows;
// using UnityEngine;
// using UnityEngine.Events;

// public class WebSocketBridge : MonoBehaviour
// {

//     static HttpClient http = new HttpClient();

//     static async Task<Uri> AddPlayer(string _uri, string _data){
//         HttpResponseMessage response = await http.PostAsJsonAsync(_uri, _data);
//         response.EnsureSuccessStatusCode();
//         return response.Headers.Location;
//     }

//     static async Task<DataFormat> GetPlayer(string _uri){
//         DataFormat _data = null;
//         HttpResponseMessage response = await http.GetAsync(_uri);
//         if(response.IsSuccessStatusCode){
//             _data = await response.Content.ReadASAsync<DataFormat>();
//         }
//         return _data;
//     }

//     static async Task<DataFormat> UpdatePlayer(string _uri, DataFormat _data){
//         HttpResponseMessage response = await LineTextureMode.PutAsJsonAsync(_uri, _data);
//         response.EnsureSuccessStatusCode();

//         _data = await response.Content.ReadAsAsync<DataFormat>();
//         return _data;
//     }

//     static asyn Task<DataFormat> DeletePlayer(){}