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
            // 1) Data klasörünü ve JSON dosyalarını hazırla
            Directory.CreateDirectory("Data");
            var notDosya = Path.Combine("Data", "notlar.json");
            var katDosya = Path.Combine("Data", "kategoriler.json");

            // 2) Servisleri başlat ve verileri yükle
            var notServis = new JsonDosyaServisi(notDosya);
            var kategoriServis = new CategoryDosyaServisi(katDosya);

            var notlar = notServis.Yukle();
            var kategoriler = kategoriServis.Yukle();

            bool cikis = false;
            while (!cikis)
            {
                Console.WriteLine("\n*** NOT DEFTERİ MENÜ ***");
                Console.WriteLine("1. Yeni Not Oluştur");
                Console.WriteLine("2. Yeni Kategori Oluştur");
                Console.WriteLine("3. Kategorileri Listele");
                Console.WriteLine("4. Notları Listele");
                Console.WriteLine("5. Not Ara");
                Console.WriteLine("6. Çıkış");
                Console.Write("Seçiminiz: ");

                var secim = Console.ReadLine();
                switch (secim)
                {
                    case "1":
                        // --- Yeni Not ---
                        Console.Write("Başlık: ");
                        var baslik = Console.ReadLine() ?? "";

                        Console.Write("İçerik: ");
                        var icerik = Console.ReadLine() ?? "";

                        Kategori? secilenKategori = null;
                        if (kategoriler.Count > 0)
                        {
                            Console.WriteLine("\nMevcut Kategoriler:");
                            foreach (var k in kategoriler)
                                Console.WriteLine($"  [{k.Id}] {k.Isim}");
                            Console.Write("Kategori seç (ID girin, yoksa boş bırakın): ");
                            var katGirdi = Console.ReadLine();
                            if (int.TryParse(katGirdi, out var katId))
                            {
                                secilenKategori = kategoriler.Find(k => k.Id == katId);
                                if (secilenKategori == null)
                                    Console.WriteLine("Uyarı: Böyle bir kategori bulunamadı.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Henüz kategori yok; not kategori olmadan kaydedilecek.");
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

                        Console.WriteLine($"\n[{yeniNot.Id}] \"{yeniNot.Baslik}\" başlıklı not eklendi." +
                                          (secilenKategori != null
                                              ? $" (Kategori: {secilenKategori.Isim})"
                                              : " (Kategori yok)"));
                        break;

                    case "2":
                        // --- Yeni Kategori ---
                        Console.Write("Yeni kategori adı: ");
                        var kategoriAdi = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(kategoriAdi))
                        {
                            var yeniKategori = new Kategori
                            {
                                Id = (kategoriler.Count > 0 ? kategoriler[^1].Id : 0) + 1,
                                Isim = kategoriAdi
                            };
                            kategoriler.Add(yeniKategori);
                            Console.WriteLine($"\n[{yeniKategori.Id}] {yeniKategori.Isim} kategorisi oluşturuldu.");
                        }
                        else
                        {
                            Console.WriteLine("Geçersiz kategori adı.");
                        }
                        break;

                    case "3":
                        // --- Kategorileri Listele ---
                        Console.WriteLine("\n-- Mevcut Kategoriler --");
                        if (kategoriler.Count == 0)
                            Console.WriteLine("Henüz kategori yok.");
                        else
                            kategoriler.ForEach(k =>
                                Console.WriteLine($"[{k.Id}] {k.Isim}")
                            );
                        break;

                    case "4":
                        // --- Notları Listele ---
                        Console.WriteLine("\n-- Tüm Notlar --");
                        if (notlar.Count == 0)
                        {
                            Console.WriteLine("Hiç not yok.");
                        }
                        else
                        {
                            notlar.ForEach(n =>
                                Console.WriteLine($"[{n.Id}] {n.Baslik} ({n.Kategori?.Isim ?? "Kategori yok"}) - {n.OlusturmaTarihi}")
                            );
                        }
                        break;

                    case "5":
                        // --- Not Ara ---
                        Console.Write("Aranacak kelime: ");
                        var anahtar = Console.ReadLine() ?? "";
                        var bulunan = notlar.FindAll(n => n.EslesiyorMu(anahtar));
                        Console.WriteLine($"\n-- '{anahtar}' içeren notlar ({bulunan.Count}) --");
                        bulunan.ForEach(n =>
                            Console.WriteLine($"[{n.Id}] {n.Baslik} ({n.Kategori?.Isim ?? "Kategori yok"})")
                        );
                        break;

                    case "6":
                        cikis = true;
                        break;

                    default:
                        Console.WriteLine("Geçersiz seçim.");
                        break;
                }
            }

            // 7) Çıkarken verileri kaydet
            notServis.Kaydet(notlar);
            kategoriServis.Kaydet(kategoriler);

            Console.WriteLine("\nProgram sonlandırıldı.");
        }
    }
}
