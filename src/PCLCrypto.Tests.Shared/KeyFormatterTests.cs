﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL) license. See LICENSE file in the project root for full license information.

#if !SILVERLIGHT
namespace PCLCrypto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Formatters;
    using Xunit;
    using Xunit.Abstractions;

    public class KeyFormatterTests
    {
        private static Lazy<RSAParameters> rsaParameters;

        private readonly ITestOutputHelper logger;

        static KeyFormatterTests()
        {
            rsaParameters = new Lazy<RSAParameters>(() =>
            {
                var algorithm = WinRTCrypto.AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithm.RsaOaepSha1);
                using (var key = algorithm.CreateKeyPair(512))
                {
                    const CryptographicPrivateKeyBlobType keyBlobFormat = CryptographicPrivateKeyBlobType.BCryptFullPrivateKey;
                    byte[] bcryptNative = key.Export(keyBlobFormat);
                    var rsaParameters = KeyFormatter.GetFormatter(keyBlobFormat).Read(bcryptNative);
                    return rsaParameters;
                }
            });
        }

        public KeyFormatterTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [SkippableTheory(typeof(NotSupportedException)), CombinatorialData]
        public void KeyFormatters_PrivateKeyRoundTrip(CryptographicPrivateKeyBlobType format)
        {
            this.logger.WriteLine("Generated RSA parameters:");
            this.LogRSAParameters(rsaParameters.Value, "  ");

            var formatter = KeyFormatter.GetFormatter(format);
            byte[] custom = formatter.Write(rsaParameters.Value);
            var rsaParametersRead = formatter.Read(custom);

            this.logger.WriteLine("Read RSA parameters:");
            this.LogRSAParameters(rsaParametersRead, "  ");

            Assert.Equal<byte>(rsaParameters.Value.Exponent, rsaParametersRead.Exponent);
            Assert.Equal<byte>(rsaParameters.Value.Modulus, rsaParametersRead.Modulus);

            Assert.Equal<byte>(rsaParameters.Value.P, rsaParametersRead.P);
            Assert.Equal<byte>(rsaParameters.Value.Q, rsaParametersRead.Q);

            if (format != CryptographicPrivateKeyBlobType.BCryptPrivateKey)
            {
                Assert.Equal<byte>(rsaParameters.Value.D, rsaParametersRead.D);
                Assert.Equal<byte>(rsaParameters.Value.DP, rsaParametersRead.DP);
                Assert.Equal<byte>(rsaParameters.Value.DQ, rsaParametersRead.DQ);
                Assert.Equal<byte>(rsaParameters.Value.InverseQ, rsaParametersRead.InverseQ);
            }
            else
            {
                // BCryptPrivateKey is lossy, by design.
                Assert.Null(rsaParametersRead.D);
                Assert.Null(rsaParametersRead.DP);
                Assert.Null(rsaParametersRead.DQ);
                Assert.Null(rsaParametersRead.InverseQ);
            }
        }

        [Theory, CombinatorialData]
        public void KeyFormatters_PublicKeyRoundTrip(CryptographicPublicKeyBlobType format)
        {
            var formatter = KeyFormatter.GetFormatter(format);
            byte[] custom = formatter.Write(rsaParameters.Value, includePrivateKey: false);
            var rsaParametersRead = formatter.Read(custom);

            Assert.Equal<byte>(rsaParameters.Value.Exponent, rsaParametersRead.Exponent);
            Assert.Equal<byte>(rsaParameters.Value.Modulus, rsaParametersRead.Modulus);

            Assert.Null(rsaParametersRead.D);
            Assert.Null(rsaParametersRead.P);
            Assert.Null(rsaParametersRead.Q);
            Assert.Null(rsaParametersRead.DP);
            Assert.Null(rsaParametersRead.DQ);
            Assert.Null(rsaParametersRead.InverseQ);
        }

/*
          Modulus: a4d5f49f3298500af851b031d27754fd63b8df7f37508b2bea15794ae706abc4cc790d5c8f4bac7ac46ac770b53830a28e97fd3bd9d2afdd18b8db9266965413
          Exponent: 010001
          P: e6505d775acbc8077462f0cdbe22a59fc6c75758a9a097211bc4e071c963e415
          D: 6b87270cb2f4a9427ebacb35b516235b28b271198bfbfecda6e65b39817bd8907b0e7051b74ddb728f1f29220cef00095d63c224d5a148e14e15a9cb4c6849
          Q: b73823d2929601f4f95050e17de1587841cbdc4152444f2352d9f83f54d71987
          DP: 200f81e352855994081499d6da27f28c5a5c77814523b0c6101a88efee0bf4bd
          DQ: 1471d6457c07f325f3e00b766e068449bf05d1891475fce2b32f116d77b91ce7
          InverseQ: c7e4c27f6596dec9f8d18eb3ccead992ca2ad7241a4abffdecaa5ad5bf965895
*/
        private static RSAParameters CreateRSAParametersWithShortD()
        {
            return new RSAParameters
            {
                // REPLACE THIS COMMENT WITH content of block comment above
            };
        }

        private void LogRSAParameters(RSAParameters parameters, string indent = "")
        {
            Action<string> logValue = name =>
            {
                byte[] value = (byte[])typeof(RSAParameters).GetTypeInfo().GetDeclaredField(name).GetValue(parameters);
                if (value != null)
                {
                    this.logger.WriteLine($"{indent}{name}: {WinRTCrypto.CryptographicBuffer.EncodeToHexString(value)}");
                }
            };
            logValue(nameof(RSAParameters.Modulus));
            logValue(nameof(RSAParameters.Exponent));
            logValue(nameof(RSAParameters.P));
            logValue(nameof(RSAParameters.D));
            logValue(nameof(RSAParameters.Q));
            logValue(nameof(RSAParameters.DP));
            logValue(nameof(RSAParameters.DQ));
            logValue(nameof(RSAParameters.InverseQ));
        }
    }
}
#endif
