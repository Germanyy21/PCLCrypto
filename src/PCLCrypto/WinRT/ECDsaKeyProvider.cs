﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL) license. See LICENSE file in the project root for full license information.

namespace PCLCrypto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PInvoke;
    using Validation;
    using static PInvoke.NCrypt;

    /// <summary>
    /// WinRT implementation of the <see cref="IAsymmetricKeyAlgorithmProvider"/> interface.
    /// </summary>
    internal class ECDsaKeyProvider : NCryptAsymmetricKeyProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ECDsaKeyProvider"/> class.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        public ECDsaKeyProvider(AsymmetricAlgorithm algorithm)
            : base(algorithm)
        {
        }

        /// <inheritdoc />
        protected internal override CryptographicPublicKeyBlobType NativePublicKeyFormatEnum => CryptographicPublicKeyBlobType.BCryptPublicKey;

        /// <inheritdoc />
        protected internal override string NativePublicKeyFormatString => AsymmetricKeyBlobTypes.BCRYPT_ECCPUBLIC_BLOB;

        /// <inheritdoc />
        protected internal override IReadOnlyDictionary<CryptographicPrivateKeyBlobType, string> NativePrivateKeyFormats => new Dictionary<CryptographicPrivateKeyBlobType, string>
        {
            { CryptographicPrivateKeyBlobType.BCryptPrivateKey, AsymmetricKeyBlobTypes.BCRYPT_ECCPRIVATE_BLOB },
        };

        /// <inheritdoc />
        protected internal override CryptographicPrivateKeyBlobType PreferredNativePrivateKeyFormat => CryptographicPrivateKeyBlobType.BCryptPrivateKey;
    }
}
