using System.Globalization;
using System.Text.Json;

CultureInfo indonesia = new CultureInfo("id-ID");

string lokasiData = Path.Combine(
    AppContext.BaseDirectory,
    "transaksi.json"
);

List<Transaksi> daftarTransaksi = new List<Transaksi>();

try
{
    if (File.Exists(lokasiData))
    {
        string isiFile = File.ReadAllText(lokasiData);

        daftarTransaksi =
            JsonSerializer.Deserialize<List<Transaksi>>(isiFile)
            ?? new List<Transaksi>();
    }
}
catch (Exception error)
{
    Console.WriteLine($"Data gagal dibaca: {error.Message}");
    Console.WriteLine("Program dihentikan agar data lama tidak tertimpa.");
    return;
}

decimal saldo = 0;

foreach (Transaksi transaksi in daftarTransaksi)
{
    if (transaksi.Jenis == "Pemasukan")
    {
        saldo += transaksi.Nominal;
    }
    else if (transaksi.Jenis == "Pengeluaran")
    {
        saldo -= transaksi.Nominal;
    }
}

Console.Write("Siapa nama kamu? ");
string nama = Console.ReadLine() ?? "";

if (string.IsNullOrWhiteSpace(nama))
{
    nama = "Teman";
}

Console.WriteLine($"\nHalo, {nama}! Selamat datang di Money Tracker.");

bool berjalan = true;

while (berjalan)
{
    Console.WriteLine("\n=== MONEY TRACKER ===");
    Console.WriteLine("1. Tambah pemasukan");
    Console.WriteLine("2. Tambah pengeluaran");
    Console.WriteLine("3. Lihat saldo");
    Console.WriteLine("4. Lihat riwayat transaksi");
    Console.WriteLine("5. Keluar");

    Console.Write("\nPilih menu (1-5): ");
    string? pilihan = Console.ReadLine();

    if (pilihan == null)
    {
        break;
    }

    switch (pilihan)
    {
        case "1":
        {
            Console.Write("Masukkan pemasukan lebih dari 50000: ");

            if (!BacaNominal(Console.ReadLine(), out decimal pemasukan)
                || pemasukan <= 50000)
            {
                Console.WriteLine(
                    "Input harus berupa angka lebih dari 50000, tanpa titik atau koma."
                );
                break;
            }

            Console.Write("Keterangan pemasukan: ");
            string keterangan = Console.ReadLine() ?? "";

            Transaksi transaksiBaru = new Transaksi
            {
                Tanggal = DateTime.Now,
                Jenis = "Pemasukan",
                Nominal = pemasukan,
                Keterangan = keterangan
            };

            daftarTransaksi.Add(transaksiBaru);

            if (SimpanData())
            {
                saldo += pemasukan;

                Console.WriteLine(
                    $"Pemasukan {Rupiah(pemasukan)} berhasil disimpan."
                );
                Console.WriteLine($"Saldo sekarang: {Rupiah(saldo)}");
            }
            else
            {
                daftarTransaksi.Remove(transaksiBaru);
            }

            break;
        }

        case "2":
        {
            Console.Write("Masukkan pengeluaran (contoh: 10000): ");

            if (!BacaNominal(Console.ReadLine(), out decimal pengeluaran)
                || pengeluaran <= 0)
            {
                Console.WriteLine(
                    "Input harus berupa angka lebih dari 0, tanpa titik atau koma."
                );
                break;
            }

            if (pengeluaran > saldo)
            {
                Console.WriteLine("Saldo tidak cukup.");
                break;
            }

            Console.Write("Keterangan pengeluaran: ");
            string keterangan = Console.ReadLine() ?? "";

            Transaksi transaksiBaru = new Transaksi
            {
                Tanggal = DateTime.Now,
                Jenis = "Pengeluaran",
                Nominal = pengeluaran,
                Keterangan = keterangan
            };

            daftarTransaksi.Add(transaksiBaru);

            if (SimpanData())
            {
                saldo -= pengeluaran;

                Console.WriteLine(
                    $"Pengeluaran {Rupiah(pengeluaran)} berhasil disimpan."
                );
                Console.WriteLine($"Sisa saldo: {Rupiah(saldo)}");
            }
            else
            {
                daftarTransaksi.Remove(transaksiBaru);
            }

            break;
        }

        case "3":
        {
            Console.WriteLine($"\nSaldo kamu: {Rupiah(saldo)}");
            break;
        }

        case "4":
        {
            Console.WriteLine("\n=== RIWAYAT TRANSAKSI ===");

            if (daftarTransaksi.Count == 0)
            {
                Console.WriteLine("Belum ada transaksi.");
                break;
            }

            decimal totalPemasukan = 0;
            decimal totalPengeluaran = 0;
            int nomor = 1;

            foreach (Transaksi transaksi in daftarTransaksi)
            {
                Console.WriteLine(
                    $"{nomor}. {transaksi.Tanggal:dd/MM/yyyy HH:mm} | " +
                    $"{transaksi.Jenis} | {Rupiah(transaksi.Nominal)}"
                );

                Console.WriteLine(
                    $"   Keterangan: {transaksi.Keterangan}"
                );

                if (transaksi.Jenis == "Pemasukan")
                {
                    totalPemasukan += transaksi.Nominal;
                }
                else if (transaksi.Jenis == "Pengeluaran")
                {
                    totalPengeluaran += transaksi.Nominal;
                }

                nomor++;
            }

            Console.WriteLine(
                $"\nTotal pemasukan  : {Rupiah(totalPemasukan)}"
            );
            Console.WriteLine(
                $"Total pengeluaran: {Rupiah(totalPengeluaran)}"
            );
            Console.WriteLine($"Saldo akhir      : {Rupiah(saldo)}");

            break;
        }

        case "5":
        {
            berjalan = false;
            Console.WriteLine($"Sampai jumpa, {nama}!");
            break;
        }

        default:
        {
            Console.WriteLine("Pilihan tidak tersedia. Pilih angka 1-5.");
            break;
        }
    }
}

string Rupiah(decimal nominal)
{
    return nominal.ToString("C0", indonesia);
}

bool BacaNominal(string? input, out decimal nominal)
{
    return decimal.TryParse(
        input?.Trim(),
        NumberStyles.None,
        CultureInfo.InvariantCulture,
        out nominal
    );
}

bool SimpanData()
{
    string lokasiSementara = lokasiData + ".tmp";

    try
    {
        JsonSerializerOptions pengaturan = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(
            daftarTransaksi,
            pengaturan
        );

        File.WriteAllText(lokasiSementara, json);
        File.Move(lokasiSementara, lokasiData, overwrite: true);

        return true;
    }
    catch (Exception error)
    {
        Console.WriteLine($"Transaksi gagal disimpan: {error.Message}");
        return false;
    }
}

class Transaksi
{
    public DateTime Tanggal { get; set; }
    public string Jenis { get; set; } = "";
    public decimal Nominal { get; set; }
    public string Keterangan { get; set; } = "";
}