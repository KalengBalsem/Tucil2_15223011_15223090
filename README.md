# Tucil2-IF2211
## Kompresi Gambar dengan Algoritma Quadtree

Program ini merupakan implementasi **algoritma Quadtree** untuk melakukan kompresi gambar. Program membaca file gambar input (dalam format seperti PNG atau JPEG), kemudian membangun struktur Quadtree berdasarkan metode perhitungan galat yang dipilih pengguna. Program ini juga mendukung fitur pembuatan GIF untuk memvisualisasikan proses pembentukan Quadtree secara bertahap.

Algoritma Quadtree bekerja dengan membagi gambar menjadi blok-blok kecil secara rekursif hingga memenuhi kondisi pemberhentian (berdasarkan ambang batas galat dan ukuran blok minimum). Hasil kompresi berupa gambar terkompresi akan disimpan, bersama dengan file GIF animasi (jika fitur GIF diaktifkan). Program mencatat rasio kompresi, waktu eksekusi, dan statistik lainnya.

## Program Requirements & Dependencies
Program ini dibuat menggunakan **Visual Studio Code** dengan **.NET 9.0 SDK**. Berikut adalah beberapa *dependencies* yang diperlukan untuk menjalankan program:

- **.NET 9.0 SDK** atau lebih baru.
- **Package NuGet**:
  - `System.Drawing.Common`: Untuk memproses gambar.
  - `Magick.NET-Q16-AnyCPU`: Untuk pembuatan GIF dan manipulasi gambar tingkat lanjut.
- (Opsional) Terminal dengan dukungan ANSI escape code untuk output berwarna (misalnya, Windows Terminal atau PowerShell).

