using PeNet;
using Signer.Models;
using SignerAPI.Models;
using System.Diagnostics;

namespace SignerAPI.Domains.ScanDll
{
    public class DllScaner : IDllScaner
    {
        public DllInfo? Dll = null;

        private static readonly string[] PkcsFunctions =
        [
            "C_Initialize",
            "C_Finalize",
            "C_GetInfo",
            "C_GetSlotList",
            "C_GetSlotInfo",
            "C_OpenSession",
            "C_CloseSession",
            "C_Login",
            "C_Logout",
            "C_SignInit",
            "C_Sign",
        ];

        DllInfo? IDllScaner.Dll
        {
            get => Dll;
            set => Dll = value;
        }

        private static bool IsPkcs11Library(string dllPath)
        {
            try
            {
                var pe = new PeFile(dllPath);
                var exports = pe.ExportedFunctions?.Select(f => f.Name).ToList();

                if (exports == null || exports.Count == 0)
                    return false;

                int count = PkcsFunctions.Count(fn =>
                    exports.Any(e => string.Equals(e, fn, StringComparison.OrdinalIgnoreCase)));

                return count >= 11;
            }
            catch
            {
                return false;
            }
        }

        private static Arch GetPkcs11Arch(string dllPath)
        {
            try
            {
                var pe = new PeFile(dllPath);
                return pe.Is64Bit ? Arch.X64 : Arch.X86;
            }
            catch
            {
                // nếu không đọc được PE file thì giữ X86 làm default
                return Arch.X86;
            }
        }

        private static void ScanDirectorySafe(string path, List<DllInfo> result)
        {
            try
            {
                //foreach (var dir in Directory.GetDirectories(path))
                //{
                //    if (IsReparsePoint(dir)) continue;
                //    ScanDirectorySafe(dir, result);
                //}

                foreach (var file in Directory.GetFiles(path, "*.dll"))
                {
                    if (IsPkcs11Library(file))
                    {
                        var info = GetDllInfo(file);
                        result.Add(info);
                    }
                }
            }
            catch
            {
                // bỏ qua lỗi
            }
        }

        private static DllInfo GetDllInfo(string dllPath)
        {
            var info = FileVersionInfo.GetVersionInfo(dllPath);
            var arch = GetPkcs11Arch(dllPath);

            return new DllInfo(
                DllPath: dllPath,
                Company: info.CompanyName ?? "",
                Product: info.ProductName ?? "",
                Description: info.FileDescription ?? "",
                Arch: arch
            );
        }

        public List<DllInfo> Scan()
        {
            string[] commonPaths = [
                @"C:\Windows\System32",
                @"C:\Windows\SysWOW64",
                @"C:\Program Files",
                @"C:\Program Files (x86)"
             ];

            var list = new List<DllInfo>();
            foreach (string commonPath in commonPaths)
            {
                ScanDirectorySafe(commonPath, list);
            }

            return list ?? [];
        }
    }
}
