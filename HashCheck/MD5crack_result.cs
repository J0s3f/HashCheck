using System;
using System.Security.Cryptography;
using System.Text;
namespace HashCheck
{
	public struct MD5crack_result
	{
		private string Hash;
		private bool cracked;
		private string clear;
		public bool isCracked()
		{
			return this.cracked;
		}
		public string getHash()
		{
			return this.Hash.ToLower();
		}
		public string getClear()
		{
			if (!this.cracked)
			{
				throw new Exception("Hash not cracked!");
			}
			return this.clear;
		}
		public void enterCrackResult(string cracked_hash)
		{
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(cracked_hash);
			byte[] value = mD.ComputeHash(bytes);
			string text = BitConverter.ToString(value);
			if (text.ToLower().Replace("-", "") == this.Hash.ToLower())
			{
				this.cracked = true;
				this.clear = cracked_hash;
				return;
			}
			throw new Exception("Cleartext does not match cracked Hash!");
		}
		public MD5crack_result(string hash)
		{
			this.Hash = hash;
			this.cracked = false;
			this.clear = "";
		}
	}
}
