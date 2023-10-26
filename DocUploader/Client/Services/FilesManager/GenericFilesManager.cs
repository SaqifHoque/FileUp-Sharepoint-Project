using DocUploader.Shared.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace DocUploader.Client.Services.FilesManager
{
    public class GenericFilesManager : IGenericFilesManager
    {
        HttpClient _http;
        public GenericFilesManager (HttpClient http)
        {
            _http = http;
        } //FilesManager


        public async Task<bool> UploadFileChunk(FileChunkDto fileChunkDto)
        {
            try
            {
                var result = await _http.PostAsJsonAsync("api/Generics/UploadFileChunk", fileChunkDto);
                result.EnsureSuccessStatusCode();
                string responseBody = await result.Content.ReadAsStringAsync();
                return Convert.ToBoolean(responseBody);
            }
            catch (Exception)
            {
                return false;
            }
        } //UploadFileChunk


        public async Task<List<string>> GetFileNames()
        {
            try
            {
                var response = await _http.GetAsync("api/Generics/GetFiles");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(responseBody)!;
            }
            catch (Exception)
            {
                return null!;
            }
        } //GetFileNames

    }
}
