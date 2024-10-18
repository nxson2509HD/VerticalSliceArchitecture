using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VerticalSliceArchitecture.Infrastructure.Helper
{
    public static class JsonFileHelper
    {
        // Phương thức đọc từ file JSON và trả về danh sách kiểu T
        public static async Task<List<T>> ReadFromJsonFileAsync<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return await JsonSerializer.DeserializeAsync<List<T>>(stream) ?? new List<T>();
            }
        }

        // Phương thức ghi vào file JSON từ danh sách kiểu T
        public static async Task WriteToJsonFileAsync<T>(string filePath, List<T> data)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(stream, data, options);
            }
        }

        // Phương thức thêm item vào file JSON
        public static async Task AddItemToJsonFileAsync<T>(string filePath, T newItem)
        {
            var items = await ReadFromJsonFileAsync<T>(filePath);

            items.Add(newItem); 

            await WriteToJsonFileAsync(filePath, items);
        }
    }
}
