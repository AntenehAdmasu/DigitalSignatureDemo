using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace DigtalSignatureDemo
{
    class SignatureGenerator
    {
        private RsaKeyParameters privateKey;
        private RsaKeyParameters publicKey;
        

        TextWriter textWriter;
        PemWriter pemWriter;

        public SignatureGenerator()
        {

            RsaKeyPairGenerator rsaKeyPairGen = new RsaKeyPairGenerator();
            rsaKeyPairGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            AsymmetricCipherKeyPair keyPair = rsaKeyPairGen.GenerateKeyPair();

            privateKey = (RsaKeyParameters)keyPair.Private;
            publicKey = (RsaKeyParameters)keyPair.Public;                 

        }
        public string getPublicKeyString()
        {
            String stringpublicKey;

            textWriter = new StringWriter();
            pemWriter = new PemWriter(textWriter);
            pemWriter.Writer.Flush();
            pemWriter.WriteObject(publicKey);
            stringpublicKey = textWriter.ToString();

            return stringpublicKey;
        }

        public string getPrivateKeyString()
        {
            String stringprivateKey;

            textWriter = new StringWriter();
            pemWriter = new PemWriter(textWriter);
            pemWriter.Writer.Flush();
            pemWriter.WriteObject(publicKey);
            stringprivateKey = textWriter.ToString();

            return stringprivateKey;
        }

        public string HashFile(String file_content)
        {
            ISigner sign = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha1WithRsaEncryption.Id);
            sign.Init(true, privateKey);
            byte[] fileInBytes = ASCIIEncoding.ASCII.GetBytes(file_content);
            sign.BlockUpdate(fileInBytes, 0, fileInBytes.Length);
            byte[] signature = sign.GenerateSignature();
            string fileHash = ByteArrayToString(signature);
            return fileHash;
        }
        public bool verifySignature(byte[] file_content, byte[] signature)
        {
            ISigner sign1 = SignerUtilities.GetSigner(PkcsObjectIdentifiers.Sha1WithRsaEncryption.Id);
            sign1.Init(false, publicKey);
            sign1.BlockUpdate(file_content, 0, file_content.Length);
            bool status = sign1.VerifySignature(signature);

            Console.WriteLine();
            Console.Write("Status of the digital signature : " + " ");

            return status;

        }
        

        private static string ByteArrayToString(byte[] byteArray)
        {
            StringBuilder mystring = new StringBuilder(byteArray.Length);
            for (int i = 0; i < byteArray.Length; i++)
            {
                mystring.Append(byteArray[i].ToString("X").ToLower());
            }
            return mystring.ToString();
        }

    }
}
