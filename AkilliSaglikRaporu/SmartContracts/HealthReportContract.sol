// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

/**
 * @title HealthReportContract
 * @dev Akıllı Sağlık Raporu paylaşım kontratı
 */
contract HealthReportContract {
    // Rapor paylaşımı için yapı
    struct ReportShare {
        string ipfsHash;
        address patient;
        address doctor;
        uint256 expiryDate;
        bool isActive;
    }
    
    // Hasta adresinden doktor adresine paylaşım eşlemesi
    // patient => doctor => ReportShare
    mapping(address => mapping(address => ReportShare)) private reportShares;
    
    // Hasta adresinden raporlarının IPFS hash'lerinin eşlemesi
    // patient => ipfsHash[]
    mapping(address => string[]) private patientReports;
    
    // Doktor adresinden erişebileceği hasta adresleri eşlemesi
    // doctor => patient[]
    mapping(address => address[]) private doctorAccesses;
    
    // Olaylar (Events)
    event ReportShared(address indexed patient, address indexed doctor, string ipfsHash, uint256 expiryDate);
    event ReportAccessRevoked(address indexed patient, address indexed doctor, string ipfsHash);
    
    /**
     * @dev Rapor paylaşımı oluşturur
     * @param _doctor Raporun paylaşılacağı doktor adresi
     * @param _ipfsHash Raporun IPFS hash değeri
     * @param _expiryDate Paylaşımın bitiş zamanı (Unix timestamp)
     */
    function shareReport(address _doctor, string memory _ipfsHash, uint256 _expiryDate) public {
        require(_doctor != address(0), "Gecersiz doktor adresi");
        require(bytes(_ipfsHash).length > 0, "Gecersiz IPFS hash");
        require(_expiryDate > block.timestamp, "Gecersiz bitis tarihi");
        
        // Paylaşımı kaydet
        reportShares[msg.sender][_doctor] = ReportShare({
            ipfsHash: _ipfsHash,
            patient: msg.sender,
            doctor: _doctor,
            expiryDate: _expiryDate,
            isActive: true
        });
        
        // Hasta raporlarına ekle
        patientReports[msg.sender].push(_ipfsHash);
        
        // Doktor erişimlerine ekle
        bool doctorExists = false;
        for (uint i = 0; i < doctorAccesses[_doctor].length; i++) {
            if (doctorAccesses[_doctor][i] == msg.sender) {
                doctorExists = true;
                break;
            }
        }
        
        if (!doctorExists) {
            doctorAccesses[_doctor].push(msg.sender);
        }
        
        emit ReportShared(msg.sender, _doctor, _ipfsHash, _expiryDate);
    }
    
    /**
     * @dev Doktorun hastanın raporuna erişim yetkisini kontrol eder ve rapor hash'ini döndürür
     * @param _patient Hastanın adresi
     * @return ipfsHash Raporun IPFS hash değeri
     */
    function getReport(address _patient) public view returns (string memory) {
        ReportShare memory share = reportShares[_patient][msg.sender];
        
        require(share.isActive, "Erisim yetkisi yok");
        require(share.expiryDate > block.timestamp, "Erisim suresi dolmus");
        require(share.doctor == msg.sender, "Sadece yetkili doktor erisebilir");
        
        return share.ipfsHash;
    }
    
    /**
     * @dev Doktorun bir rapora erişim yetkisi olup olmadığını kontrol eder
     * @param _ipfsHash Kontrol edilecek raporun IPFS hash değeri
     * @return bool Erişim yetkisi varsa true, yoksa false
     */
    function hasAccess(string memory _ipfsHash) public view returns (bool) {
        // Doktorun erişebileceği hastaları döngüyle kontrol et
        for (uint i = 0; i < doctorAccesses[msg.sender].length; i++) {
            address patient = doctorAccesses[msg.sender][i];
            ReportShare memory share = reportShares[patient][msg.sender];
            
            // IPFS hash eşleşiyor mu, aktif mi ve süresi dolmamış mı kontrol et
            if (keccak256(bytes(share.ipfsHash)) == keccak256(bytes(_ipfsHash)) && 
                share.isActive && 
                share.expiryDate > block.timestamp) {
                return true;
            }
        }
        
        return false;
    }
    
    /**
     * @dev Doktorun erişim yetkisini iptal eder
     * @param _doctor Erişimi iptal edilecek doktor adresi
     */
    function revokeAccess(address _doctor) public {
        ReportShare storage share = reportShares[msg.sender][_doctor];
        
        require(share.isActive, "Paylasim zaten aktif degil");
        require(share.patient == msg.sender, "Sadece hasta erisimi iptal edebilir");
        
        // Paylaşımı deaktif et
        share.isActive = false;
        
        emit ReportAccessRevoked(msg.sender, _doctor, share.ipfsHash);
    }
    
    /**
     * @dev Hastanın tüm raporlarının IPFS hash'lerini döndürür
     * @return string[] IPFS hash'lerinin dizisi
     */
    function getPatientReports() public view returns (string[] memory) {
        return patientReports[msg.sender];
    }
    
    /**
     * @dev Doktorun erişebileceği hasta adreslerini döndürür
     * @return address[] Hasta adreslerinin dizisi
     */
    function getDoctorPatients() public view returns (address[] memory) {
        return doctorAccesses[msg.sender];
    }
} 