using NtpNotDefteri.Interfaces;
using System;

namespace NtpNotDefteri.Models
{
    public class Not : IAranabilir
    {
        public int Id { get; set; }
        public string? Baslik { get; set; }
        public string? Icerik { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public Kategori? Kategori { get; set; }

        public bool EslesiyorMu(string aramaKelimesi)
        {
            return (Baslik?.Contains(aramaKelimesi, StringComparison.OrdinalIgnoreCase) == true)
                || (Icerik?.Contains(aramaKelimesi, StringComparison.OrdinalIgnoreCase) == true);
        }
    }
}
