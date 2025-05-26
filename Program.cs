using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                Console.WriteLine("6. Not Güncelle");
                Console.WriteLine("7. Not Sil");
                Console.WriteLine("8. Kategori Güncelle");
                Console.WriteLine("9. Kategori Sil");
                Console.WriteLine("10. Tarihe Göre Filtreleme");
                Console.WriteLine("11. Yarınki Notları Bilgilendir");
                Console.WriteLine("12. Çıkış");
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
                                secilenKategori = kategoriler.Find(k => k.Id == katId);
                        }

                        var yeniNot = new Not
                        {
                            Id = notlar.Any() ? notlar.Max(n => n.Id) + 1 : 1,
                            Baslik = baslik,
                            Icerik = icerik,
                            OlusturmaTarihi = DateTime.Now,
                            Kategori = secilenKategori
                        };
                        notlar.Add(yeniNot);
                        Console.WriteLine($"\n[{yeniNot.Id}] “{yeniNot.Baslik}” eklendi" +
                                          (secilenKategori != null
                                              ? $" (Kategori: {secilenKategori.Isim})"
                                              : "") );
                        break;

                    case "2":
                        // --- Yeni Kategori ---
                        Console.Write("Yeni kategori adı: ");
                        var kategoriAdi = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(kategoriAdi))
                        {
                            var yeniKategori = new Kategori
                            {
                                Id = kategoriler.Any() ? kategoriler.Max(k => k.Id) + 1 : 1,
                                Isim = kategoriAdi
                            };
                            kategoriler.Add(yeniKategori);
                            Console.WriteLine($"\n[{yeniKategori.Id}] {yeniKategori.Isim} oluşturuldu.");
                        }
                        break;

                    case "3":
                        // --- Kategorileri Listele ---
                        Console.WriteLine("\n-- Kategoriler --");
                        if (!kategoriler.Any())
                            Console.WriteLine("Henüz kategori yok.");
                        else
                            kategoriler.ForEach(k => Console.WriteLine($"[{k.Id}] {k.Isim}"));
                        break;

                    case "4":
                        // --- Notları Listele ---
                        Console.WriteLine("\n-- Notlar --");
                        if (!notlar.Any())
                            Console.WriteLine("Hiç not yok.");
                        else
                            notlar.ForEach(n =>
                                Console.WriteLine($"[{n.Id}] {n.Baslik} ({n.Kategori?.Isim ?? "Kategori yok"}) - {n.OlusturmaTarihi}")
                            );
                        break;

                    case "5":
                        // --- Not Ara ---
                        Console.Write("Aranacak kelime: ");
                        var anahtar = Console.ReadLine() ?? "";
                        var bulunan = notlar.Where(n => n.EslesiyorMu(anahtar)).ToList();
                        Console.WriteLine($"\n-- '{anahtar}' içeren notlar ({bulunan.Count}) --");
                        bulunan.ForEach(n =>
                            Console.WriteLine($"[{n.Id}] {n.Baslik} ({n.Kategori?.Isim ?? "Kategori yok"})")
                        );
                        break;

                    case "6":
                        // --- Not Güncelle ---
                        Console.Write("Güncellenecek Not ID: ");
                        if (int.TryParse(Console.ReadLine(), out var gid))
                        {
                            var gn = notlar.Find(n => n.Id == gid);
                            if (gn != null)
                            {
                                Console.Write("Yeni Başlık (enter boş bırakır): ");
                                var nb = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(nb)) gn.Baslik = nb;

                                Console.Write("Yeni İçerik (enter boş bırakır): ");
                                var ni = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(ni)) gn.Icerik = ni;

                                // Kategori güncelleme
                                if (kategoriler.Any())
                                {
                                    Console.WriteLine("Yeni kategori ID gir (enter boş bırakır):");
                                    kategoriler.ForEach(k => Console.WriteLine($"[{k.Id}] {k.Isim}"));
                                    var nk = Console.ReadLine();
                                    if (int.TryParse(nk, out var nkid))
                                        gn.Kategori = kategoriler.Find(k => k.Id == nkid);
                                }

                                Console.WriteLine("Not güncellendi.");
                            }
                            else Console.WriteLine("ID bulunamadı.");
                        }
                        break;

                    case "7":
                        // --- Not Sil ---
                        Console.Write("Silinecek Not ID: ");
                        if (int.TryParse(Console.ReadLine(), out var sid))
                        {
                            var sn = notlar.Find(n => n.Id == sid);
                            if (sn != null)
                            {
                                notlar.Remove(sn);
                                Console.WriteLine("Not silindi.");
                            }
                        }
                        break;

                    case "8":
                        // --- Kategori Güncelle ---
                        Console.Write("Güncellenecek Kat. ID: ");
                        if (int.TryParse(Console.ReadLine(), out var ukid))
                        {
                            var uk = kategoriler.Find(k => k.Id == ukid);
                            if (uk != null)
                            {
                                Console.Write("Yeni isim: ");
                                var ui = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(ui)) uk.Isim = ui;
                                Console.WriteLine("Kategori güncellendi.");
                            }
                        }
                        break;

                    case "9":
                        // --- Kategori Sil ---
                        Console.Write("Silinecek Kat. ID: ");
                        if (int.TryParse(Console.ReadLine(), out var skid))
                        {
                            var sk = kategoriler.Find(k => k.Id == skid);
                            if (sk != null)
                            {
                                // Silinen kategoriyi notlardan da temizle
                                notlar.ForEach(n => { if (n.Kategori?.Id == skid) n.Kategori = null; });
                                kategoriler.Remove(sk);
                                Console.WriteLine("Kategori silindi.");
                            }
                        }
                        break;

                    case "10":
                        // --- Tarihe Göre Filtreleme ---
                        Console.Write("Filtre tarihi (YYYY-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out var dt))
                        {
                            var filt = notlar.Where(n => n.OlusturmaTarihi.Date == dt.Date).ToList();
                            Console.WriteLine($"\n-- {dt:yyyy-MM-dd} tarihli notlar ({filt.Count}) --");
                            filt.ForEach(n => Console.WriteLine($"[{n.Id}] {n.Baslik}"));
                        }
                        break;

                    case "11":
                        // --- Yarınki Notları Bilgilendir ---
                        var yarin = DateTime.Today.AddDays(1);
                        var yn = notlar.Where(n => n.OlusturmaTarihi.Date == yarin).ToList();
                        Console.WriteLine($"\n-- Yarın ({yarin:yyyy-MM-dd}) not sayısı: {yn.Count} --");
                        if (yn.Any())
                            yn.ForEach(n => Console.WriteLine($"[{n.Id}] {n.Baslik}"));
                        break;

                    case "12":
                        cikis = true;
                        break;

                    default:
                        Console.WriteLine("Geçersiz seçim.");
                        break;
                }
            }

            // Çıkarken verileri kaydet
            notServis.Kaydet(notlar);
            kategoriServis.Kaydet(kategoriler);

            Console.WriteLine("\nProgram sonlandırıldı.");
        }
    }
}

