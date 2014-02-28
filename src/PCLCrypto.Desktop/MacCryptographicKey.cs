﻿//-----------------------------------------------------------------------
// <copyright file="MacCryptographicKey.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PCLCrypto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Validation;

    /// <summary>
    /// A .NET Framework implementation of the <see cref="ICryptographicKey"/> interface
    /// for use with MACs.
    /// </summary>
    internal class MacCryptographicKey : CryptographicKey, ICryptographicKey
    {
        /// <summary>
        /// The algorithm to use when hashing.
        /// </summary>
        private readonly MacAlgorithm algorithm;

        /// <summary>
        /// The key material.
        /// </summary>
        private readonly byte[] key;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacCryptographicKey" /> class.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="key">The key.</param>
        internal MacCryptographicKey(MacAlgorithm algorithm, byte[] key)
        {
            Requires.NotNull(key, "key");
            this.algorithm = algorithm;
            this.key = key;
        }

        /// <inheritdoc />
        public int KeySize
        {
            get { return this.key.Length; }
        }

        /// <inheritdoc />
        public byte[] Export(CryptographicPrivateKeyBlobType blobType = CryptographicPrivateKeyBlobType.Pkcs8RawPrivateKeyInfo)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public byte[] ExportPublicKey(CryptographicPublicKeyBlobType blobType = CryptographicPublicKeyBlobType.X509SubjectPublicKeyInfo)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        protected internal override byte[] Sign(byte[] data)
        {
            using (var algorithm = MacAlgorithmProvider.GetAlgorithm(this.algorithm))
            {
                algorithm.Key = this.key;
                return algorithm.ComputeHash(data);
            }
        }

        /// <inheritdoc />
        protected internal override bool VerifySignature(byte[] data, byte[] signature)
        {
            using (var algorithm = MacAlgorithmProvider.GetAlgorithm(this.algorithm))
            {
                algorithm.Key = this.key;
                byte[] computedMac = algorithm.ComputeHash(data);
                return CryptoUtilities.BufferEquals(computedMac, signature);
            }
        }
    }
}
