using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NtpNotDefteri.Models;

namespace NtpNotDefteri.Services
{
    public class CategoryDosyaServisi
    {
        private readonly string _dosyaYolu;

        public CategoryDosyaServisi(string dosyaYolu)
        {
            _dosyaYolu = dosyaYolu;
        }

        public List<Kategori> Yukle()
        {
            try
            {
                if (!File.Exists(_dosyaYolu))
                    return new List<Kategori>();
                var json = File.ReadAllText(_dosyaYolu);
                return JsonConvert.DeserializeObject<List<Kategori>>(json)
                       ?? new List<Kategori>();
            }
            catch
            {
                return new List<Kategori>();
            }
        }

        public void Kaydet(List<Kategori> kategoriler)
        {
            try
            {
                var json = JsonConvert.SerializeObject(kategoriler, Formatting.Indented);
                File.WriteAllText(_dosyaYolu, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kategori kaydederken hata: {ex.Message}");
            }
        }
    }
}
