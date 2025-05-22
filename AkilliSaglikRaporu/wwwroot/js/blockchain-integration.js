/**
 * Akıllı Sağlık Raporu - Blockchain Entegrasyonu
 * Bu dosya, MetaMask ve Ethereum blockchain ile etkileşim için gerekli fonksiyonları içerir.
 */

// Smart Contract ABI - Bu ABI, SmartContracts/HealthReportContract.sol dosyasından derlenen kontratın ABI'sidir
const contractABI = [
    {
        "inputs": [
            {
                "internalType": "address",
                "name": "_doctor",
                "type": "address"
            }
        ],
        "name": "revokeAccess",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function"
    },
    {
        "inputs": [
            {
                "internalType": "address",
                "name": "_doctor",
                "type": "address"
            },
            {
                "internalType": "string",
                "name": "_ipfsHash",
                "type": "string"
            },
            {
                "internalType": "uint256",
                "name": "_expiryDate",
                "type": "uint256"
            }
        ],
        "name": "shareReport",
        "outputs": [],
        "stateMutability": "nonpayable",
        "type": "function"
    },
    {
        "anonymous": false,
        "inputs": [
            {
                "indexed": true,
                "internalType": "address",
                "name": "patient",
                "type": "address"
            },
            {
                "indexed": true,
                "internalType": "address",
                "name": "doctor",
                "type": "address"
            },
            {
                "indexed": false,
                "internalType": "string",
                "name": "ipfsHash",
                "type": "string"
            }
        ],
        "name": "ReportAccessRevoked",
        "type": "event"
    },
    {
        "anonymous": false,
        "inputs": [
            {
                "indexed": true,
                "internalType": "address",
                "name": "patient",
                "type": "address"
            },
            {
                "indexed": true,
                "internalType": "address",
                "name": "doctor",
                "type": "address"
            },
            {
                "indexed": false,
                "internalType": "string",
                "name": "ipfsHash",
                "type": "string"
            },
            {
                "indexed": false,
                "internalType": "uint256",
                "name": "expiryDate",
                "type": "uint256"
            }
        ],
        "name": "ReportShared",
        "type": "event"
    },
    {
        "inputs": [],
        "name": "getDoctorPatients",
        "outputs": [
            {
                "internalType": "address[]",
                "name": "",
                "type": "address[]"
            }
        ],
        "stateMutability": "view",
        "type": "function"
    },
    {
        "inputs": [],
        "name": "getPatientReports",
        "outputs": [
            {
                "internalType": "string[]",
                "name": "",
                "type": "string[]"
            }
        ],
        "stateMutability": "view",
        "type": "function"
    },
    {
        "inputs": [
            {
                "internalType": "address",
                "name": "_patient",
                "type": "address"
            }
        ],
        "name": "getReport",
        "outputs": [
            {
                "internalType": "string",
                "name": "",
                "type": "string"
            }
        ],
        "stateMutability": "view",
        "type": "function"
    },
    {
        "inputs": [
            {
                "internalType": "string",
                "name": "_ipfsHash",
                "type": "string"
            }
        ],
        "name": "hasAccess",
        "outputs": [
            {
                "internalType": "bool",
                "name": "",
                "type": "bool"
            }
        ],
        "stateMutability": "view",
        "type": "function"
    }
];

// Smart Contract adresi - Gerçek uygulamada bu adres, kontrat deploy edildikten sonra güncellenmelidir
const contractAddress = "0x123456789abcdef0123456789abcdef012345678";

// Web3 ve kontrat nesneleri
let web3;
let healthReportContract;

/**
 * MetaMask'a bağlan ve gerekli nesneleri başlat
 * @returns {Promise<string>} Bağlanan hesabın adresi
 */
