// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

void GenerateKeys()
{
    using (RSA rsa = RSA.Create(2048))
    {
        // Export private + public key
        string privateKeyXml = rsa.ToXmlString(true);  // true = include private params
        File.WriteAllText("private_key.xml", privateKeyXml);
        Console.WriteLine("Private key saved to private_key.xml");

        // Export public key only
        string publicKeyXml = rsa.ToXmlString(false);  // false = public params only
        File.WriteAllText("public_key.xml", publicKeyXml);
        Console.WriteLine("Public key saved to public_key.xml");
    }

    Console.WriteLine("RSA key pair generated.");
}

static string SignData(string data)
{
    // Load your private key (example using a local file)
    var privateKey = RSA.Create();
    var keyXml = System.IO.File.ReadAllText("private_key.xml"); // Replace with your key file path
    privateKey.FromXmlString(keyXml);

    var bytesToSign = Encoding.UTF8.GetBytes(data);
    var signatureBytes = privateKey.SignData(bytesToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    return Convert.ToBase64String(signatureBytes);
}

bool VerifySignature(string data, string signatureBase64)
{
    try
    {
        var publicKey = RSA.Create();
        string keyXml = System.IO.File.ReadAllText("public_key.xml"); // Or load from config/cert
        publicKey.FromXmlString(keyXml);

        var dataBytes = Encoding.UTF8.GetBytes(data);
        var signatureBytes = Convert.FromBase64String(signatureBase64);

        return publicKey.VerifyData(
            dataBytes,
            signatureBytes,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Verification error: {ex.Message}");
        return false;
    }
}

GenerateKeys();

// Generate a random secret
var guid = Guid.NewGuid().ToString();
Console.WriteLine($"Generated GUID: {guid}");

// Sign it using a private RSA key
string signature = SignData(guid);
Console.WriteLine($"Signature: {signature}");

if (VerifySignature(guid, signature))
{
    Console.WriteLine("Signature verification succeeded.");
}
else
{
    Console.WriteLine("Signature verification failed.");
}