### Instalasi .NET SDK (Jika Belum Terinstal)
#### Windows:
1. Unduh .NET 9.0 SDK dari situs resmi Microsoft: [Download .NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
2. Ikuti petunjuk instalasi yang diberikan.

#### Linux/macOS:
Jalankan perintah berikut di terminal:
```sh
# Untuk Ubuntu/Debian
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0

# Untuk macOS dengan Homebrew
brew install dotnet
```
Detail lebih lanjut mengenai instalasi .NET SDK dapat diakses di [dokumentasi resmi .NET](https://learn.microsoft.com/en-us/dotnet/core/install/).

### Instalasi Package NuGet
Sebelum menjalankan program, pastikan untuk menginstal package yang diperlukan. Jalankan perintah berikut di direktori proyek (di mana file `.csproj` berada):

```sh
dotnet add package System.Drawing.Common
dotnet add package Magick.NET-Q16-AnyCPU
```

#### Catatan:
- Package `System.Drawing.Common` digunakan untuk memproses gambar (membaca dan menulis piksel).
- Package `Magick.NET-Q16-AnyCPU` digunakan untuk pembuatan GIF dan manipulasi gambar tingkat lanjut. Pastikan package ini terinstal dengan benar, karena fitur GIF tidak akan berfungsi tanpanya.
- Jika Anda menggunakan file eksekusi `QuadtreeCompression.exe`, package ini sudah disertakan dalam build. Namun, jika Anda menjalankan dengan `dotnet run`, Anda perlu menginstalnya terlebih dahulu.

## Cara Menjalankan dan Menggunakan Program
Program ini dapat dijalankan dengan dua cara: menggunakan file eksekusi `QuadtreeCompression.exe` atau melalui perintah `dotnet run` di CLI. Berikut adalah langkah-langkahnya:

### 1. Pastikan .NET SDK Sudah Terinstal
Program ini membutuhkan **.NET 9.0 SDK atau lebih baru**.  
Cek versi .NET yang terinstal dengan perintah:
```sh
dotnet --version
```
Jika belum terinstal, silakan instal sesuai dengan keterangan dalam [Program Requirements & Dependencies](#program-requirements--dependencies).

---

### 2. Menjalankan Program Menggunakan File Eksekusi (`QuadtreeCompression.exe`)
#### Langkah-langkah:
1. Pastikan Anda berada di direktori proyek.
2. Buka folder `bin` dan cari file `QuadtreeCompression.exe`.
3. Jalankan file eksekusi dengan mengklik dua kali (jika menggunakan GUI) atau melalui terminal:
   ```sh
   cd bin
   QuadtreeCompression.exe
   ```
4. Program akan meminta input dari pengguna (lihat bagian [Input dan Menjalankan Program](#input-dan-menjalankan-program)).

#### Catatan:
- File `QuadtreeCompression.exe` hanya dapat dijalankan di sistem operasi yang sesuai dengan build (misalnya, Windows). Jika Anda menggunakan Linux atau macOS, gunakan metode `dotnet run` (lihat langkah berikutnya).
- Pastikan semua *dependencies* (seperti `Magick.NET-Q16-AnyCPU`) sudah terinstal di sistem Anda, meskipun biasanya sudah disertakan dalam build.

---

### 3. Menjalankan Program Menggunakan Perintah `dotnet run`
#### Langkah-langkah:
1. Pastikan Anda berada di direktori proyek (di mana file `.csproj` berada).
2. Instal package NuGet yang diperlukan (jika belum diinstal):
   ```sh
   dotnet add package System.Drawing.Common
   dotnet add package Magick.NET-Q16-AnyCPU
   ```
3. Jalankan perintah berikut di terminal:
   ```sh
   dotnet run
   ```
4. Program akan meminta input dari pengguna (lihat bagian [Input dan Menjalankan Program](#input-dan-menjalankan-program)).

#### Catatan:
- Perintah `dotnet run` akan mengkompilasi ulang proyek sebelum menjalankannya. Pastikan semua *source code* di folder `src` tidak memiliki error.
- Metode ini dapat digunakan di semua sistem operasi (Windows, Linux, macOS) selama .NET SDK terinstal.

---

### 4. Input dan Menjalankan Program
Saat program dijalankan (baik melalui `QuadtreeCompression.exe` atau `dotnet run`), pengguna akan diminta memasukkan beberapa informasi:

1. **Nama file gambar input**:
   ```
   Masukkan nama file gambar input (termasuk ekstensi, misalnya: ./test_input/image.png):
   ```
   - File gambar harus berada di folder `test_input`.
   - Format yang didukung: `.png`, `.jpg`.

2. **Metric**:
   ```
   Pilih metric (1: Varians, 2: MAD, 3: MPD, 4: Entropi, 5: SSIM):
   ```
   - Masukkan angka sesuai metode yang diinginkan (1-5).

3. **Ambang batas galat**:
   ```
   Masukkan ambang batas galat (contoh: 10.0):
   ```
   - Masukkan nilai numerik (floating-point) sebagai ambang batas galat untuk metric.

4. **Ukuran blok minimum**:
   ```
   Masukkan ukuran blok minimum (contoh: 4):
   ```
   - Masukkan nilai integer sebagai ukuran minimum blok (harus merupakan kelipatan 2).

5. **Membuat GIF**:
   ```
   Apakah Anda ingin membuat GIF visualisasi? (y/N):
   ```
   - Masukkan "y" untuk mengaktifkan fitur pembuatan GIF, atau "N" untuk melewatinya.

Program kemudian akan memproses gambar, membangun Quadtree, dan menghasilkan output.

---

### 5. Output dan Penyimpanan Hasil
- **Output di Terminal**:
  - Jika kompresi berhasil, program akan menampilkan:
    ```
    Kompresi selesai!
    Rasio kompresi: 75.32%
    Waktu eksekusi: 1250 ms
    Jumlah simpul daun Quadtree: 1024
    ```
  - Jika fitur GIF diaktifkan, program juga akan mencatat waktu pembuatan GIF:
    ```
    GIF berhasil dibuat!
    Waktu pembuatan GIF: 300 ms
    ```

- **Penyimpanan Hasil**:
  - Gambar terkompresi akan disimpan di folder yang sesuai dengan absolute path, input dari user.
    ```

---

### Contoh Alur Eksekusi
1. **Menjalankan program**:
   ```sh
   cd bin
   QuadtreeCompression.exe
   ```
   atau
   ```sh
   dotnet run
   ```

2. **Masukkan input**:
   ```
   Masukkan nama file gambar input (termasuk ekstensi, misalnya: ./test_input/image.png):
   ./test_input/sample.png
   Pilih metode IQA (1: Varians, 2: MAD, 3: MPD, 4: Entropi, 5: SSIM):
   1
   Masukkan ambang batas galat (contoh: 10.0):
   15.0
   Masukkan ukuran blok minimum (contoh: 4):
   8
   Apakah Anda ingin membuat GIF visualisasi? (ya/tidak):
   ya
   ```

3. **Hasil di terminal**:
   ```
   Kompresi selesai!
   Rasio kompresi: 68.45%
   Waktu eksekusi: 980 ms
   Jumlah simpul daun Quadtree: 896
   GIF berhasil dibuat!
   Waktu pembuatan GIF: 250 ms
   Apakah Anda ingin menyimpan statistik kompresi ke file teks? (ya/tidak):
   ya
   ```

4. **File output**:
   - Gambar terkompresi: `test/sample_compressed.png`
   - File GIF: `test/sample_quadtree.gif`
   - Statistik: `test/sample_stats.txt`

## Author
| Nama              | NIM      | Kelas IF2211 |
|-------------------|----------|--------------|
| Asybel B.P.     | 15223011 | K1       |
| Ignacio K.A.     | 15223090 | K1       |

---

### Catatan Tambahan
- **Performa**:
  - Waktu eksekusi sangat bergantung pada ukuran gambar, metode IQA yang dipilih, dan parameter seperti ambang batas galat dan ukuran blok minimum.
  - Untuk gambar besar (misalnya, 4096x4096 piksel), disarankan menggunakan ambang batas galat yang lebih besar untuk mengurangi waktu pemrosesan.