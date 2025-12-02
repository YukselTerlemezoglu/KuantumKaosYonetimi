using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KuantumKaosYönetimiCsharp
{
    // ==== Custom Exception ====
    public class KuantumCokusuException : Exception
    {
        public KuantumCokusuException(string message) : base(message)
        {
        }
    }

    // ==== Interface ====
    public interface IKritik
    {
        void AcilDurumSogutmasi();
    }

    // ==== Abstract Base Class ====
    public abstract class KuantumNesnesi
    {
        private double stabilite; // 0-100 arası tutulacak

        public string ID { get; set; }

        public double Stabilite
        {
            get => stabilite;
            protected set
            {
                if (value < 0)
                {
                    stabilite = 0;
                }
                else if (value > 100)
                {
                    stabilite = 100;
                }
                else
                {
                    stabilite = value;
                }
            }
        }

        public int TehlikeSeviyesi { get; protected set; } // 1-10

        protected KuantumNesnesi(string id, double stabilite, int tehlikeSeviyesi)
        {
            ID = id;
            Stabilite = stabilite;
            TehlikeSeviyesi = tehlikeSeviyesi;
        }

        protected void AzaltStabilite(double miktar)
        {
            Stabilite -= miktar;
            if (Stabilite <= 0)
            {
                throw new KuantumCokusuException($"Kuantum Çöküşü! Patlayan Nesne ID: {ID}");
            }
        }

        protected void ArttirStabilite(double miktar)
        {
            Stabilite += miktar;
        }

        public abstract void AnalizEt();

        public virtual string DurumBilgisi()
        {
            return $"[{GetType().Name}] ID: {ID}, Stabilite: {Stabilite:F2}, Tehlike: {TehlikeSeviyesi}";
        }
    }

    // ==== Concrete Classes ====
    public class VeriPaketi : KuantumNesnesi
    {
        public VeriPaketi(string id, double stabilite, int tehlikeSeviyesi)
            : base(id, stabilite, tehlikeSeviyesi)
        {
        }

        public override void AnalizEt()
        {
            Console.WriteLine("Veri içeriği okundu.");
            AzaltStabilite(5);
        }
    }

    public class KaranlikMadde : KuantumNesnesi, IKritik
    {
        public KaranlikMadde(string id, double stabilite, int tehlikeSeviyesi)
            : base(id, stabilite, tehlikeSeviyesi)
        {
        }

        public override void AnalizEt()
        {
            Console.WriteLine("Karanlık madde analizi yapılıyor...");
            AzaltStabilite(15);
        }

        public void AcilDurumSogutmasi()
        {
            Console.WriteLine("Karanlık madde için acil durum soğutması uygulanıyor...");
            ArttirStabilite(50);
        }
    }

    public class AntiMadde : KuantumNesnesi, IKritik
    {
        public AntiMadde(string id, double stabilite, int tehlikeSeviyesi)
            : base(id, stabilite, tehlikeSeviyesi)
        {
        }

        public override void AnalizEt()
        {
            Console.WriteLine("Evrenin dokusu titriyor...");
            AzaltStabilite(25);
        }

        public void AcilDurumSogutmasi()
        {
            Console.WriteLine("Antimadde için acil durum soğutması uygulanıyor...");
            ArttirStabilite(50);
        }
    }

    class Program
    {
        static List<KuantumNesnesi> envanter = new List<KuantumNesnesi>();
        static Random rnd = new Random();
        static int sayac = 1;

        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("KUANTUM AMBARI KONTROL PANELİ");
                    Console.WriteLine("1. Yeni Nesne Ekle (Rastgele)");
                    Console.WriteLine("2. Tüm Envanteri Listele");
                    Console.WriteLine("3. Nesneyi Analiz Et (ID ile)");
                    Console.WriteLine("4. Acil Durum Soğutması Yap (ID ile)");
                    Console.WriteLine("5. Çıkış");
                    Console.Write("Seçiminiz: ");

                    string secim = Console.ReadLine();
                    Console.WriteLine();

                    switch (secim)
                    {
                        case "1":
                            YeniNesneEkle();
                            break;
                        case "2":
                            EnvanteriListele();
                            break;
                        case "3":
                            NesneAnalizEt();
                            break;
                        case "4":
                            SogutmaYap();
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Geçersiz seçim!\n");
                            break;
                    }

                    Console.WriteLine();
                }
            }
            catch (KuantumCokusuException ex)
            {
                Console.WriteLine();
                Console.WriteLine("SİSTEM ÇÖKTÜ! TAHLİYE BAŞLATILIYOR...");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Program sonlandı.");
        }

        static void YeniNesneEkle()
        {
            int tip = rnd.Next(3); // 0: Veri, 1: Karanlık, 2: Anti
            double stabilite = rnd.Next(60, 101); // 60-100
            int tehlike = rnd.Next(1, 11);        // 1-10

            KuantumNesnesi nesne;
            string id;

            if (tip == 0)
            {
                id = $"V-{sayac++}";
                nesne = new VeriPaketi(id, stabilite, tehlike);
            }
            else if (tip == 1)
            {
                id = $"K-{sayac++}";
                nesne = new KaranlikMadde(id, stabilite, tehlike);
            }
            else
            {
                id = $"A-{sayac++}";
                nesne = new AntiMadde(id, stabilite, tehlike);
            }

            envanter.Add(nesne);
            Console.WriteLine($"Yeni nesne eklendi: {nesne.DurumBilgisi()}");
        }

        static void EnvanteriListele()
        {
            if (envanter.Count == 0)
            {
                Console.WriteLine("Envanter boş.");
                return;
            }

            Console.WriteLine("=== ENVANTER DURUMU ===");
            foreach (var nesne in envanter)
            {
                Console.WriteLine(nesne.DurumBilgisi());
            }
        }

        static KuantumNesnesi NesneBul(string id)
        {
            foreach (var n in envanter)
            {
                if (n.ID.Equals(id, StringComparison.OrdinalIgnoreCase))
                    return n;
            }
            return null;
        }

        static void NesneAnalizEt()
        {
            if (envanter.Count == 0)
            {
                Console.WriteLine("Depo tamamen boş.");
                return;
            }

            Console.Write("Analiz edilecek nesnenin ID'sini giriniz: ");
            string id = Console.ReadLine();

            var nesne = NesneBul(id);
            if (nesne == null)
            {
                Console.WriteLine("Nesne bulunamadı!");
                return;
            }

            else
            {
                nesne.AnalizEt();
                Console.WriteLine("Güncel durum: " + nesne.DurumBilgisi());
            }
        }

        static void SogutmaYap()
        {
            if (envanter.Count == 0)
            {
                Console.WriteLine("Depo tamamen boş.");
                return;
            }

            Console.Write("Soğutma yapılacak nesnenin ID'sini giriniz: ");
            string id = Console.ReadLine();

            var nesne = NesneBul(id);
            if (nesne == null)
            {
                Console.WriteLine("Nesne bulunamadı!");
                return;
            }

            if (nesne is IKritik kritikNesne)
            {
                kritikNesne.AcilDurumSogutmasi();
                Console.WriteLine("Güncel durum: " + nesne.DurumBilgisi());
            }
            else
            {
                Console.WriteLine("Bu nesne soğutulamaz! (IKritik değil)");
            }
        }
    }
}