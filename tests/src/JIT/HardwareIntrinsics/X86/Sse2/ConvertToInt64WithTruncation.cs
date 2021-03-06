// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
//

using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace IntelHardwareIntrinsicTest
{
    internal static partial class Program
    {
        private const int Pass = 100;
        private const int Fail = 0;

        static unsafe int Main(string[] args)
        {
            int testResult = Pass;
            int testsCount = 21;
            string methodUnderTestName = nameof(Sse2.ConvertToInt64WithTruncation);

            if (Sse2.IsSupported)
            {
                using (var doubleTable = TestTableScalarSse2<double, long>.Create(testsCount, 2.0))
                {
                    if (Environment.Is64BitProcess)
                    {
                        for (int i = 0; i < testsCount; i++)
                        {
                            (Vector128<double>, Vector128<double>) value = doubleTable[i];
                            long result = Sse2.ConvertToInt64WithTruncation(value.Item1);
                            doubleTable.SetOutArray(result);
                        }

                        CheckMethodEightOne<double, long> checkDouble = (Span<double> x, Span<double> y, long z, ref long a) =>
                        {
                            a = (long)x[0];
                            return a == z;
                        };

                        if (!doubleTable.CheckResult(checkDouble))
                        {
                            PrintError(doubleTable, methodUnderTestName, "(Span<double> x, Span<double> y, long z, ref long a) => ConvertToInt64", checkDouble);
                            testResult = Fail;
                        }
                    }
                    else
                    {
                        try
                        {
                            for (int i = 0; i < testsCount; i++)
                            {
                                (Vector128<double>, Vector128<double>) value = doubleTable[i];
                                long result = Sse2.ConvertToInt64WithTruncation(value.Item1);
                                doubleTable.SetOutArray(result);
                            }
                            testResult = Fail;
                            Console.WriteLine($"{nameof(Sse2)}.{methodUnderTestName} failed: expected PlatformNotSupportedException exception.");
                        }
                        catch (PlatformNotSupportedException)
                        {

                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Sse2.IsSupported: {Sse2.IsSupported}, skipped tests of {typeof(Sse2)}.{methodUnderTestName}");
            }

            return testResult;
        }
    }
}
