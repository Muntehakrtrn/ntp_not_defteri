using System;
using System.Collections.Generic;
using System.IO;
using NtpNotDefteri.Models;
using NtpNotDefteri.Services;

namespace NtpNotDefteri
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1) Data klasörünü ve JSON dosyasını hazırla
            Directory.CreateDirectory("Data");
            var dosyaYolu = Path.Combine("Data", "notlar.json");

            // 2) Servisi başlat ve varsa notları yükle
            var servis = new JsonDosyaServisi(dosyaYolu);
            var notlar = servis.Yukle();

            // 3) Notlardaki kategorileri tekilleştirerek listeye ekle
            var kategoriler = new List<Kategori>();
            foreach (var n in notlar)
            {
                if (n.Kategori != null && !kategoriler.Exists(k => k.Id == n.Kategori.Id))
                    kategoriler.Add(n.Kategori);
            }

            bool cikis = false;
            while (!cikis)
            {
                Console.WriteLine("\n*** NOT DEFTERİ MENÜ ***");
                Console.WriteLine("1. Yeni Not Oluştur");
                Console.WriteLine("2. Yeni Kategori Oluştur");
                Console.WriteLine("3. Notları Listele");
                Console.WriteLine("4. Not Ara");
                Console.WriteLine("5. Çıkış");
                Console.Write("Seçiminiz: ");

                var secim = Console.ReadLine();
                switch (secim)
                {
                    case "1":
                        // Yeni not oluşturma
                        Console.Write("Başlık: ");
                        var baslik = Console.ReadLine();

                        Console.Write("İçerik: ");
                        var icerik = Console.ReadLine();

                        // Kategori seçimi
                        Kategori? secilenKategori = null;
                        if (kategoriler.Count > 0)
                        {
                            Console.WriteLine("Kategori seç (ID gir ya da boş bırak):");
                            foreach (var k in kategoriler)
                                Console.WriteLine($"  [{k.Id}] {k.Isim}");
                            Console.Write("Seçiminiz: ");
                            if (int.TryParse(Console.ReadLine(), out var secimId))
                                secilenKategori = kategoriler.Find(k => k.Id == secimId);
                        }

                        var yeniNot = new Not
                        {
                            Id = (notlar.Count > 0 ? notlar[^1].Id : 0) + 1,
                            Baslik = baslik,
                            Icerik = icerik,
                            OlusturmaTarihi = DateTime.Now,
                            Kategori = secilenKategori
                        };
                        notlar.Add(yeniNot);
                        Console.WriteLine($"[{yeniNot.Id}] \"{yeniNot.Baslik}\" başlıklı not eklendi.");
                        break;

                    case "2":
                        // Yeni kategori oluşturma
                        Console.Write("Yeni kategori adı: ");
                        var kategoriAdi = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(kategoriAdi))
                        {
                            var yeniKategori = new Kategori
                            {
                                Id = (kategoriler.Count > 0 ? kategoriler[^1].Id : 0) + 1,
                                Isim = kategoriAdi
                            };
                            kategoriler.Add(yeniKategori);
                            Console.WriteLine($"[{yeniKategori.Id}] {yeniKategori.Isim} kategorisi oluşturuldu.");
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz kategori adı.");
                        }
                        break;

                    case "3":
                        // Notları listele
                        Console.WriteLine("\n-- Tüm Notlar --");
                        foreach (var n in notlar)
                        {
                            Console.WriteLine($"[{n.Id}] {n.Baslik} ({n.Kategori?.Isim ?? "Kategori yok"}) - {n.OlusturmaTarihi}");
                        }
                        break;

                    case "4":
                        // Not ara
                        Console.Write("Aranacak kelime: ");
                        var anahtar = Console.ReadLine();
                        var bulunan = notlar.FindAll(n => n.EslesiyorMu(anahtar ?? ""));
                        Console.WriteLine($"\n-- '{anahtar}' içeren notlar ({bulunan.Count}) --");
                        foreach (var n in bulunan)
                        {
                            Console.WriteLine($"[{n.Id}] {n.Baslik} ({n.Kategori?.Isim ?? "Kategori yok"})");
                        }
                        break;

                    case "5":
                        cikis = true;
                        break;

                    default:
                        Console.WriteLine("Geçersiz seçim.");
                        break;
                }
            }

            // 4) Çıkarken notları kaydet
            servis.Kaydet(notlar);
            Console.WriteLine("Program sonlandırıldı.");
        }
    }
}
