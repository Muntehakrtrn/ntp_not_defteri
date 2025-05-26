using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NtpNotDefteri.Models;

namespace NtpNotDefteri.Services
{
    public class JsonDosyaServisi
    {
        private readonly string _dosyaYolu;

        public JsonDosyaServisi(string dosyaYolu)
        {
            _dosyaYolu = dosyaYolu;
        }

        public void Kaydet(List<Not> notlar)
        {
            try
            {
                var json = JsonConvert.SerializeObject(notlar, Formatting.Indented);
                File.WriteAllText(_dosyaYolu, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosyaya kaydederken hata: {ex.Message}");
            }
        }

        public List<Not> Yukle()
        {
            try
            {
                if (!File.Exists(_dosyaYolu))
                    return new List<Not>();

                var json = File.ReadAllText(_dosyaYolu);
                return JsonConvert.DeserializeObject<List<Not>>(json)
                       ?? new List<Not>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosyadan yüklerken hata: {ex.Message}");
                return new List<Not>();
            }
        }
    }
}
