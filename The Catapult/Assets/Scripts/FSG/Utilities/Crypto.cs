using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FSG.Utilities
{
	public sealed class Crypto
	{
		private readonly object cryptLock = new object();

		private readonly string pw;

		private readonly byte[] salt;

		private Aes aes;

		private ICryptoTransform currentEncryptor;

		private ICryptoTransform currentDecryptor;

		private ICryptoTransform encryptor
		{
			get
			{
				if (currentEncryptor == null || !currentEncryptor.CanReuseTransform)
				{
					currentEncryptor = aes.CreateEncryptor();
				}
				return currentEncryptor;
			}
		}

		private ICryptoTransform decryptor
		{
			get
			{
				if (currentDecryptor == null || !currentDecryptor.CanReuseTransform)
				{
					currentDecryptor = aes.CreateDecryptor();
				}
				return currentDecryptor;
			}
		}

		public Crypto(string pw, string salt)
		{
			if (salt.Length != 20)
			{
				throw new Exception("Invalid salt length. Must be 20");
			}
			this.pw = pw;
			this.salt = Encoding.ASCII.GetBytes(salt);
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(this.pw, this.salt);
			aes = new AesManaged();
			aes.Key = rfc2898DeriveBytes.GetBytes(aes.KeySize / 8);
			aes.IV = rfc2898DeriveBytes.GetBytes(aes.BlockSize / 8);
		}

		public byte[] Encrypt(byte[] input)
		{
			lock (cryptLock)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						cryptoStream.Write(input, 0, input.Length);
					}
					return memoryStream.ToArray();
				}
			}
		}

		public byte[] Decrypt(byte[] input)
		{
			lock (cryptLock)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
					{
						cryptoStream.Write(input, 0, input.Length);
					}
					return memoryStream.ToArray();
				}
			}
		}
	}
}
