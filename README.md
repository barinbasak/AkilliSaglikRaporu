# Akıllı Sağlık Raporu Paylaşımı

Blockchain ve IPFS teknolojileri kullanılarak geliştirilen, güvenli ve zaman kısıtlı sağlık raporu paylaşım sistemi.

## Proje Tanımı

Bu proje, hastaların sağlık raporlarını IPFS üzerinde güvenli bir şekilde depolamasına ve bu raporları belirli doktorlarla belirli bir süre için paylaşmasına olanak tanır. Süre dolduğunda, doktorun rapora erişimi otomatik olarak sona erer.

## Teknolojiler

- **Backend**: ASP.NET Core MVC (.NET 8.0)
- **Veritabanı**: SQL Server (Entity Framework Core)
- **Blockchain**: Ethereum (Sepolia Test Ağı)
- **Merkeziyetsiz Depolama**: IPFS
- **Cüzdan Entegrasyonu**: MetaMask

## Özellikler

- MetaMask ile kullanıcı kimlik doğrulama
- Sağlık raporlarını IPFS'e yükleme
- Raporları belirli bir süre için doktorlarla paylaşma
- Süre dolduğunda otomatik erişim kontrolü
- Doktor paneli ile paylaşılan raporlara erişim

## Kurulum

### Gereksinimler

- .NET 8.0 SDK
- SQL Server
- MetaMask eklentisi
- IPFS erişimi (Infura veya yerel node)

### Adımlar

1. Repoyu klonlayın:
   ```
   git clone https://github.com/yourusername/AkilliSaglikRaporu.git
   ```

2. Proje dizinine gidin:
   ```
   cd AkilliSaglikRaporu
   ```

3. Bağımlılıkları yükleyin:
   ```
   dotnet restore
   ```

4. Veritabanını oluşturun:
   ```
   dotnet ef database update
   ```

5. Uygulamayı çalıştırın:
   ```
   dotnet run
   ```

## Smart Contract

Projenin Ethereum smart contract'ı `SmartContracts/HealthReportContract.sol` dosyasında bulunmaktadır. Bu kontrat, rapor paylaşımı ve erişim kontrolü için gerekli fonksiyonları içerir.

Smart contract'ı Sepolia test ağına deploy etmek için:

1. Remix IDE veya Truffle gibi bir araç kullanın
2. Kontratı derleyin ve deploy edin
3. Deploy edilen kontrat adresini `appsettings.json` dosyasında güncelleyin

## Kullanım Senaryosu

1. Hasta MetaMask ile giriş yapar
2. Sağlık raporunu sisteme yükler (IPFS'e gönderilir)
3. Doktorun Ethereum adresini ve paylaşım süresini girerek raporunu paylaşır
4. Doktor kendi MetaMask hesabıyla giriş yaparak kendisiyle paylaşılan raporları görüntüler
5. Paylaşım süresi dolduğunda, doktor artık rapora erişemez

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakınız.

## İletişim

Sorularınız veya önerileriniz için: [email@example.com](mailto:email@example.com) 