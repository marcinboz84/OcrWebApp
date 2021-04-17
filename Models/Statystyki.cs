using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OcrWebApp.Models
{
    public class Statystyki
    {
        public string Nazwisko { get; set; }
        public int OcenaOgolna { get; set; }
        public int Bramki { get; set; }
        public int StrzalyCelne { get; set; }
        public int StrzalyNiecelne { get; set; }
    }
}