async function connectToMetaMask() {
    if (window.ethereum) {
        try {
            // MetaMask'tan hesap erişimi iste
            const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
            
            // Web3 nesnesini oluştur
            web3 = new Web3(window.ethereum);
            
            // Kontrat nesnesini oluştur
            healthReportContract = new web3.eth.Contract(contractABI, contractAddress);
            
            // Bağlanan hesabı döndür
            return accounts[0];
        } catch (error) {
            console.error("MetaMask bağlantısı sırasında hata oluştu:", error);
            throw error;
        }
    } else {
        throw new Error("MetaMask bulunamadı! Lütfen MetaMask eklentisini yükleyin.");
    }
}

/**
 * Raporu doktor ile paylaş
 * @param {string} doctorAddress Doktor Ethereum adresi
 * @param {string} ipfsHash IPFS hash değeri
 * @param {number} expiryDate Paylaşım bitiş tarihi (Unix timestamp)
 * @returns {Promise<string>} İşlem hash'i
 */
async function shareHealthReport(doctorAddress, ipfsHash, expiryDate) {
    try {
        // Bağlı hesabı al
        const accounts = await web3.eth.getAccounts();
        const fromAddress = accounts[0];
        
        // Smart contract fonksiyonunu çağır
        const tx = await healthReportContract.methods.shareReport(
            doctorAddress,
            ipfsHash,
            expiryDate
        ).send({ from: fromAddress });
        
        return tx.transactionHash;
    } catch (error) {
        console.error("Rapor paylaşımı sırasında hata oluştu:", error);
        throw error;
    }
}

/**
 * Doktorun rapora erişim yetkisini kontrol et
 * @param {string} ipfsHash IPFS hash değeri
 * @returns {Promise<boolean>} Erişim yetkisi varsa true, yoksa false
 */
async function checkReportAccess(ipfsHash) {
    try {
        // Bağlı hesabı al
        const accounts = await web3.eth.getAccounts();
        const fromAddress = accounts[0];
        
        // Smart contract fonksiyonunu çağır
        const hasAccess = await healthReportContract.methods.hasAccess(ipfsHash).call({ from: fromAddress });
        
        return hasAccess;
    } catch (error) {
        console.error("Erişim kontrolü sırasında hata oluştu:", error);
        throw error;
    }
}

/**
 * Hasta raporunu getir (doktor için)
 * @param {string} patientAddress Hasta Ethereum adresi
 * @returns {Promise<string>} Raporun IPFS hash değeri
 */
async function getPatientReport(patientAddress) {
    try {
        // Bağlı hesabı al
        const accounts = await web3.eth.getAccounts();
        const fromAddress = accounts[0];
        
        // Smart contract fonksiyonunu çağır
        const ipfsHash = await healthReportContract.methods.getReport(patientAddress).call({ from: fromAddress });
        
        return ipfsHash;
    } catch (error) {
        console.error("Rapor alınırken hata oluştu:", error);
        throw error;
    }
}

/**
 * Doktorun erişim yetkisini iptal et
 * @param {string} doctorAddress Doktor Ethereum adresi
 * @returns {Promise<string>} İşlem hash'i
 */
async function revokeAccess(doctorAddress) {
    try {
        // Bağlı hesabı al
        const accounts = await web3.eth.getAccounts();
        const fromAddress = accounts[0];
        
        // Smart contract fonksiyonunu çağır
        const tx = await healthReportContract.methods.revokeAccess(doctorAddress).send({ from: fromAddress });
        
        return tx.transactionHash;
    } catch (error) {
        console.error("Erişim iptali sırasında hata oluştu:", error);
        throw error;
    }
}

/**
 * Unix timestamp'i JavaScript Date nesnesine dönüştür
 * @param {number} timestamp Unix timestamp
 * @returns {Date} JavaScript Date nesnesi
 */
function timestampToDate(timestamp) {
    return new Date(timestamp * 1000);
}

/**
 * JavaScript Date nesnesini Unix timestamp'e dönüştür
 * @param {Date} date JavaScript Date nesnesi
 * @returns {number} Unix timestamp
 */
function dateToTimestamp(date) {
    return Math.floor(date.getTime() / 1000);
} 