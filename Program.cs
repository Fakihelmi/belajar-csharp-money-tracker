Console.Write("Siapa nama kamu? ");
string nama = Console.ReadLine() ??"";

decimal saldo = 0;
Console.WriteLine($"halo, {nama}! Selamat Datang di Money Tracker");
Console.WriteLine($"1. Tambah Pemasukan");
Console.WriteLine($"2. Tambah Pengeluaran");
Console.WriteLine($"3. Lihat Saldo");
Console.WriteLine($"4. Keluar");

Console.Write("\nPilih Menu (1-4): ");
string pilihan = Console.ReadLine() ?? "";

switch (pilihan)
{
    case "1":
        Console.WriteLine("Fitur pemasukan akan kita buat berikutnya.");
        break;

    case "2":
        Console.WriteLine("Fitur pengeluaran akan kita buat berikutnya.");
        break;

    case "3":
        Console.WriteLine($"Saldo kamu: Rp{saldo:N0}");
        break;

    case "4":
        Console.WriteLine("Terima kasih sudah menggunakan Money Tracker!");
        break;

    default:
        Console.WriteLine("Pilihan tidak tersedia. Masukkan angka 1-4.");
        break;

}